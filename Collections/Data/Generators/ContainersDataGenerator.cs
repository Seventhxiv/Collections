using LuminaSupplemental.Excel.Model;
using System.Collections.Generic;

namespace Collections;

public class ContainersDataGenerator
{
    public Dictionary<uint, List<uint>> itemsToContainers = new();

    public ContainersDataGenerator()
    {
        Dev.StartStopwatch();
        PopulateData();
        Dev.EndStopwatch();
    }

    private void PopulateData()
    {
        var ItemSupplementList = CsvLoader.LoadResource<ItemSupplement>(CsvLoader.ItemSupplementResourceName, out var failedLines);
        foreach (var entry in ItemSupplementList)
        {
            if (!itemsToContainers.ContainsKey(entry.ItemId))
            {
                itemsToContainers[entry.ItemId] = new List<uint>();
            }
            itemsToContainers[entry.ItemId].Add(entry.SourceItemId);
        }
    }
}
