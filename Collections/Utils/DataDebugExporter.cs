using System.IO;
using System.Text;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

namespace Collections;

public class DataDebugExporter
{
    public static void ShopsDataDebugger()
    {
        var data = new List<ShopData>();
        foreach (var collectible in Services.DataProvider.GetCollection<GlamourCollectible>())
        {
            var sources = collectible.CollectibleKey.CollectibleSources;
            var badObservedCost = new List<uint>();
            var goodObservedCost = new List<uint>();
            foreach (var source in sources)
            {
                if (source is not ShopCollectibleSource)
                {
                    continue;
                }

                var shopSource = (ShopCollectibleSource)source;

                var name = source.GetName();
                var locatable = shopSource.GetIslocatable();
                var npcResident = shopSource.ENpcResident;
                var NpcId = npcResident is not null ? npcResident.RowId : 0;
                var validNpc = npcResident is not null;
                var npcName = npcResident is not null ? npcResident.Singular.ToString() : "";
                var item = collectible.CollectibleKey.item;
                var shopId = shopSource.ShopId;

                var costIndicator = shopSource.costItems.First().collectibleKey.item.RowId;
                if (validNpc && locatable)
                    goodObservedCost.Add(costIndicator);
                else
                    badObservedCost.Add(costIndicator);
            }

            if (badObservedCost.Any() && !badObservedCost.Intersect(goodObservedCost).Any())
            {
                foreach (var source in sources)
                {
                    if (source is not ShopCollectibleSource)
                    {
                        continue;
                    }

                    var shopSource = (ShopCollectibleSource)source;

                    var name = source.GetName();
                    var locatable = shopSource.GetIslocatable();
                    var npcResident = shopSource.ENpcResident;
                    var NpcId = npcResident is not null ? npcResident.RowId : 0;
                    var validNpc = npcResident is not null;
                    var npcName = npcResident is not null ? npcResident.Singular.ToString() : "";
                    var item = collectible.CollectibleKey.item;
                    var shopId = shopSource.ShopId;
                        var dataObj = new ShopData()
                        {
                            itemId = item.RowId,
                            itemName = item.Name,
                            npcId = NpcId,
                            npcName = npcName,
                            validNpc = validNpc,
                            locatableNpc = locatable,
                            shopId = shopId,
                            costDescription = name,
                        };
                        data.Add(dataObj);
                }
            }
        }


        SaveToCSV(data, "DataDebug.csv");
    }

    public class ShopData
    {
        public uint npcId { get; set; }
        public string npcName { get; set; }
        public bool validNpc { get; set; }
        public bool locatableNpc { get; set; }
        public uint shopId { get; set; }
        public uint itemId { get; set; }
        public string itemName { get; set; }
        public string costDescription { get; set; }
    }

    public static void SaveToCSV<T>(List<T> data, string path = "")
    {
        var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ",",
            Encoding = Encoding.UTF8
        };

        using (var writer = new StreamWriter(path))
        using (var csvWriter = new CsvWriter(writer, csvConfig))
        {
            csvWriter.WriteRecords(data);
        }
    }
}

