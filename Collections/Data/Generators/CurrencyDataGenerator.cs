namespace Collections;

public class CurrencyDataGenerator
{
    public Dictionary<uint, CollectibleSourceCategory> ItemIdToSourceCategory = new()
    {
        { 1, CollectibleSourceCategory.Gil }, // Gil
        { 20, CollectibleSourceCategory.CompanySeals }, // Storm Seal (designated company seals)
        { 25, CollectibleSourceCategory.PvP }, // Wolf Mark
        { 10310, CollectibleSourceCategory.Scrips }, // Blue gatherers scrip
        { 10311, CollectibleSourceCategory.Scrips }, // Red gatherers scrip
        { 17834, CollectibleSourceCategory.Scrips }, // Yellow gatherers scrip
        { 25200, CollectibleSourceCategory.Scrips }, // White gatherers scrip
        { 33914, CollectibleSourceCategory.Scrips }, // Purple gatherers scrip
        { 10308, CollectibleSourceCategory.Scrips }, // Blue gatherers scrip
        { 10309, CollectibleSourceCategory.Scrips }, // Red gatherers scrip
        { 17833, CollectibleSourceCategory.Scrips }, // Yellow gatherers scrip
        { 25199, CollectibleSourceCategory.Scrips }, // White gatherers scrip
        { 33913, CollectibleSourceCategory.Scrips }, // Purple gatherers scrip
        { 10307, CollectibleSourceCategory.HuntSeals }, // Centurio Seal
        { 27, CollectibleSourceCategory.HuntSeals }, // Allied Seal
        { 26533, CollectibleSourceCategory.HuntSeals }, // Sack of nuts
        { 29, CollectibleSourceCategory.MGP }, // MGP
        { 37549, CollectibleSourceCategory.IslandSanctuary }, // Seafarer's Cowrie
        { 15422, CollectibleSourceCategory.DeepDungeon }, // Gemorran potsherd
    };

    public CurrencyDataGenerator()
    {
        PopulateData();
    }

    private void PopulateData()
    {
        // Add Tomestones
        var TomestonesItemSheet = ExcelCache<TomestonesItem>.GetSheet();
        foreach (var tomestone in TomestonesItemSheet)
        {
            ItemIdToSourceCategory[tomestone.Item.Row] = CollectibleSourceCategory.Tomestones;
        }

        // Add Beast tribe currencies
        var beastTribeSheet = ExcelCache<BeastTribe>.GetSheet();
        foreach (var beastTribe in beastTribeSheet)
        {
            ItemIdToSourceCategory[beastTribe.CurrencyItem.Row] = CollectibleSourceCategory.BeastTribes;
        }

        // Add Gatherer/Crafter Scrips (Added manually - not really necassary, keeping it here for now)
        //var ItemSheet = ExcelCache<ItemAdapter>.GetSheet(Dalamud.ClientLanguage.English);
        //foreach (var item in ItemSheet)
        //{
        //    if (item.Name.ToString().EndsWith("Gatherers' Scrip") || item.Name.ToString().EndsWith("Crafters' Scrip"))
        //    {
        //        itemIdToSourceCategory[item.RowId] = CollectibleSourceCategory.Scrips;
        //    }
        //}
    }
}
