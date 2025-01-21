using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.Excel.Services;

namespace Collections;

public class InstancesDataGenerator : BaseDataGenerator<ContentFinderCondition>
{
    public Dictionary<uint, List<uint>> contentFinderConditionToItems = new(); // TODO deprecate

    private ExcelCache<ContentFinderCondition> contentFinderConditionList { get; set; }

    protected override void InitializeData()
    {
        contentFinderConditionList = ExcelCache<ContentFinderCondition>.GetSheet();
        setDungeonBossChestList();
        setDungeonBossDropList();
        setDungeonChestList();
        setDungeonDropList();
        setItemsToInstances();
    }

    private void setDungeonBossChestList()
    {
        var dungeonBossChestList = CsvLoader.LoadResource<DungeonBossChest>(CsvLoader.DungeonBossChestResourceName, out var failedLines, out var exceptions);
        foreach (var entry in dungeonBossChestList)
        {
            AddItemInstancePair(entry.ItemId, entry.ContentFinderConditionId);
        }
    }

    private void setDungeonBossDropList()
    {
        var dungeonBossDropList = CsvLoader.LoadResource<DungeonBossDrop>(CsvLoader.DungeonBossDropResourceName, out var failedLines, out var exceptions);
        foreach (var entry in dungeonBossDropList)
        {
            AddItemInstancePair(entry.ItemId, entry.ContentFinderConditionId);
        }
    }

    private void setDungeonChestList()
    {
        var dungeonChestItemList = CsvLoader.LoadResource<DungeonChestItem>(CsvLoader.DungeonChestItemResourceName, out var failedLines, out var exceptions);
        var dungeonChestList = CsvLoader.LoadResource<DungeonChest>(CsvLoader.DungeonChestResourceName, out var failedLines2, out var exceptions2);
        foreach (var dungeonChestItem in dungeonChestItemList)
        {
            var dungeonChest = dungeonChestList.Where(row => row.RowId == dungeonChestItem.ChestId).Single();
            AddItemInstancePair(dungeonChestItem.ItemId, dungeonChest.ContentFinderConditionId);
        }
    }

    private void setDungeonDropList()
    {
        var dungeonDropList = CsvLoader.LoadResource<DungeonDrop>(CsvLoader.DungeonDropItemResourceName, out var failedLines, out var exceptions);
        foreach (var entry in dungeonDropList)
        {
            AddItemInstancePair(entry.ItemId, entry.ContentFinderConditionId);
        }
    }

    private static readonly string FileName = "ItemIdToDuty.csv";
    private void setItemsToInstances()
    {
        var itemsToInstancesCSVList = CSVHandler.Load<ItemIdToSource>(FileName);
        foreach (var entry in itemsToInstancesCSVList)
        {
            if (entry.SourceId == 0)
                continue;

            AddItemInstancePair(entry.ItemId, entry.SourceId);
        }
    }

    private void AddItemInstancePair(uint itemId, uint contentFinderConditionId)
    {
        var contentFinderCondition = contentFinderConditionList.GetRow(contentFinderConditionId);

        AddEntry(itemId, contentFinderCondition.Value);

        if (!contentFinderConditionToItems.ContainsKey(contentFinderConditionId))
        {
            contentFinderConditionToItems[contentFinderConditionId] = new List<uint>();
        }
        if (!contentFinderConditionToItems[contentFinderConditionId].Contains(itemId))
            contentFinderConditionToItems[contentFinderConditionId].Add(itemId);
    }
}
