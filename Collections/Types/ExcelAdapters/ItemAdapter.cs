namespace Collections;

[Sheet("Item")]
public unsafe struct ItemAdapter(ExcelPage page, uint offset, uint row) : IExcelRow<ItemAdapter>
{
    public List<Job>? Jobs { get; set; }
    public EquipSlot EquipSlot { get; set; }
    public bool IsEquipment { get; set; }

    static ItemAdapter IExcelRow<ItemAdapter>.Create(ExcelPage page, uint offset, uint row)
    {
        var obj = new ItemAdapter(page, offset, row);
        obj.PopulateData();
        return obj;
    }

    public void PopulateData()
    {
        InitializeEquipSlot();
        InitializeJobs();
    }

    public void InitializeEquipSlot()
    {
        var equipSlotCategory = ExcelCache<EquipSlotCategoryAdapter>.GetSheet().GetRow(EquipSlotCategory.RowId);
        if (equipSlotCategory != null)
        {
            EquipSlot = equipSlotCategory.Value.EquipSlot;
            IsEquipment = EquipSlot != EquipSlot.None;
        }
    }

    public void InitializeJobs()
    {
        if (IsEquipment)
        {
            var classJobCategory = ExcelCache<ClassJobCategoryAdapter>.GetSheet().GetRow(ClassJobCategory.RowId);
            //Services.classJobCategorySheet[(int)ClassJobCategory.Value.RowId];
            if (classJobCategory != null)
            {
                Jobs = classJobCategory.Value.Jobs;
            }
        }
        else
        {
            Jobs = new List<Job>();
        }
    }

    // Original
    public uint RowId => row;

    public readonly ReadOnlySeString Singular => page.ReadString(offset, offset);
    public readonly ReadOnlySeString Plural => page.ReadString(offset + 4, offset);
    public readonly ReadOnlySeString Description => page.ReadString(offset + 8, offset);
    public readonly ReadOnlySeString Name => page.ReadString(offset + 12, offset);
    public readonly sbyte Adjective => page.ReadInt8(offset + 16);
    public readonly sbyte PossessivePronoun => page.ReadInt8(offset + 17);
    public readonly sbyte StartsWithVowel => page.ReadInt8(offset + 18);
    public readonly sbyte Unknown0 => page.ReadInt8(offset + 19);
    public readonly sbyte Pronoun => page.ReadInt8(offset + 20);
    public readonly sbyte Article => page.ReadInt8(offset + 21);
    public readonly ulong ModelMain => page.ReadUInt64(offset + 24);
    public readonly ulong ModelSub => page.ReadUInt64(offset + 32);
    public readonly ushort DamagePhys => page.ReadUInt16(offset + 40);
    public readonly ushort DamageMag => page.ReadUInt16(offset + 42);
    public readonly ushort Delayms => page.ReadUInt16(offset + 44);
    public readonly ushort BlockRate => page.ReadUInt16(offset + 46);
    public readonly ushort Block => page.ReadUInt16(offset + 48);
    public readonly ushort DefensePhys => page.ReadUInt16(offset + 50);
    public readonly ushort DefenseMag => page.ReadUInt16(offset + 52);
    public readonly Collection<short> BaseParamValue => new(page, offset, offset, &BaseParamValueCtor, 6);
    public readonly Collection<short> BaseParamValueSpecial => new(page, offset, offset, &BaseParamValueSpecialCtor, 6);
    public readonly byte LevelEquip => page.ReadUInt8(offset + 78);
    public readonly byte RequiredPvpRank => page.ReadUInt8(offset + 79);
    public readonly byte EquipRestriction => page.ReadUInt8(offset + 80);
    public readonly RowRef<ClassJobCategory> ClassJobCategory => new(page.Module, (uint)page.ReadUInt8(offset + 81), page.Language);
    public readonly RowRef<GrandCompany> GrandCompany => new(page.Module, (uint)page.ReadUInt8(offset + 82), page.Language);
    public readonly RowRef<ItemSeries> ItemSeries => new(page.Module, (uint)page.ReadUInt8(offset + 83), page.Language);
    public readonly byte BaseParamModifier => page.ReadUInt8(offset + 84);
    public readonly RowRef<ClassJob> ClassJobUse => new(page.Module, (uint)page.ReadUInt8(offset + 85), page.Language);
    public readonly byte Unknown2 => page.ReadUInt8(offset + 86);
    public readonly byte Unknown3 => page.ReadUInt8(offset + 87);
    public readonly Collection<RowRef<BaseParam>> BaseParam => new(page, offset, offset, &BaseParamCtor, 6);
    public readonly RowRef<ItemSpecialBonus> ItemSpecialBonus => new(page.Module, (uint)page.ReadUInt8(offset + 94), page.Language);
    public readonly byte ItemSpecialBonusParam => page.ReadUInt8(offset + 95);
    public readonly Collection<RowRef<BaseParam>> BaseParamSpecial => new(page, offset, offset, &BaseParamSpecialCtor, 6);
    public readonly byte MaterializeType => page.ReadUInt8(offset + 102);
    public readonly byte MateriaSlotCount => page.ReadUInt8(offset + 103);
    public readonly byte SubStatCategory => page.ReadUInt8(offset + 104);
    public readonly bool IsAdvancedMeldingPermitted => page.ReadPackedBool(offset + 105, 0);
    public readonly bool IsPvP => page.ReadPackedBool(offset + 105, 1);
    public readonly bool IsGlamorous => page.ReadPackedBool(offset + 105, 2);
    public readonly RowRef AdditionalData => (/* FilterGroup */ page.ReadUInt8(offset + 151)) switch
    {
        14 => RowRef.GetFirstValidRowOrUntyped(page.Module, page.ReadUInt32(offset + 112), [typeof(HousingExterior), typeof(HousingInterior), typeof(HousingYardObject), typeof(HousingFurniture), typeof(HousingPreset), typeof(HousingUnitedExterior)], 655606763, page.Language),
        15 => RowRef.Create<Stain>(page.Module, page.ReadUInt32(offset + 112), page.Language),
        18 => RowRef.Create<TreasureHuntRank>(page.Module, page.ReadUInt32(offset + 112), page.Language),
        20 => RowRef.Create<GardeningSeed>(page.Module, page.ReadUInt32(offset + 112), page.Language),
        25 => RowRef.Create<AetherialWheel>(page.Module, page.ReadUInt32(offset + 112), page.Language),
        26 => RowRef.Create<CompanyAction>(page.Module, page.ReadUInt32(offset + 112), page.Language),
        27 => RowRef.Create<TripleTriadCard>(page.Module, page.ReadUInt32(offset + 112), page.Language),
        28 => RowRef.Create<AirshipExplorationPart>(page.Module, page.ReadUInt32(offset + 112), page.Language),
        32 => RowRef.Create<Orchestrion>(page.Module, page.ReadUInt32(offset + 112), page.Language),
        36 => RowRef.Create<SubmarinePart>(page.Module, page.ReadUInt32(offset + 112), page.Language),
        _ => RowRef.CreateUntyped(page.ReadUInt32(offset + 112), page.Language),
    };
    public readonly uint StackSize => page.ReadUInt32(offset + 116);
    public readonly uint PriceMid => page.ReadUInt32(offset + 120);
    public readonly uint PriceLow => page.ReadUInt32(offset + 124);
    public readonly RowRef<ItemRepairResource> ItemRepair => new(page.Module, (uint)page.ReadInt32(offset + 128), page.Language);
    public readonly RowRef<Item> ItemGlamour => new(page.Module, (uint)page.ReadInt32(offset + 132), page.Language);
    public readonly ushort Icon => page.ReadUInt16(offset + 136);
    public readonly RowRef<ItemLevel> LevelItem => new(page.Module, (uint)page.ReadUInt16(offset + 138), page.Language);
    public readonly ushort Unknown4 => page.ReadUInt16(offset + 140);
    public readonly RowRef<ItemAction> ItemAction => new(page.Module, (uint)page.ReadUInt16(offset + 142), page.Language);
    public readonly ushort Cooldowns => page.ReadUInt16(offset + 144);
    public readonly ushort Desynth => page.ReadUInt16(offset + 146);
    public readonly ushort AetherialReduce => page.ReadUInt16(offset + 148);
    public readonly byte Rarity => page.ReadUInt8(offset + 150);
    public readonly byte FilterGroup => page.ReadUInt8(offset + 151);
    public readonly RowRef<ItemUICategory> ItemUICategory => new(page.Module, (uint)page.ReadUInt8(offset + 152), page.Language);
    public readonly RowRef<ItemSearchCategory> ItemSearchCategory => new(page.Module, (uint)page.ReadUInt8(offset + 153), page.Language);
    public readonly RowRef<EquipSlotCategory> EquipSlotCategory => new(page.Module, (uint)page.ReadUInt8(offset + 154), page.Language);
    public readonly RowRef<ItemSortCategory> ItemSortCategory => new(page.Module, (uint)page.ReadUInt8(offset + 155), page.Language);
    public readonly byte DyeCount => page.ReadUInt8(offset + 156);
    public readonly byte CastTimeSeconds => page.ReadUInt8(offset + 157);
    public readonly RowRef<ClassJob> ClassJobRepair => new(page.Module, (uint)page.ReadUInt8(offset + 158), page.Language);
    public readonly bool IsUnique => page.ReadPackedBool(offset + 159, 0);
    public readonly bool IsUntradable => page.ReadPackedBool(offset + 159, 1);
    public readonly bool IsIndisposable => page.ReadPackedBool(offset + 159, 2);
    public readonly bool Lot => page.ReadPackedBool(offset + 159, 3);
    public readonly bool CanBeHq => page.ReadPackedBool(offset + 159, 4);
    public readonly bool IsCrestWorthy => page.ReadPackedBool(offset + 159, 5);
    public readonly bool IsCollectable => page.ReadPackedBool(offset + 159, 6);
    public readonly bool AlwaysCollectable => page.ReadPackedBool(offset + 159, 7);

    private static short BaseParamValueCtor(ExcelPage page, uint parentOffset, uint offset, uint i) => page.ReadInt16(offset + 54 + i * 2);
    private static short BaseParamValueSpecialCtor(ExcelPage page, uint parentOffset, uint offset, uint i) => page.ReadInt16(offset + 66 + i * 2);
    private static RowRef<BaseParam> BaseParamCtor(ExcelPage page, uint parentOffset, uint offset, uint i) => new(page.Module, (uint)page.ReadUInt8(offset + 88 + i), page.Language);
    private static RowRef<BaseParam> BaseParamSpecialCtor(ExcelPage page, uint parentOffset, uint offset, uint i) => new(page.Module, (uint)page.ReadUInt8(offset + 96 + i), page.Language);

}