using LuminaSupplemental.Excel.Model;

namespace Collections;

public class MogStationDataGenerator
{
    public HashSet<uint> items = new();

    public MogStationDataGenerator()
    {
        PopulateData();
    }

    private static readonly string FileName = "ItemIdToQuest.csv";
    private void PopulateData()
    {
        // Based on LuminaSupplemental
        var StoreItemList = CsvLoader.LoadResource<StoreItem>(CsvLoader.StoreItemResourceName, out var failedLines);
        foreach (var entry in StoreItemList)
        {
            items.Add(entry.ItemId);
        }

        // Based on resource data
        var resourceData = CSVHandler.Load<ItemIdToSource>(FileName);
        items.UnionWith(resourceData.Select(e => e.ItemId));

        // FittingShopCategoryItem sheet
        var FittingShopCategoryItemSheet = ExcelCache<FittingShopCategoryItem>.GetSheet()!;

        foreach (var FittingShopCategoryItem in FittingShopCategoryItemSheet)
        {
            var itemId = Convert.ToUInt32(FittingShopCategoryItem.Unknown0);

            if (!items.Contains(itemId))
            {
                items.Add(itemId);
            }
        }

        // FittingShopItemSet sheet
        var FittingShopItemSetSheet = ExcelCache<FittingShopItemSet>.GetSheet()!;

        foreach (var fittingShopItemSet in FittingShopItemSetSheet)
        {
            AddIfNotZero((uint)fittingShopItemSet.Unknown0);
            AddIfNotZero((uint)fittingShopItemSet.Unknown1);
            AddIfNotZero((uint)fittingShopItemSet.Unknown2);
            AddIfNotZero((uint)fittingShopItemSet.Unknown3);
            AddIfNotZero((uint)fittingShopItemSet.Unknown4);
            AddIfNotZero((uint)fittingShopItemSet.Unknown5);
        }

        // Override items that shouldn't be considered mog station
        foreach(var itemId in DataOverrides.IgnoreMogStationId)
        {
            if (items.Contains(itemId))
            {
                items.Remove(itemId);
            }
        }
    }

    private void AddIfNotZero(uint itemId)
    {
        if (itemId != 0)
        {
            if (!items.Contains(itemId))
            {
                items.Add(itemId);
            }
        }
    }
}
