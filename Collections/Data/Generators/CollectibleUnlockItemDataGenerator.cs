using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace Collections;

public class CollectibleKeyDataGenerator
{
    public readonly Dictionary<uint, ItemAdapter> mountUnlockItem = new();
    public readonly Dictionary<uint, ItemAdapter> minionUnlockItem = new();

    public CollectibleKeyDataGenerator()
    {
        Dev.Start();
        PopulateData();
        Dev.Stop();
    }

    private void PopulateData()
    {
        var itemSheet = Excel.GetExcelSheet<ItemAdapter>();

        foreach (var item in itemSheet)
        {
            // This seems pretty consistent
            var type = item.ItemAction.Value?.Type;
            var collectibleData = item.ItemAction.Value?.Data;
            if (type == 1322)
            {
                mountUnlockItem[collectibleData[0]] = item;
            }
            else if (type == 853)
            {
                minionUnlockItem[collectibleData[0]] = item;

            }//2633
        }
    }
}
