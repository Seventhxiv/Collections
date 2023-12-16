using LuminaSupplemental.Excel.Model;

namespace Collections;

public class MogStationDataGenerator : BaseDataGenerator<uint>
{
    private static readonly string FileName = "ItemIdToQuest.csv";
    protected override void InitializeData()
    {
        // Based on LuminaSupplemental
        var StoreItemList = CsvLoader.LoadResource<StoreItem>(CsvLoader.StoreItemResourceName, out var failedLines);
        foreach (var entry in StoreItemList)
        {
            AddEntry(entry.ItemId, 0);
        }

        // Based on resource data
        var resourceData = CSVHandler.Load<ItemIdToSource>(FileName);
        foreach (var entry in resourceData)
        {
            AddEntry(entry.ItemId, 0);
        }

        // FittingShopCategoryItem sheet
        var FittingShopCategoryItemSheet = ExcelCache<FittingShopCategoryItem>.GetSheet()!;

        foreach (var FittingShopCategoryItem in FittingShopCategoryItemSheet)
        {
            var itemId = Convert.ToUInt32(FittingShopCategoryItem.Unknown0);
            AddEntry(itemId, 0);
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
            RemoveEntry(itemId, 0);
        }
    }

    private void AddIfNotZero(uint itemId)
    {
        if (itemId != 0)
        {
            AddEntry(itemId, 0);
        }
    }
}
