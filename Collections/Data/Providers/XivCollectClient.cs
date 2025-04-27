using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace Collections;

// Mainly provides us with sources for collectibles that LuminaSupplemental does not have.
public class XivCollectClient 
{
    public Dictionary<string, CollectionData> characterNameToCollectionData = new();
    public static Dictionary<string, string> collectionNameToXivEndpoint = new Dictionary<string, string>{
        {BardingCollectible.CollectionName, "bardings"},
        {BlueMageCollectible.CollectionName, "spells"},
        {EmoteCollectible.CollectionName, "emotes"},
        {FashionAccessoriesCollectible.CollectionName, "fashions"},
        {GlassesCollectible.CollectionName, "facewear"},
        {FramerKitCollectible.CollectionName, "frames"},
        {HairstyleCollectible.CollectionName, "hairstyles"},
        {MinionCollectible.CollectionName, "minions"},
        {MountCollectible.CollectionName, "mounts"},
        {OrchestrionCollectible.CollectionName, "orchestrions"},
        {OutfitsCollectible.CollectionName, "outfits"},
        {TripleTriadCollectible.CollectionName, "triad/cards"},
    };

    private readonly HttpClient httpClient =
    new(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All })
    {
        Timeout = TimeSpan.FromMilliseconds(10000)
    };

    public void Dispose()
    {
        httpClient.Dispose();
    }

    private async Task<List<CollectibleResult>?> GetXivCollectDataInternal(string collectionEndpoint)
    {
        try {
            var client = new HttpClient();
            // I have no idea why, but trying to use GetAsync with the below URL cuts off anything past the root URL.
            // so we HAVE to make a HttpRequestMessage object with the URL and send that =/
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://ffxivcollect.com/api/{collectionEndpoint}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var items = await JsonSerializer.DeserializeAsync<XivRequestResult>(responseStream);
            return items.results;

        }
        catch (Exception e)
        {
            Dev.Log("Caught exception" + e.ToString());
            return null;
        }
    }

    public async Task<bool> FetchXivCollectData()
    {
        var collections = Services.DataProvider.collections;
        foreach(var collection in collections)
        {
            var items = await GetXivCollectDataInternal(collection.Value.name);
            if(items != null)
                await GenerateResourceSheetsFromXivCollect(items, collection.Value.collection);
        }
        return true;
    }

    public async Task<bool> GenerateResourceSheetsFromXivCollect(List<CollectibleResult> items, List<ICollectible> collection)
    {

        List<ItemIdToSource> mogStationSources = [];
        List<ItemIdToSource> eventSources = [];
        List<ItemIdToSource> achievementSources = [];
        List<ItemIdToSource> questSources = [];
        List<ItemIdToSource> craftingSources = [];
        List<ItemIdToSource> dutySources = [];
        List<ItemIdToSource> miscSources = [];
        // since this is only supposed to run on develop, we can afford to be inefficient
        var contentFinderConditionList = ExcelCache<ContentFinderCondition>.GetSheet();
        var instanceIdToFinderId = new Dictionary<uint, uint>();
        instanceIdToFinderId.Add(0, 0);
        foreach(var entry in contentFinderConditionList)
        {
            if(entry.Content.RowId != 0 && entry.Name != "" && !instanceIdToFinderId.ContainsKey(entry.Content.RowId))
            {
                instanceIdToFinderId.Add(entry.Content.RowId, entry.RowId);
            }
        }
        Dev.Log();

        foreach(var result in items)
        {
            Dev.Log($"Processing Item {result.name} with ID {result.id} and Item ID {result.item_id}");
            // name sent by XivCollect API follows uppercase on all names.
            var searchResult = collection.Where((c) => c.GetDisplayName().UpperCaseAfterSpaces() == result.name).FirstOrDefault();
            if(searchResult != null) Dev.Log($"Found existing item with name {searchResult.GetDisplayName().UpperCaseAfterSpaces()}");
            foreach(var source in result.sources)
            {
                uint idToUse = (uint)result.id;
                if(result.item_id != null) idToUse = (uint)result.item_id;
                if(searchResult != null) idToUse = searchResult.Id;
                // find instance
                switch(source.type)
                {
                    case "Premium":
                        mogStationSources.Add(new ItemIdToSource {Collection = "Outfits", ItemId = idToUse, SourceId = 0, SourceDescription = source.text});
                        break;
                    case "Event":
                    case "Limited":
                    case "Purchase":
                        eventSources.Add(new ItemIdToSource {Collection = "Outfits", ItemId = idToUse, SourceId = 0, SourceDescription = source.text});
                        break;
                    case "Quest":
                        questSources.Add(new ItemIdToSource {Collection = "Outfits", ItemId = idToUse, SourceId = (uint?)source.related_id ?? 0, SourceDescription = source.text});
                        break;
                    case "Crafting":
                        craftingSources.Add(new ItemIdToSource {Collection = "Outfits", ItemId = idToUse, SourceId = (uint?)result.items?.First().id ?? 0, SourceDescription = source.text});
                        break;
                    case "Raid":
                    case "Trial":
                        dutySources.Add(new ItemIdToSource{Collection = "Outfits", ItemId = idToUse, SourceId = instanceIdToFinderId[((uint?)source.related_id)?? 0], SourceDescription = source.text });
                        Dev.Log($"Mount: {result.name}");
                        break;
                    case "V&C Dungeon":
                        if(source.related_id != null)
                        {
                            dutySources.Add(new ItemIdToSource{Collection = "Outfits", ItemId = idToUse, SourceId = instanceIdToFinderId[((uint?)source.related_id)?? 0], SourceDescription = source.text });
                        }
                        else
                        {
                            miscSources.Add(new ItemIdToSource{Collection = "Outfits",ItemId = idToUse, SourceId = 0, SourceDescription = source.text});
                        }
                        break;
                    case "Dungeon":
                        dutySources.Add(new ItemIdToSource{Collection = "Outfits", ItemId = idToUse, SourceId = (uint?)source.related_id ?? 0, SourceDescription = source.text });
                        break;
                    case "Achievement":
                        achievementSources.Add(new ItemIdToSource{Collection = "Outfits", ItemId = idToUse, SourceId =(uint?)source.related_id ?? 0, SourceDescription = source.text });
                        break;
                    default:
                        Dev.Log($"Missing Category: {source.type}");
                        miscSources.Add(new ItemIdToSource {Collection = "Outfits", ItemId = idToUse, SourceId = (uint?)source.related_id ?? 0, SourceDescription = source.text});
                        break;
                }
            }
        }
        CSVHandler.Write<ItemIdToSource>(mogStationSources, "ItemIdMogStationAlt.csv");
        CSVHandler.Write<ItemIdToSource>(dutySources, "ItemIdToDutyAlt.csv");
        CSVHandler.Write<ItemIdToSource>(eventSources, "ItemIdToEventAlt.csv");
        CSVHandler.Write<ItemIdToSource>(questSources, "ItemIdToQuestAlt.csv");
        CSVHandler.Write<ItemIdToSource>(craftingSources, "ItemIdToCraftedAlt.csv");
        CSVHandler.Write<ItemIdToSource>(miscSources, "ItemIdToMiscAlt.csv");
        return true;
    }

    public class XivRequestResult
    {
        public object query {get; set;}
        public int count {get; set;}
        public List<CollectibleResult> results {get; set;}
    }

    public class CollectibleResult
    {
        public int id { get; set; } // collection ID, not item ID
        public string name { get; set; } // item name
        public string description { get; set; } // unlock item description
        public int order { get; set; } // internal order
        public string patch { get; set; } // alternative source for patches
        public int? item_id { get; set; } // associated ItemAdapter ID
        public bool tradeable { get; set; } 
        public string owned { get; set; } // XivCollect item ownership
        public string image { get; set; } // XivCollect image link
        public string icon {get; set;} // XivCollect icon link
        public List<XivCollectSource> sources {get; set;} // Sources for item
        public List<ItemData>? items {get; set;} // items associated with collectible, mostly for outfits

        public class ItemData
        {
            public int id {get; set;}
            public string name {get; set;}
        }
    }

    public class XivCollectSource
    {
        public string type {get; set;} // XivCollect categories
        public string text {get; set;} // Individual source name
        public string? related_type {get; set;} // Game Category
        public int? related_id {get; set;} // Associated Game Object ID
    }
}