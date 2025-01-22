using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace Collections;

public class UniversalisClient
{
    public Dictionary<uint, MarketplaceItemData> itemToMarketplaceData = new();
    private const string Fields = "listings.pricePerUnit,averagePriceNQ,averagePriceHQ";

    private readonly HttpClient httpClient =
    new(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All })
    {
        Timeout = TimeSpan.FromMilliseconds(10000)
    };

    private World? homeWorld = null;

    public void Dispose()
    {
        httpClient.Dispose();
    }
    public async Task populateMarketBoardData(uint itemId)
    {
        if (itemToMarketplaceData.ContainsKey(itemId))
        {
            return;
        }
        itemToMarketplaceData[itemId] = await GetMarketBoardData(itemId).ConfigureAwait(false);
    }
    public async Task<MarketplaceItemData?> GetMarketBoardData(uint itemId)
    {
        var world = Services.ClientState.LocalPlayer?.CurrentWorld.Value;
        if (world != null)
        {
            homeWorld = (World)world;
        }
        var worldData = await GetMarketBoardDataInternal(itemId, homeWorld.Value.Name.ToString());
        var DCData = await GetMarketBoardDataInternal(itemId, homeWorld.Value.DataCenter.Value.Name.ToString());
        return ParseMarketplaceItemData(worldData, DCData);
    }

    private async Task<UniversalisItemData?> GetMarketBoardDataInternal(uint itemId, string worldDcRegion)
    {
        try
        {
            using var result = await httpClient.GetAsync($"https://universalis.app/api/v2/{worldDcRegion}/{itemId}?Fields={Fields}");

            if (result.StatusCode != HttpStatusCode.OK)
            {
                Dev.Log("Bad HttpStatusCode: " + result.StatusCode.GetEnumName());
                return null;
            }

            await using var responseStream = await result.Content.ReadAsStreamAsync();
            var item = await JsonSerializer.DeserializeAsync<UniversalisItemData>(responseStream);
            if (item == null)
            {
                return null;
            }

            return item;
        }
        catch (Exception e)
        {
            Dev.Log("Caught exception" + e.ToString());
            return null;
        }
    }

    private MarketplaceItemData ParseMarketplaceItemData(UniversalisItemData worldData, UniversalisItemData DCData)
    {
        return new MarketplaceItemData()
        {
            minPriceWorld = worldData.listings.FirstOrDefault()?.pricePerUnit,
            minPriceDC = DCData.listings.FirstOrDefault()?.pricePerUnit,
            avgPriceWorld = Math.Min(worldData.averagePriceNQ, worldData.averagePriceHQ),
            avgPriceDC = Math.Min(DCData.averagePriceNQ, DCData.averagePriceHQ),
        };
    }

    public class MarketplaceItemData
    {
        public double? minPriceWorld;
        public double avgPriceWorld;
        public double? minPriceDC;
        public double avgPriceDC;
    }

    private class UniversalisItemData
    {
        public List<Listing> listings { get; set; }
        public double averagePriceNQ { get; set; }
        public double averagePriceHQ { get; set; }

        public class Listing
        {
            public long pricePerUnit { get; set; }
        }
    }
}
