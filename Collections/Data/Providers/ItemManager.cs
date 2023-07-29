using ImGuiScene;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Collections;

public class ItemManager
{
    public Dictionary<EquipSlot, List<ICollectible>> items = new();
    public Dictionary<EquipSlot, TextureWrap> defaultEquipSlotIcons = new();
    public List<ClassJobEntity> classJobs = new();
    public List<StainEntity> stains = new();

    private ExcelSheet<Item> itemSheet { get; set; }
    public ItemManager()
    {
        Dev.Start();
        itemSheet = Excel.GetExcelSheet<Item>()!;
        //Task.Run(() => populateData());
        PopulateData();
        Dev.Stop();
    }
    private void PopulateData()
    {
        // Glamour collection
        items = itemSheet.AsParallel()
            .Where(entry => entry.LevelEquip >= 1)
            .Where(entry => getItemEquipSlot(entry) != EquipSlot.None)
            .Where(entry => !entry.Name.ToString().StartsWith("Dated "))
            .Select(entry => (ICollectible)new GlamourCollectible(entry))
            .GroupBy(entry => getItemEquipSlot(entry.CollectibleUnlockItem.item))
            .ToDictionary(g => g.Key, g => g.OrderByDescending(e => e.SortKey()).ToList());

        // Cache Equip slot icons
        var equipSlotList = Enum.GetValues(typeof(EquipSlot)).Cast<EquipSlot>().ToList();
        defaultEquipSlotIcons = equipSlotList.AsParallel().ToDictionary(entry => entry, entry =>
        {
            var iconHandler = new IconHandler(GetEquipSlotIcon(entry));
            return iconHandler.GetIcon();
        });

        // Class jobs
        var classJobSheet = Excel.GetExcelSheet<ClassJobEntity>()!;
        classJobs = classJobSheet.AsParallel().Where(entry => ClassJobEntity.ClassJobConfig.ContainsKey(entry.Abbreviation)).ToList();

        // Stains
        stains = Excel.GetExcelSheet<StainEntity>().Where(s => s.Color != 0).ToList();
    }

    public EquipSlot getItemEquipSlot(Item item)
    {
        if (item.EquipSlotCategory.Value.MainHand != 0)
            return EquipSlot.MainHand;
        if (item.EquipSlotCategory.Value.OffHand != 0)
            return EquipSlot.OffHand;
        if (item.EquipSlotCategory.Value.Head != 0)
            return EquipSlot.Head;
        if (item.EquipSlotCategory.Value.Body != 0)
            return EquipSlot.Chest;
        if (item.EquipSlotCategory.Value.Gloves != 0)
            return EquipSlot.Hands;
        if (item.EquipSlotCategory.Value.Legs != 0)
            return EquipSlot.Legs;
        if (item.EquipSlotCategory.Value.Feet != 0)
            return EquipSlot.Feet;
        return EquipSlot.None;
    }


    private int GetEquipSlotIcon(EquipSlot equipSlot)
    {
        switch (equipSlot)
        {
            case EquipSlot.Head: return 60124;
            case EquipSlot.Chest: return 60125;
            case EquipSlot.Hands: return 60126;
            case EquipSlot.Legs: return 60129;
            case EquipSlot.Feet: return 60130;
            case EquipSlot.MainHand: return 60102;
            case EquipSlot.OffHand: return 60110;
        }
        return 60135;
    }
}
