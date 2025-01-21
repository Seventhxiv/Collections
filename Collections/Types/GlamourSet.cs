namespace Collections;

[Serializable]
public class GlamourTree
{
    public List<GlamourDirectory> Directories = new();
}

[Serializable]
public class GlamourDirectory
{
    public string Name;
    public List<GlamourSet> GlamourSets = new();

    public GlamourDirectory(string name)
    {
        Name = name;
    }
}

[Serializable]
public class GlamourSet
{
    public string Name;
    public Dictionary<EquipSlot, GlamourItem> Items = new();

    public GlamourSet(string name)
    {
        Name = name;
    }

    public GlamourItem? GetItem(EquipSlot equipSlot)
    {
        Items.TryGetValue(equipSlot, out var item);
        return item;
    }

    public void SetItem(EquipSlot equipSlot, GlamourItem glamouritem)
    {
        if (!Services.DataProvider.SupportedEquipSlots.Contains(equipSlot))
            throw new ArgumentOutOfRangeException($"Equip slot {equipSlot} not supported for GlamourSet");

        Items[equipSlot] = glamouritem;
    }

    public void SetItem(ItemAdapter item, uint stainId)
    {
        if (!Services.DataProvider.SupportedEquipSlots.Contains(item.EquipSlot))
            throw new ArgumentOutOfRangeException($"Equip slot {item.EquipSlot} not supported for GlamourSet");

        Items[item.EquipSlot] = new GlamourItem(item.RowId, stainId);
    }

    public void ClearEquipSlot(EquipSlot equipSlot)
    {
        Items.Remove(equipSlot);
    }
}

[Serializable]
public class GlamourItem
{
    public uint ItemId;
    public uint StainId;

    public GlamourItem(uint itemId, uint stainId)
    {
        ItemId = itemId;
        StainId = stainId;
    }

    public GlamourCollectible GetCollectible()
    {
        return CollectibleCache<GlamourCollectible, ItemAdapter>.Instance.GetObject(ItemId);
    }

    public StainAdapter GetStain()
    {
        return (StainAdapter)ExcelCache<StainAdapter>.GetSheet().GetRow(StainId)!;
    }


    public EquipSlot GetEquipSlot()
    {
        return GetCollectible().ExcelRow.EquipSlot;
    }
}
