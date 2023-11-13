namespace Collections;

public class CurrencyDataGenerator
{
    private Dictionary<uint, CollectibleSourceCategory> itemIdToCollectionMethodType = new()
    {
        //{ 25, CollectibleSourceType.WolfMarks },
        //{ 25, CollectibleSourceType.PvP },
    };
    private Dictionary<string, CollectibleSourceCategory> itemNameToCollectionMethodType = new(StringComparer.InvariantCultureIgnoreCase)
    {
        { "Gil", CollectibleSourceCategory.Gil },
        { "Storm Seal", CollectibleSourceCategory.CompanySeals }, // designated company seals
        { "Wolf Mark", CollectibleSourceCategory.PvP },
        { "Gatherers' Scrip", CollectibleSourceCategory.Scrips },
        { "Crafters' Scrip", CollectibleSourceCategory.Scrips },
        { "Centurio Seal", CollectibleSourceCategory.HuntSeals },
        { "Allied Seal", CollectibleSourceCategory.HuntSeals },
        { "Sack Of Nuts", CollectibleSourceCategory.HuntSeals },
        { "MGP", CollectibleSourceCategory.MGP },
        { "Seafarer's Cowrie", CollectibleSourceCategory.IslandSanctuary },
        { "Gelmorran potsherd", CollectibleSourceCategory.DeepDungeon },
    };

    public CurrencyDataGenerator()
    {
        Dev.StartStopwatch();
        PopulateData();
        Dev.EndStopwatch();
    }

    public CollectibleSourceCategory CurrencyItemCollectionMethodType(Item item)
    {
        if (itemIdToCollectionMethodType.ContainsKey(item.RowId))
        {
            return itemIdToCollectionMethodType[item.RowId];
        }
        return CollectibleSourceCategory.Other;
    }

    private void PopulateData()
    {
        // Looks up tomestone sheet
        var TomestonesItemSheet = Excel.GetExcelSheet<TomestonesItem>();
        foreach (var tomestone in TomestonesItemSheet)
        {
            itemIdToCollectionMethodType[tomestone.Item.Row] = CollectibleSourceCategory.Tomestones;
        }

        // Populate based on manual data in itemNameToContentType
        var ItemSheet = Excel.GetExcelSheet<Item>();
        foreach (var item in ItemSheet)
        {
            if (itemNameToCollectionMethodType.ContainsKey(item.Name))
            {
                var contentType = itemNameToCollectionMethodType[item.Name];
                itemIdToCollectionMethodType[item.RowId] = contentType;
            }
            else if (item.Name.ToString().EndsWith("Gatherers' Scrip") || item.Name.ToString().EndsWith("Crafters' Scrip"))
            {
                itemIdToCollectionMethodType[item.RowId] = CollectibleSourceCategory.Scrips;
            }
        }
    }
}
