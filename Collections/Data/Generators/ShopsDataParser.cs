using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Collections;

public class ShopsDataParser
{
    public Dictionary<uint, List<ShopEntry>> itemToShopEntry = new();

    private Dictionary<uint, uint> NpcDataToNpcBase { get; init; } = new();

    private ExcelSheet<Item> ItemSheet { get; init; }
    private ExcelSheet<ENpcBase> ENpcBaseSheet { get; init; }
    private ExcelSheet<SpecialShopEntity> SpecialShopEntitySheet { get; init; }
    private ExcelSheet<CustomTalk> CustomTalkSheet { get; init; }
    private ExcelSheet<CustomTalkNestHandlers> CustomTalkNestHandlersSheet { get; init; }
    private ExcelSheet<InclusionShop> InclusionShopSheet { get; init; }
    private ExcelSheet<InclusionShopSeries> InclusionShopSeriesSheet { get; init; }
    private ExcelSheet<TopicSelect> TopicSelectSheet { get; init; }
    private ExcelSheet<PreHandler> PreHandlerSheet { get; init; }

    public ShopsDataParser()
    {
        ItemSheet = Excel.GetExcelSheet<Item>();
        ENpcBaseSheet = Excel.GetExcelSheet<ENpcBase>();
        SpecialShopEntitySheet = Excel.GetExcelSheet<SpecialShopEntity>();
        CustomTalkSheet = Excel.GetExcelSheet<CustomTalk>();
        CustomTalkNestHandlersSheet = Excel.GetExcelSheet<CustomTalkNestHandlers>();
        InclusionShopSheet = Excel.GetExcelSheet<InclusionShop>();
        InclusionShopSeriesSheet = Excel.GetExcelSheet<InclusionShopSeries>();
        TopicSelectSheet = Excel.GetExcelSheet<TopicSelect>();
        PreHandlerSheet = Excel.GetExcelSheet<PreHandler>();

        PopulateNpcDataToNpcBase();
        PopulateGilShop();
        PopulateGCShop();
        PopulateSpecialShop();
    }

    internal enum EventHandlerType : uint
    {
        GilShop = 0x0004,
        CustomTalk = 0x000B,
        GcShop = 0x0016,
        SpecialShop = 0x001B,
        FcShop = 0x002A,
    }

    private void PopulateNpcDataToNpcBase()
    {
        Dev.StartStopwatch();

        var FirstSpecialShopId = SpecialShopEntitySheet.First().RowId;
        var LastSpecialShopId = SpecialShopEntitySheet.Last().RowId;

        foreach (var ENpcBase in ENpcBaseSheet)
        {
            foreach (var ENpcData in ENpcBase.ENpcData)
            {
                NpcDataToNpcBase[ENpcData] = ENpcBase.RowId;

                var npcData = ENpcData;
                if (npcData == 0)
                {
                    continue;
                }

                // CustomTalk
                if (MatchEventHandlerType(npcData, EventHandlerType.CustomTalk))
                {
                    var customTalk = CustomTalkSheet.GetRow(npcData);
                    if (customTalk == null)
                    {
                        continue;
                    }


                    // CustomTalk - SpecialLinks
                    if (customTalk.SpecialLinks != 0)
                    {
                        try
                        {
                            for (uint index = 0; index <= 30; index++)
                            {
                                var customTalkNestHandler = CustomTalkNestHandlersSheet.GetRow(customTalk.SpecialLinks, index);
                                if (customTalkNestHandler != null)
                                {
                                    var specialShop = SpecialShopEntitySheet.GetRow(customTalkNestHandler.NestHandler);
                                    if (specialShop != null)
                                    {
                                        NpcDataToNpcBase[specialShop.RowId] = ENpcBase.RowId;
                                    }
                                }
                            }
                        }
                        catch { }
                    }

                    // CustomTalk - ScriptArg
                    foreach (var arg in customTalk.ScriptArg)
                    {
                        if (arg < FirstSpecialShopId || arg > LastSpecialShopId)
                        {
                            continue;
                        }
                        var specialShop = SpecialShopEntitySheet.GetRow(arg);
                        if (specialShop != null)
                        {
                            NpcDataToNpcBase[specialShop.RowId] = ENpcBase.RowId;
                        }
                    }
                }

                // InclusionShops
                var inclusionShop = InclusionShopSheet.GetRow(npcData);
                addInclusionShop(inclusionShop, ENpcBase.RowId);

                // PreHandler
                var preHandler = PreHandlerSheet.GetRow(npcData);
                addPreHandler(preHandler, ENpcBase.RowId);

                // TopicSelect
                var topicSelect = TopicSelectSheet.GetRow(npcData);
                if (topicSelect != null)
                {
                    foreach (var data in topicSelect.Shop)
                    {
                        if (data == 0)
                        {
                            continue;
                        }

                        if (MatchEventHandlerType(data, EventHandlerType.SpecialShop))
                        {
                            var specialShop = SpecialShopEntitySheet.GetRow(data);
                            if (specialShop != null)
                            {
                                NpcDataToNpcBase[specialShop.RowId] = ENpcBase.RowId;
                            }
                            continue;
                        }

                        // TopicSelect -> PreHandler
                        preHandler = PreHandlerSheet.GetRow(data);
                        addPreHandler(preHandler, ENpcBase.RowId);
                    }
                }
            }
        }
        Dev.EndStopwatch();
    }
    private static bool MatchEventHandlerType(uint data, EventHandlerType type)
    {
        return (data >> 16 & (uint)type) == (uint)type;
    }

    private void addPreHandler(PreHandler preHandler, uint ENpcBaseId)
    {
        if (preHandler == null)
        {
            return;
        }

        var target = preHandler.Target;
        if (target == 0)
        {
            return;
        }

        if (MatchEventHandlerType(target, EventHandlerType.SpecialShop))
        {
            var specialShop = SpecialShopEntitySheet.GetRow(target);
            if (specialShop != null)
            {
                NpcDataToNpcBase[specialShop.RowId] = ENpcBaseId;
            }
            return;
        }

        var inclusionShop = InclusionShopSheet.GetRow(target);
        addInclusionShop(inclusionShop, ENpcBaseId);
    }

    private void addInclusionShop(InclusionShop inclusionShop, uint ENpcBaseId)
    {
        if (inclusionShop == null)
        {
            return;
        }

        foreach (var category in inclusionShop.Category)
        {
            if (category.Value.RowId == 0)
            {
                continue;
            }

            for (uint i = 0; ; i++)
            {
                try
                {
                    var series = InclusionShopSeriesSheet.GetRow(category.Value.InclusionShopSeries.Row, i);
                    if (series == null)
                    {
                        break;
                    }

                    var specialShop = SpecialShopEntitySheet.GetRow(series.SpecialShop.Row);
                    if (specialShop != null)
                    {
                        NpcDataToNpcBase[specialShop.RowId] = ENpcBaseId;
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }
        }
    }

    private void PopulateGilShop()
    {
        Dev.StartStopwatch();
        var gilShopSheet = Excel.GetExcelSheet<GilShop>()!;
        var gilShopItemSheet = Excel.GetExcelSheet<GilShopItem>()!;
        var gilItem = ItemSheet.GetRow((uint)Currencies.Gil);
        foreach (var gilShop in gilShopSheet)
        {
            for (uint i = 0; i < 100; i++)
            {
                try
                {
                    var gilShopItem = gilShopItemSheet.GetRow(gilShop.RowId, i);
                    var item = gilShopItem?.Item.Value;
                    if (item == null)
                    {
                        break;
                    }

                    if (!itemToShopEntry.ContainsKey(item.RowId))
                    {
                        itemToShopEntry[item.RowId] = new List<ShopEntry>();
                    }
                    var ENpcDataId = gilShop.RowId;
                    var ENpcBaseId = NpcDataToNpcBase[ENpcDataId];
                    var costList = new List<(Item Item, int Amount)> { (gilItem, (int)item.PriceMid) };
                    var shopEntry = new ShopEntry(costList, ENpcBaseId);
                    //var shopEntry = new ShopEntry()
                    //{
                    //    Cost = new List<ShopEntryCost>() {
                    //        new ShopEntryCost()
                    //        {
                    //        CurrencyType = CollectibleSourceType.Gil, Amount = (int)item.PriceMid
                    //        }
                    //    },
                    //    ENpcBaseId = ENpcBaseId
                    //};
                    itemToShopEntry[item.RowId].Add(shopEntry);
                }
                catch
                {
                    break;
                }
            }
        }
        Dev.EndStopwatch();
    }

    private void PopulateGCShop()
    {
        Dev.StartStopwatch();
        var GCShopSheet = Excel.GetExcelSheet<GCShop>()!; // 4 rows
        var GCScripShopCategorySheet = Excel.GetExcelSheet<GCScripShopCategory>()!; // 31 rows
        var GCScripShopItemSheet = Excel.GetExcelSheet<GCScripShopItem>()!; // 36.x
        var GCItem = ItemSheet.GetRow((uint)Currencies.CompanySeals);

        foreach (var gcShop in GCShopSheet)
        {
            var gcShopCategories = GCScripShopCategorySheet.Where(i => i.GrandCompany.Row == gcShop.GrandCompany.Row).ToList();
            if (gcShopCategories.Count == 0)
            {
                return;
            }

            foreach (var category in gcShopCategories)
            {
                for (var i = 0u; ; i++)
                {
                    try
                    {
                        var GCScripShopItem = GCScripShopItemSheet.GetRow(category.RowId, i);
                        if (GCScripShopItem == null)
                        {
                            break;
                        }

                        var item = GCScripShopItem.Item.Value;
                        if (item == null)
                        {
                            break;
                        }


                        if (!itemToShopEntry.ContainsKey(item.RowId))
                        {
                            itemToShopEntry[item.RowId] = new List<ShopEntry>();
                        }
                        var ENpcDataId = gcShop.RowId;
                        var ENpcBaseId = NpcDataToNpcBase[ENpcDataId];
                        var costList = new List<(Item Item, int Amount)> { (GCItem, (int)item.PriceMid) };
                        var shopEntry = new ShopEntry(costList, ENpcBaseId);
                        //var shopEntry = new ShopEntry()
                        //{
                        //    Cost = new List<ShopEntryCost>() {new ShopEntryCost()
                        //     {
                        //        CurrencyType = CollectibleSourceType.CompanySeals, Amount = (int)GCScripShopItem.CostGCSeals
                        //     }
                        //    },
                        //    ENpcBaseId = ENpcBaseId
                        //};
                        itemToShopEntry[item.RowId].Add(shopEntry);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            }
        }
        Dev.EndStopwatch();
    }

    private void PopulateSpecialShop()
    {
        Dev.StartStopwatch();

        foreach (var specialShop in SpecialShopEntitySheet)
        {
            uint? ENpcBaseId = null;
            if (NpcDataToNpcBase.ContainsKey(specialShop.RowId))
            {
                ENpcBaseId = NpcDataToNpcBase[specialShop.RowId];
            }

            foreach (var entry in specialShop.Entries)
            {
                //var CostShopEntryList = new List<ShopEntryCost>();
                var costList = new List<(Item Item, int Amount)>();

                foreach (var cost in entry.Cost)
                {
                    if (cost.Item.Row == 0)
                    {
                        continue;
                    }

                    costList.Add((cost.Item.Value, (int)cost.Count));
                    //var currencyType = PluginServices.ContentType.getSourceType(cost.Item.Value);
                    //var costShopEntry = new ShopEntryCost()
                    //{
                    //    CurrencyType = currencyType, Amount = (int)cost.Count, Item = cost.Item.Value
                    //};

                    //CostShopEntryList.Add(costShopEntry);
                }
                //costStr += "Cost: " + specialShop.RowId + cost.Item.Value.Name + "x" + cost.Count.ToString() + ", ";

                foreach (var result in entry.Result)
                {
                    var itemId = result.Item.Value.RowId;
                    if (itemId == 0)
                    {
                        break;
                    }

                    if (!itemToShopEntry.ContainsKey(itemId))
                    {
                        itemToShopEntry[itemId] = new List<ShopEntry>();
                    }
                    var shopEntry = new ShopEntry(costList, ENpcBaseId);
                    //var shopEntry = new ShopEntry()
                    //{
                    //    Cost = CostShopEntryList,
                    //    ENpcBaseId = ENpcBaseId
                    //};
                    itemToShopEntry[itemId].Add(shopEntry);
                }
            }
        }
        Dev.EndStopwatch();
    }
}
