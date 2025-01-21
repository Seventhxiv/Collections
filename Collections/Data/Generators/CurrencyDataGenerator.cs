namespace Collections;

public class CurrencyDataGenerator
{
    public Dictionary<uint, SourceCategory> ItemIdToSourceCategory = new()
    {
        { 1, SourceCategory.Gil }, // Gil
        { 20, SourceCategory.CompanySeals }, // Storm Seal (designated company seals)
        { 25, SourceCategory.PvP }, // Wolf Mark
        { 10310, SourceCategory.Scrips }, // Blue gatherers scrip
        { 10311, SourceCategory.Scrips }, // Red gatherers scrip
        { 17834, SourceCategory.Scrips }, // Yellow gatherers scrip
        { 25200, SourceCategory.Scrips }, // White gatherers scrip
        { 33914, SourceCategory.Scrips }, // Purple gatherers scrip
        { 10308, SourceCategory.Scrips }, // Blue gatherers scrip
        { 10309, SourceCategory.Scrips }, // Red gatherers scrip
        { 17833, SourceCategory.Scrips }, // Yellow gatherers scrip
        { 25199, SourceCategory.Scrips }, // White gatherers scrip
        { 33913, SourceCategory.Scrips }, // Purple gatherers scrip
        { 10307, SourceCategory.HuntSeals }, // Centurio Seal
        { 27, SourceCategory.HuntSeals }, // Allied Seal
        { 26533, SourceCategory.HuntSeals }, // Sack of nuts
        { 29, SourceCategory.MGP }, // MGP
        { 37549, SourceCategory.IslandSanctuary }, // Seafarer's Cowrie
        { 15422, SourceCategory.DeepDungeon }, // Gemorran potsherd
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
            ItemIdToSourceCategory[tomestone.Item.RowId] = SourceCategory.Tomestones;
        }

        // Add Beast tribe currencies
        var beastTribeSheet = ExcelCache<BeastTribe>.GetSheet();
        foreach (var beastTribe in beastTribeSheet)
        {
            ItemIdToSourceCategory[beastTribe.CurrencyItem.RowId] = SourceCategory.BeastTribes;
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
