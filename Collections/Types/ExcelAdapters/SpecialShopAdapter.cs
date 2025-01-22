namespace Collections;

[Sheet("SpecialShop")]
public unsafe struct SpecialShopAdapter(ExcelPage page, uint offset, uint row) : IExcelRow<SpecialShopAdapter>
{
    static SpecialShopAdapter IExcelRow<SpecialShopAdapter>.Create(ExcelPage page, uint offset, uint row)
    {
        var obj = new SpecialShopAdapter(page, offset, row);
        obj.PopulateData();
        return obj;
    }

    public void PopulateData()
    {
    }

    // Credit to https://github.com/Critical-Impact/CriticalCommonLib/blob/54b594f459ba1479a3bc67ad18ca65d206c63571/Sheets/SpecialShopListing.cs#L9
    // No idea why this is a thing...
    public static uint FixItemId(uint itemId, byte UseCurrencyType)
    {

        if (itemId == 0 || itemId > 7)
        {
            return itemId;
        }

        switch (UseCurrencyType)
        {
            // Scrips
            case 16:
                switch (itemId)
                {
                    case 1: return 28;
                    case 2: return 25199;
                    case 4: return 25200;
                    case 6: return 33913;
                    case 7: return 33914;
                    default: return itemId;
                }
            // Gil
            case 8:
                return 1;
            case 4:
                var tomestones = BuildTomestones();
                return tomestones[itemId];
            case 2:
                if (itemId == 1)
                {
                    tomestones = BuildTomestones();
                    return tomestones[itemId];
                }
                else
                {
                    return itemId;
                }
            default:
                return itemId;
                // Tomestones
                //case 4:
                //    if (TomeStones.ContainsKey(itemId))
                //    {
                //        return TomeStones[itemId];
                //    }

                //    return itemId;
        }
    }

    private static Dictionary<uint, uint> BuildTomestones()
    {
        var tomestoneItems = ExcelCache<TomestonesItem>.GetSheet()
            .Where(t => t.Tomestones.RowId > 0)
            .OrderBy(t => t.Tomestones.RowId)
            .ToArray();

        var tomeStones = new Dictionary<uint, uint>();

        for (uint i = 0; i < tomestoneItems.Length; i++)
        {
            tomeStones[i + 1] = tomestoneItems[i].Item.RowId;
        }

        return tomeStones;
    }

    // Original
    public uint RowId => row;

    public readonly ReadOnlySeString Name => page.ReadString(offset, offset);
    public readonly Collection<ItemStruct> Item => new(page, offset, offset, &ItemCtor, 60);
    public readonly RowRef<Quest> Quest => new(page.Module, page.ReadUInt32(offset + 5764), page.Language);
    public readonly uint Unknown0 => page.ReadUInt32(offset + 5768);
    public readonly uint Unknown1 => page.ReadUInt32(offset + 5772);
    public readonly int CompleteText => page.ReadInt32(offset + 5776);
    public readonly int NotCompleteText => page.ReadInt32(offset + 5780);
    public readonly ushort Unknown2 => page.ReadUInt16(offset + 5784);
    public readonly ushort Unknown5 => page.ReadUInt16(offset + 5786);
    public readonly byte UseCurrencyType => page.ReadUInt8(offset + 5788);
    public readonly bool Unknown3 => page.ReadPackedBool(offset + 5789, 0);
    public readonly bool Unknown4 => page.ReadPackedBool(offset + 5789, 1);

    private static ItemStruct ItemCtor(ExcelPage page, uint parentOffset, uint offset, uint i) => new(page, parentOffset, offset + 4 + i * 96);

    public readonly struct ItemStruct(ExcelPage page, uint parentOffset, uint offset)
    {
        public readonly Collection<RowRef<SpecialShopItemCategory>> Category => new(page, parentOffset, offset, &CategoryCtor, 2);
        public readonly RowRef<Quest> Quest => new(page.Module, (uint)page.ReadInt32(offset + 48), page.Language);
        public readonly Collection<int> Unknown0 => new(page, parentOffset, offset, &Unknown0Ctor, 4);
        public readonly RowRef<Achievement> AchievementUnlock => new(page.Module, (uint)page.ReadInt32(offset + 68), page.Language);
        public readonly int Unknown2 => page.ReadInt32(offset + 72);
        public readonly ushort PatchNumber => page.ReadUInt16(offset + 82);
        public readonly Collection<byte> Unknown1 => new(page, parentOffset, offset, &Unknown1Ctor, 6);

        public readonly Collection<ReceiveItemsStruct> ReceiveItems => new(page, parentOffset, offset, &ReceiveItemsCtor, 2);
        public readonly Collection<ItemCostsStruct> ItemCosts => new(page, parentOffset, offset, &ItemCostsCtor, 3);
        public readonly Collection<ItemCostsStruct> FixedItemCosts => new(page, parentOffset, offset, &ItemCostsCtor, 3);

        private static RowRef<SpecialShopItemCategory> CategoryCtor(ExcelPage page, uint parentOffset, uint offset, uint i) => new(page.Module, (uint)page.ReadInt32(offset + 28 + i * 4), page.Language);
        private static int Unknown0Ctor(ExcelPage page, uint parentOffset, uint offset, uint i) => page.ReadInt32(offset + 52 + i * 4);
        private static byte Unknown1Ctor(ExcelPage page, uint parentOffset, uint offset, uint i) => page.ReadUInt8(offset + 87 + i);
        private static ReceiveItemsStruct ReceiveItemsCtor(ExcelPage page, uint parentOffset, uint offset, uint i) => new(page, parentOffset, offset, i);
        private static ItemCostsStruct ItemCostsCtor(ExcelPage page, uint parentOffset, uint offset, uint i) => new(page, parentOffset, offset, i);

        public readonly struct ReceiveItemsStruct(ExcelPage page, uint parentOffset, uint offset, uint i)
        {
            public readonly uint ReceiveCount => page.ReadUInt32(offset + i * 4);
            public readonly RowRef<Item> Item => new(page.Module, (uint)page.ReadInt32(offset + 20 + i * 4), page.Language);
            public readonly bool ReceiveHq => page.ReadBool(offset + 93 + i);
        }

        public readonly struct ItemCostsStruct(ExcelPage page, uint parentOffset, uint offset, uint i)
        {
            public readonly uint CurrencyCost => page.ReadUInt32(offset + 8 + i * 4);
            public readonly RowRef<Item> ItemCost => new(page.Module, FixItemId((uint)page.ReadInt32(offset + 36 + i * 4), page.ReadUInt8(parentOffset + 5788)), page.Language);
            public readonly ushort CollectabilityCost => page.ReadUInt16(offset + 76 + i * 2);
            public readonly byte HqCost => page.ReadUInt8(offset + 84 + i);
        }
    }

}
