using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Collections;

/**
 * 1. Sets up observer on Glamour Dresser
 * 2. Acts as interface to interact with the Glamour Dresser plates
 */
public class GlamourDresserManager
{
    public GlamDresserObserver GlamDresserObserver { get; init; }
    private ExcelSheet<Lumina.Excel.GeneratedSheets.Cabinet> cabinetSheet { get; init; }
    public List<uint> Items { get; set; } = new();

    public GlamourDresserManager()
    {
        Dev.Start();
        GlamDresserObserver = new GlamDresserObserver();
        cabinetSheet = Excel.GetExcelSheet<Lumina.Excel.GeneratedSheets.Cabinet>();
        loadItemsFromConfiguration();
        Dev.Stop();
    }

    public void loadItemsFromConfiguration()
    {
        var startCount = Items.Count;
        var glamDresserIdList = Services.Configuration.DresserContentIds;
        Items.Clear();
        foreach (var itemId in glamDresserIdList)
        {
            Items.Add(itemId);
        }
        Dev.Log($"Updated GlamourDresser count {startCount} -> {Items.Count}");
    }

    private unsafe AgentInterface* EditorAgent =>
    FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.MiragePrismMiragePlate);

    public unsafe void SetPlateItem(ItemAdapter item)
    {
        SetPlateSlotPointer(item);

        // Look up in dresser
        var glamItem = GetGlamItemFromDresser(item.RowId);
        if (glamItem != null)
        {
            Dev.Log("Setting plate item from dresser: " + item.Name);
            var glamItemNonnull = (GlamourDresserItem)glamItem;
            Services.GameFunctionsExecutor.SetGlamourPlate((IntPtr)EditorAgent, PlateItemSource.GlamourDresser, (int)glamItemNonnull.Index, glamItemNonnull.ItemId, glamItemNonnull.StainId);
            return;
        }

        // Look up in armory
        if (GetIsItemInArmory(item.RowId))
        {
            Dev.Log("Setting plate item from armory: " + item.Name);
            var cabinetId = GetCabinetId(item.RowId);
            Services.GameFunctionsExecutor.SetGlamourPlate((IntPtr)EditorAgent, PlateItemSource.Armoire, (int)cabinetId, item.RowId, 0);
            return;
        }
    }

    // TODO Test this after changing EquipSlot values/order
    private unsafe void SetPlateSlotPointer(ItemAdapter item)
    {
        var equipSlot = item.EquipSlot;
        var editorInfo = *(IntPtr*)((IntPtr)EditorAgent + 0x28);
        var slotPtr = (EquipSlot*)(editorInfo + 0x18);
        *slotPtr = equipSlot;
    }

    private bool GetIsItemInDresser(uint itemId)
    {
        return GetGlamItemFromDresser(itemId) != null;
    }

    private GlamourDresserItem? GetGlamItemFromDresser(uint itemId)
    {
        if (GlamDresserObserver.glamItems.ContainsKey(itemId))
        {
            return GlamDresserObserver.glamItems[itemId];
        }
        else
        {
            return null;
        }
    }

    private unsafe bool GetIsItemInArmory(uint itemId)
    {
        var cabinetLoaded = UIState.Instance()->Cabinet.IsCabinetLoaded();
        if (cabinetLoaded)
        {
            var cabinetId = GetCabinetId(itemId);
            return UIState.Instance()->Cabinet.IsItemInCabinet((int)cabinetId);
        }
        return false;
    }

    public uint GetCabinetId(uint itemId)
    {
        var cabinetItem = cabinetSheet.Where(entry => entry.Item.Row == itemId).FirstOrDefault();
        return cabinetItem.RowId;
    }
}
