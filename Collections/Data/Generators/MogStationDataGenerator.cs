using Lumina.Excel.GeneratedSheets;
using LuminaSupplemental.Excel.Model;
using System;
using System.Collections.Generic;

namespace Collections;

public class MogStationDataGenerator
{
    public List<uint> items = new();

    public MogStationDataGenerator()
    {
        Dev.StartStopwatch();
        PopulateData();
        Dev.EndStopwatch();
    }

    private void PopulateData()
    {
        // CSV based
        var StoreItemList = CsvLoader.LoadResource<StoreItem>(CsvLoader.StoreItemResourceName, out var failedLines);
        foreach (var entry in StoreItemList)
        {
            items.Add(entry.ItemId);
        }

        // FittingShopCategoryItem sheet
        var FittingShopCategoryItemSheet = Excel.GetExcelSheet<FittingShopCategoryItem>()!;

        foreach (var FittingShopCategoryItem in FittingShopCategoryItemSheet)
        {
            var itemId = Convert.ToUInt32(FittingShopCategoryItem.Unknown0);

            if (!items.Contains(itemId))
            {
                items.Add(itemId);
            }
        }

        // FittingShopItemSet sheet
        var FittingShopItemSetSheet = Excel.GetExcelSheet<FittingShopItemSet>()!;

        foreach (var fittingShopItemSet in FittingShopItemSetSheet)
        {
            AddIfNotZero((uint)fittingShopItemSet.Unknown0);
            AddIfNotZero((uint)fittingShopItemSet.Unknown1);
            AddIfNotZero((uint)fittingShopItemSet.Unknown2);
            AddIfNotZero((uint)fittingShopItemSet.Unknown3);
            AddIfNotZero((uint)fittingShopItemSet.Unknown4);
            AddIfNotZero((uint)fittingShopItemSet.Unknown5);
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
