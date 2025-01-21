namespace Collections;

[Sheet("EquipSlotCategory")]
public struct EquipSlotCategoryAdapter(ExcelPage page, uint offset, uint row) : IExcelRow<EquipSlotCategoryAdapter>
{
    static EquipSlotCategoryAdapter IExcelRow<EquipSlotCategoryAdapter>.Create(ExcelPage page, uint offset, uint row)
    {
        var obj = new EquipSlotCategoryAdapter(page, offset, row);
        obj.PopulateData();
        return obj;
    }

    public void PopulateData()
    {
        EquipSlot = GetEquipSlot();
    }

    public EquipSlot EquipSlot { get; set; }

    private EquipSlot GetEquipSlot()
    {
        if (MainHand != 0)
            return EquipSlot.MainHand;
        if (OffHand != 0)
            return EquipSlot.OffHand;
        if (Head != 0)
            return EquipSlot.Head;
        if (Body != 0)
            return EquipSlot.Body;
        if (Gloves != 0)
            return EquipSlot.Gloves;
        if (Legs != 0)
            return EquipSlot.Legs;
        if (Feet != 0)
            return EquipSlot.Feet;
        return EquipSlot.None;
    }

    // Can also handle this with reflections, doing it explicitly for now
    //foreach (var equipSlot in GetEnumValues<EquipSlot>())
    //{
    //    if (this.GetProperty<sbyte>(equipSlot.GetEnumName()) != 0)
    //    {
    //        EquipSlot = equipSlot;
    //        return;
    //    }
    //}

    // Original
    public uint RowId => row;

    public readonly sbyte MainHand => page.ReadInt8(offset);
    public readonly sbyte OffHand => page.ReadInt8(offset + 1);
    public readonly sbyte Head => page.ReadInt8(offset + 2);
    public readonly sbyte Body => page.ReadInt8(offset + 3);
    public readonly sbyte Gloves => page.ReadInt8(offset + 4);
    public readonly sbyte Waist => page.ReadInt8(offset + 5);
    public readonly sbyte Legs => page.ReadInt8(offset + 6);
    public readonly sbyte Feet => page.ReadInt8(offset + 7);
    public readonly sbyte Ears => page.ReadInt8(offset + 8);
    public readonly sbyte Neck => page.ReadInt8(offset + 9);
    public readonly sbyte Wrists => page.ReadInt8(offset + 10);
    public readonly sbyte FingerL => page.ReadInt8(offset + 11);
    public readonly sbyte FingerR => page.ReadInt8(offset + 12);
    public readonly sbyte SoulCrystal => page.ReadInt8(offset + 13);

}
