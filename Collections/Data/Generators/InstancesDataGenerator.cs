using System.IO;
using LuminaSupplemental.Excel.Model;

namespace Collections;

public class InstancesDataGenerator
{
    public Dictionary<uint, List<ContentFinderCondition>> itemToContentFinderCondition = new();
    public Dictionary<uint, List<uint>> contentFinderConditionToItems = new();

    private ExcelCache<ContentFinderCondition> contentFinderConditionList { get; init; }

    public InstancesDataGenerator()
    {
        //Dev.Start();
        contentFinderConditionList = ExcelCache<ContentFinderCondition>.GetSheet();
        setDungeonBossChestList();
        setDungeonBossDropList();
        setDungeonChestList();
        setDungeonDropList();
        setItemsToInstances();
        //Dev.Stop();
    }

    private void setDungeonBossChestList()
    {
        var dungeonBossChestList = CsvLoader.LoadResource<DungeonBossChest>(CsvLoader.DungeonBossChestResourceName, out var failedLines);
        foreach (var entry in dungeonBossChestList)
        {
            AddItemInstancePair(entry.ItemId, entry.ContentFinderConditionId);
        }
    }

    private void setDungeonBossDropList()
    {
        var dungeonBossDropList = CsvLoader.LoadResource<DungeonBossDrop>(CsvLoader.DungeonBossDropResourceName, out var failedLines);
        foreach (var entry in dungeonBossDropList)
        {
            AddItemInstancePair(entry.ItemId, entry.ContentFinderConditionId);
        }
    }

    private void setDungeonChestList()
    {
        var dungeonChestItemList = CsvLoader.LoadResource<DungeonChestItem>(CsvLoader.DungeonChestItemResourceName, out var failedLines);
        var dungeonChestList = CsvLoader.LoadResource<DungeonChest>(CsvLoader.DungeonChestResourceName, out var failedLines2);
        foreach (var dungeonChestItem in dungeonChestItemList)
        {
            var dungeonChest = dungeonChestList.Where(row => row.RowId == dungeonChestItem.ChestId).Single();
            AddItemInstancePair(dungeonChestItem.ItemId, dungeonChest.ContentFinderConditionId);
        }
    }

    private void setDungeonDropList()
    {
        var dungeonDropList = CsvLoader.LoadResource<DungeonDrop>(CsvLoader.DungeonDropItemResourceName, out var failedLines);
        foreach (var entry in dungeonDropList)
        {
            AddItemInstancePair(entry.ItemId, entry.ContentFinderConditionId);
        }
    }

    private static readonly string ItemsToInstancesPath = Path.Combine(Services.PluginInterface.AssemblyLocation.Directory?.FullName!, @"Data\Resources\itemsToInstances.csv");
    private void setItemsToInstances()
    {
        var itemsToInstancesCSVList = Helpers.LoadCSV<ItemToInstance>(ItemsToInstancesPath);
        foreach (var entry in itemsToInstancesCSVList)
        {
            var itemId = entry.itemId;
            foreach (var dutyId in entry.dutyIds.Split(";"))
            {
                AddItemInstancePair(itemId, Convert.ToUInt32(dutyId));
            }
        }
    }

    private class ItemToInstance
    {
        public uint itemId { get; set; }
        public string Events { get; set; }
        public bool isMogstation { get; set; }
        public string dutyIds { get; set; }
    }

    private void AddItemInstancePair(uint itemId, uint contentFinderConditionId)
    {
        var contentFinderCondition = contentFinderConditionList.GetRow(contentFinderConditionId);

        if (!itemToContentFinderCondition.ContainsKey(itemId))
        {
            itemToContentFinderCondition[itemId] = new List<ContentFinderCondition>();
        }

        if (!itemToContentFinderCondition[itemId].Contains(contentFinderCondition))
            itemToContentFinderCondition[itemId].Add(contentFinderCondition);

        if (!contentFinderConditionToItems.ContainsKey(contentFinderConditionId))
        {
            contentFinderConditionToItems[contentFinderConditionId] = new List<uint>();
        }
        if (!contentFinderConditionToItems[contentFinderConditionId].Contains(itemId))
            contentFinderConditionToItems[contentFinderConditionId].Add(itemId);
    }
}
