using Dalamud.Game;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static FFXIVClientStructs.FFXIV.Client.Game.Character.DrawDataContainer;

namespace Collections;

public class GameActionsExecutor : BaseAddressResolver
{
    private delegate byte tryOnDelegate(uint unknownCanEquip, uint itemBaseId, ulong stainColor, uint itemGlamourId, byte unknownByte);
    private delegate void setGlamourPlateSlotDelegate(IntPtr agent, PlateItemSource plateItemSource, int glamId, uint itemId, byte stainId);

    private readonly tryOnDelegate tryOn;
    private readonly setGlamourPlateSlotDelegate setGlamourPlateSlot;

    protected nint tryOnPointer { get; private set; }
    protected nint setGlamourPlateSlotPointer { get; private set; }

    public GameActionsExecutor()
    {
        Setup(Services.SigScanner);
        tryOn = Marshal.GetDelegateForFunctionPointer<tryOnDelegate>(tryOnPointer);
        setGlamourPlateSlot = Marshal.GetDelegateForFunctionPointer<setGlamourPlateSlotDelegate>(setGlamourPlateSlotPointer);
    }

    protected override void Setup64Bit(ISigScanner SigScanner)
    {
        tryOnPointer = SigScanner.ScanText("E8 ?? ?? ?? ?? EB 35 BA ?? ?? ?? ??");
        setGlamourPlateSlotPointer = SigScanner.ScanText("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 48 8B 46 10 8B 1B");
    }

    public void ChangeEquip(Item item, int stainId = 0, bool save = true)
    {
        if (save)
            SaveChangedEquip(item, stainId);

        var equipSlot = Services.ItemManager.getItemEquipSlot(item);

        if (equipSlot == EquipSlot.MainHand || equipSlot == EquipSlot.OffHand)
        {
            var slot = equipSlot == EquipSlot.MainHand ? 0 : 1;
            var changeWeaponEquip = new ChangeWeaponEquip() { Set = (ushort)item.ModelMain, Base = (byte)(item.ModelMain >> 16), Variant = (byte)(item.ModelMain >> 32), Dye = (byte)stainId };
            ChangeWeapon(changeWeaponEquip, (WeaponSlot)slot);
        }
        else
        {
            var EquipItem = new ChangeEquipItem() { Id = (ushort)item.ModelMain, Variant = (byte)(item.ModelMain >> 16), Dye = (byte)stainId };
            ChangeEquip(EquipItem, (EquipmentSlot)equipSlot);
        }

        Dev.Log($"Preview item: " + item.Name);
    }

    private List<ChangedEquipState> changedEquipState = new();
    private void SaveChangedEquip(Item item, int stainId = 0)
    {
        //var itemToBeReplaced = GetItemToBeReplaced(item);
        //var equippedGear = GetEquippedGear();
        var equipSlot = Services.ItemManager.getItemEquipSlot(item);
        changedEquipState.Add(new ChangedEquipState() { equipSlot = equipSlot });
    }

    private class ChangedEquipState
    {
        public EquipSlot equipSlot;
        //public Item preItem;
        //public byte preStain;
    }

    //private unsafe (Item, byte, EquipSlot)? GetItemToBeReplaced(Item item)
    //{
    //    var itemSheet = Excel.GetExcelSheet<Item>()!;
    //    var equipSlot = Services.ItemManager.getItemEquipSlot(item);

    //    var container = InventoryManager.Instance()->GetInventoryContainer(InventoryType.EquippedItems);

    //    for (var i = 0; i < 13; i++)
    //    {
    //        var invSlot = container->GetInventorySlot(i);
    //        var invItem = itemSheet.GetRow(invSlot->GlamourID != 0 ? invSlot->GlamourID : invSlot->ItemID);
    //        if (Services.ItemManager.getItemEquipSlot(invItem) == equipSlot)
    //        {
    //            return (invItem, invSlot->Stain, equipSlot);
    //        }
    //        //items.Add((item, invSlot->Stain));
    //    }
    //    return null;
    //    //return items
    //    //    .Where(i => Services.ItemManager.getItemEquipSlot(i) != EquipSlot.None)
    //    //    .GroupBy(i => Services.ItemManager.getItemEquipSlot(i), i => i)
    //    //    .ToDictionary(i => i.Key, i => i.FirstOrDefault());
    //}

    public unsafe void ResetPreview()
    {
        var equipSlotsToReset = changedEquipState.Select(i => i.equipSlot).ToHashSet();
        if (equipSlotsToReset.Count == 0)
        {
            return;
        }
        var itemSheet = Excel.GetExcelSheet<Item>()!;
        var container = InventoryManager.Instance()->GetInventoryContainer(InventoryType.EquippedItems);

        foreach (var equipSlot in equipSlotsToReset)
        {
            ResetSlot(equipSlot);
        }
        for (var i = 0; i < 13; i++)
        {
            var invSlot = container->GetInventorySlot(i);
            var invItem = itemSheet.GetRow(invSlot->GlamourID != 0 ? invSlot->GlamourID : invSlot->ItemID);
            if (equipSlotsToReset.Contains(Services.ItemManager.getItemEquipSlot(invItem)))
                ChangeEquip(invItem, invSlot->Stain, false);
            //items.Add((item, invSlot->Stain));
        }
        changedEquipState.Clear();
    }

    private void ResetSlot(EquipSlot equipSlot)
    {
        if (equipSlot == EquipSlot.MainHand || equipSlot == EquipSlot.OffHand)
        {
            var slot = equipSlot == EquipSlot.MainHand ? 0 : 1;
            var changeWeaponEquip = new ChangeWeaponEquip() { Set = 0, Base = 0, Variant = 0, Dye = 0 };
            ChangeWeapon(changeWeaponEquip, (WeaponSlot)slot);
        }
        else
        {
            Dev.Log($"resetting {equipSlot}");
            var EquipItem = new ChangeEquipItem() { Id = 0, Variant = 0, Dye = 0 };
            ChangeEquip(EquipItem, (EquipmentSlot)equipSlot);
        }
    }

    private void ChangeEquip(ChangeEquipItem item, EquipmentSlot slot)
    {
        unsafe
        {
            var p = (Character*)Services.ClientState.LocalPlayer.Address;
            var e = new EquipmentModelId()
            {
                Id = item.Id,
                Stain = item.Dye,
                Variant = item.Variant,
            };
            p->DrawData.LoadEquipment(slot, &e, true);
        }
    }

    private void ChangeWeapon(ChangeWeaponEquip item, WeaponSlot slot)
    {
        unsafe
        {
            var p = (Character*)Services.ClientState.LocalPlayer.Address;
            var e = new WeaponModelId()
            {
                Id = item.Set,
                Type = item.Base,
                Stain = item.Dye,
                Variant = item.Variant,
            };
            p->DrawData.LoadWeapon(slot, e, 0, 0, 0, 0);
        }
    }

    public void TryOn(uint item)
    {
        TryOn(item, 0);
    }

    public void TryOn(uint item, ulong stain)
    {
        Dev.Log(item.ToString());
        tryOn(0xFF, item, stain, item, 0);
    }

    private static unsafe AgentInterface* EditorAgent =>
        FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.MiragePrismMiragePlate);
    public unsafe void SetGlamourPlate(PlateItemSource PlateItemSource, int glamId, uint itemId, byte stainId)
    {
        setGlamourPlateSlot((IntPtr)EditorAgent, PlateItemSource, glamId, itemId, stainId);
    }
    public unsafe void SetGlamourPlate(IntPtr agent, PlateItemSource PlateItemSource, int glamId, uint itemId, byte stainId)
    {
        setGlamourPlateSlot(agent, PlateItemSource, glamId, itemId, stainId);
    }

    public static bool IsInGPose()
    {
        // return Services.PluginInterface.UiBuilder.GposeActive;
        return GameMain.IsInGPose();
    }
}

public enum PlateItemSource
{
    GlamourDresser = 1,
    Armoire = 2,
}

[StructLayout(LayoutKind.Explicit, Size = 0x4)]
public struct ChangeEquipItem
{
    [FieldOffset(0)] public ushort Id;
    [FieldOffset(2)] public byte Variant;
    [FieldOffset(3)] public byte Dye;
}

[StructLayout(LayoutKind.Explicit)]
public struct ChangeWeaponEquip
{
    [FieldOffset(0x00)] public ushort Set;
    [FieldOffset(0x02)] public ushort Base;
    [FieldOffset(0x04)] public ushort Variant;
    [FieldOffset(0x06)] public byte Dye;
}
