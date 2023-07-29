using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using LuminaSupplemental.Excel.Model;
using System.Collections.Generic;
using System.Linq;

namespace Collections;

public class InstancesDataParser
{
    public Dictionary<uint, List<ContentFinderCondition>> itemToContentFinderCondition = new();
    public Dictionary<uint, List<uint>> contentFinderConditionToItems = new();
    //public Dictionary<ContentFinderCondition, List<uint>> contentFinderConditionToItems = new();

    private ExcelSheet<ContentFinderCondition> contentFinderConditionList { get; init; }

    public InstancesDataParser()
    {
        Dev.Start();
        contentFinderConditionList = Excel.GetExcelSheet<ContentFinderCondition>();

        setDungeonBossChestList();
        setDungeonBossDropList();
        setDungeonChestList();
        setDungeonDropList();
        Dev.Stop();
    }

    private void setDungeonBossChestList()
    {
        var dungeonBossChestList = CsvLoader.LoadResource<DungeonBossChest>(CsvLoader.DungeonBossChestResourceName, out var failedLines);
        foreach (var entry in dungeonBossChestList)
        {
            AddItemInstancePair(entry.ItemId, entry.ContentFinderConditionId);
            //if (!itemToContentFinderCondition.ContainsKey(entry.ItemId))
            //{
            //    itemToContentFinderCondition[entry.ItemId] = new List<ContentFinderCondition>();
            //}
            //var contentFinderCondition = contentFinderConditionList.GetRow(entry.ContentFinderConditionId);
            //itemToContentFinderCondition[entry.ItemId].Add(contentFinderCondition);
        }
    }

    private void setDungeonBossDropList()
    {
        var dungeonBossDropList = CsvLoader.LoadResource<DungeonBossDrop>(CsvLoader.DungeonBossDropResourceName, out var failedLines);
        foreach (var entry in dungeonBossDropList)
        {
            AddItemInstancePair(entry.ItemId, entry.ContentFinderConditionId);
            //if (!itemToContentFinderCondition.ContainsKey(entry.ItemId))
            //{
            //    itemToContentFinderCondition[entry.ItemId] = new List<ContentFinderCondition>();
            //}
            //var contentFinderCondition = contentFinderConditionList.GetRow(entry.ContentFinderConditionId);
            //itemToContentFinderCondition[entry.ItemId].Add(contentFinderCondition);
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

            //var contentFinderCondition = contentFinderConditionList.GetRow(dungeonChest.ContentFinderConditionId);
            //if (!itemToContentFinderCondition.ContainsKey(dungeonChestItem.ItemId))
            //{
            //    itemToContentFinderCondition[dungeonChestItem.ItemId] = new List<ContentFinderCondition>();
            //}
            //itemToContentFinderCondition[dungeonChestItem.ItemId].Add(contentFinderCondition);
        }
    }

    private void setDungeonDropList()
    {
        var dungeonDropList = CsvLoader.LoadResource<DungeonDrop>(CsvLoader.DungeonDropItemResourceName, out var failedLines);
        foreach (var entry in dungeonDropList)
        {
            AddItemInstancePair(entry.ItemId, entry.ContentFinderConditionId);
            //if (!itemToContentFinderCondition.ContainsKey(entry.ItemId))
            //{
            //    itemToContentFinderCondition[entry.ItemId] = new List<ContentFinderCondition>();
            //}
            //var contentFinderCondition = contentFinderConditionList.GetRow(entry.ContentFinderConditionId);
            //itemToContentFinderCondition[entry.ItemId].Add(contentFinderCondition);
        }
    }

    private void AddItemInstancePair(uint itemId, uint contentFinderConditionId)
    {
        var contentFinderCondition = contentFinderConditionList.GetRow(contentFinderConditionId);

        if (!itemToContentFinderCondition.ContainsKey(itemId))
        {
            itemToContentFinderCondition[itemId] = new List<ContentFinderCondition>();
        }
        itemToContentFinderCondition[itemId].Add(contentFinderCondition);

        if (!contentFinderConditionToItems.ContainsKey(contentFinderConditionId))
        {
            contentFinderConditionToItems[contentFinderConditionId] = new List<uint>();
        }
        contentFinderConditionToItems[contentFinderConditionId].Add(itemId);

    }
}
