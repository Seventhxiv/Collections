using Dalamud.Logging;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Collections;

public class GlamDresserObserver
{
    private List<uint> itemIds = new();
    public Dictionary<uint, GlamourDresserItem> glamItems = new();

    private DateTime? openedTime;
    private DateTime? latestUpdatedTime;
    private bool wasUpdated = false;
    public unsafe void OnFrameworkTick(IFramework framework)
    {

        //var boxOpen = IsWindowOpen(gui, "MiragePrismPrismBox");

        ////return boxOpen;


        var dresserAddon = (AtkUnitBase*)Services.GameGui.GetAddonByName("MiragePrismPrismBox", 1);
        var dresserOpen = dresserAddon != null && dresserAddon->IsVisible;

        // Reset state if not interacting with Dresser
        if (!dresserOpen)
        {
            openedTime = null;
            wasUpdated = false;
            return;
        }

        // Return if already updated
        if (wasUpdated)
        {
            return;
        }

        // Reset if updated in the last 10 seconds
        if (latestUpdatedTime != null && latestUpdatedTime > DateTime.Now - TimeSpan.FromMilliseconds(10000))
        {
            wasUpdated = true;
            return;
        }

        if (getDresserWarmedUp())
        {
            wasUpdated = true;
            latestUpdatedTime = DateTime.Now;
            updateDresserContents();
            Services.Configuration.updateDresserContentIds(itemIds);
            Services.GlamourDresserManager.loadItemsFromConfiguration();
        }
    }

    private void updateConfiguration()
    {
        foreach (var itemId in itemIds)
        {
            Services.Configuration.DresserContentIds.Add(itemId);
        }
        Services.Configuration.Save();
    }

    private bool getDresserWarmedUp()
    {
        // Initialize dresserOpenedTime on first open
        if (openedTime == null)
        {
            openedTime = DateTime.Now;
            return false;
        }

        // Less than 500ms passed - not warmed up
        if (openedTime > DateTime.Now - TimeSpan.FromMilliseconds(500))
        {
            return false;
        }

        // Warmed up after 500ms from open time - reset state
        openedTime = null;
        return true;
    }

    private unsafe AgentInterface* DresserAgent =>
        FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.MiragePrismMiragePlate);
    private unsafe void updateDresserContents()
    {
        //var agents = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->GetUiModule()->GetAgentModule();
        //var dresserAgent = agents->GetAgentByInternalId(AgentId.MiragePrismPrismBox);

        if (!DresserAgent->IsAgentActive())
        {
            Dev.Log("dresser not warmed up");
            return;
        }

        itemIds.Clear();
        glamItems.Clear();
        var itemsStart = *(IntPtr*)((IntPtr)DresserAgent + 0x28);
        if (itemsStart == IntPtr.Zero)
        {
            PluginLog.Debug("dresser contents: itemsStart at 0");
        }

        for (var i = 0; i < 800; i++)
        {
            var glamItem = *(GlamourDresserItem*)(itemsStart + (i * 136));
            if (glamItem.ItemId == 0)
            {
                continue;
            }
            var itemId = glamItem.ItemId > 1000000 ? glamItem.ItemId - 1000000 : glamItem.ItemId;
            itemIds.Add(itemId);
            glamItems[itemId] = glamItem;
        }

        Dev.Log($"Final size: {itemIds.Count}");
    }

    //private static bool IsDresserOpen(GameGui gui)
    //{
    //    var boxOpen = IsWindowOpen(gui, "MiragePrismPrismBox");

    //    return boxOpen;
    //}

    //private static unsafe bool IsWindowOpen(GameGui gui, string name)
    //{
    //    var addon = (AtkUnitBase*)gui.GetAddonByName(name, 1);
    //    return addon != null && addon->IsVisible;
    //}

}

[StructLayout(LayoutKind.Explicit, Size = 136)]
public readonly struct GlamourDresserItem
{
    [FieldOffset(0x70)]
    internal readonly uint Index;

    [FieldOffset(0x74)]
    internal readonly uint ItemId;

    [FieldOffset(0x86)]
    internal readonly byte StainId;
}
