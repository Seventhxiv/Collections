namespace Collections;

public class ShopsDataGenerator
{
    public Dictionary<uint, List<ShopEntry>> itemToShopEntry = new();

    private Dictionary<uint, uint> NpcDataToNpcBase { get; init; } = new();

    private ExcelCache<ItemAdapter> ItemSheet { get; init; }
    private ExcelCache<ENpcBase> ENpcBaseSheet { get; init; }
    private ExcelCache<SpecialShopAdapter> SpecialShopEntitySheet { get; init; }
    private ExcelCache<CustomTalk> CustomTalkSheet { get; init; }
    private ExcelCache<CustomTalkNestHandlers> CustomTalkNestHandlersSheet { get; init; }
    private ExcelCache<InclusionShop> InclusionShopSheet { get; init; }
    private ExcelCache<InclusionShopSeries> InclusionShopSeriesSheet { get; init; }
    private ExcelCache<TopicSelect> TopicSelectSheet { get; init; }
    private ExcelCache<PreHandler> PreHandlerSheet { get; init; }

    public ShopsDataGenerator()
    {
        //Dev.Start();
        ItemSheet = ExcelCache<ItemAdapter>.GetSheet();
        ENpcBaseSheet = ExcelCache<ENpcBase>.GetSheet();
        SpecialShopEntitySheet = ExcelCache<SpecialShopAdapter>.GetSheet();
        CustomTalkSheet = ExcelCache<CustomTalk>.GetSheet();
        CustomTalkNestHandlersSheet = ExcelCache<CustomTalkNestHandlers>.GetSheet();
        InclusionShopSheet = ExcelCache<InclusionShop>.GetSheet();
        InclusionShopSeriesSheet = ExcelCache<InclusionShopSeries>.GetSheet();
        TopicSelectSheet = ExcelCache<TopicSelect>.GetSheet();
        PreHandlerSheet = ExcelCache<PreHandler>.GetSheet();

        PopulateNpcDataToNpcBase();
        PopulateGilShop();
        PopulateGCShop();
        PopulateSpecialShop();
        //Dev.Stop();
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
        Dev.Start();

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

        // Inject manual data
        foreach (var (NpcBaseId, gilShopRowIds) in DataOverrides.GilShopToNpcBase)
        {
            foreach (var gilShopRowId in gilShopRowIds)
            {
                NpcDataToNpcBase.TryAdd(gilShopRowId, NpcBaseId);
            }
        }

        foreach (var (specialShopRowId, NpcBaseId) in DataOverrides.SpecialShopToNpcBase)
        {
            NpcDataToNpcBase.TryAdd(specialShopRowId, NpcBaseId);
        }
        Dev.Stop();
    }

    private uint? GetNpcBaseFromNpcData(uint npcDataId)
    {
        if (NpcDataToNpcBase.ContainsKey(npcDataId))
        {
            // Map Journeyman Salvager to Calamity Salvager (locateable)
            var npcBaseId = NpcDataToNpcBase[npcDataId];
            if (npcBaseId == 1025924)
                return 1006004;

            return npcBaseId;
        }
        return null;
    }

    private static bool MatchEventHandlerType(uint data, EventHandlerType type)
    {
        return ((data >> 16) & (uint)type) == (uint)type;
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
        Dev.Start();
        var gilShopSheet = ExcelCache<GilShop>.GetSheet()!;
        var gilShopItemSheet = ExcelCache<GilShopItem>.GetSheet()!;
        var gilItem = ItemSheet.GetRow((uint)Currency.Gil);
        foreach (var gilShop in gilShopSheet)
        {
            if (DataOverrides.IgnoreGilShopId.Contains(gilShop.RowId))
            {
                continue;
            }

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
                    uint? ENpcBaseId = null;
                    if (NpcDataToNpcBase.ContainsKey(ENpcDataId))
                    {
                        ENpcBaseId = GetNpcBaseFromNpcData(ENpcDataId);
                    }
                    var costList = new List<(ItemAdapter Item, int Amount)> { (gilItem, (int)item.PriceMid) };
                    var shopEntry = new ShopEntry(costList, ENpcBaseId, gilShop.RowId);
                    itemToShopEntry[item.RowId].Add(shopEntry);
                }
                catch
                {
                    continue;
                }
            }
        }
        Dev.Stop();
    }

    private void PopulateGCShop()
    {
        Dev.Start();
        var GCShopSheet = ExcelCache<GCShop>.GetSheet()!; // 4 rows
        var GCScripShopCategorySheet = ExcelCache<GCScripShopCategory>.GetSheet()!; // 31 rows
        var GCScripShopItemSheet = ExcelCache<GCScripShopItem>.GetSheet()!; // 36.x
        var GCItem = ItemSheet.GetRow((uint)Currency.CompanySeals);

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
                        var ENpcBaseId = GetNpcBaseFromNpcData(ENpcDataId);
                        var costList = new List<(ItemAdapter Item, int Amount)> { (GCItem, (int)item.PriceMid) };
                        var shopEntry = new ShopEntry(costList, ENpcBaseId, gcShop.RowId);
                        itemToShopEntry[item.RowId].Add(shopEntry);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            }
        }
        Dev.Stop();
    }

    private void PopulateSpecialShop()
    {
        Dev.Start();

        foreach (var specialShop in SpecialShopEntitySheet)
        {
            if (DataOverrides.IgnoreSpecialShopId.Contains(specialShop.RowId))
            {
                continue;
            }

            uint? ENpcBaseId = null;
            if (NpcDataToNpcBase.ContainsKey(specialShop.RowId))
            {
                ENpcBaseId = GetNpcBaseFromNpcData(specialShop.RowId);
            }

            foreach (var entry in specialShop.Entries)
            {
                var costList = new List<(ItemAdapter Item, int Amount)>();

                foreach (var cost in entry.Cost)
                {
                    if (cost.Item.Row == 0)
                    {
                        continue;
                    }

                    costList.Add((cost.Item.Value, (int)cost.Count));
                }
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
                    var shopEntry = new ShopEntry(costList, ENpcBaseId, specialShop.RowId);
                    itemToShopEntry[itemId].Add(shopEntry);
                }
            }
        }
        Dev.Stop();
    }
}
