using Dalamud.Interface.Windowing;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Collections;

public class InstanceWindow : Window, IDisposable
{

    private FiltersWidget FiltersWidget { get; init; }
    private CollectionWidget CollectionWidget { get; init; }
    private CollectionWidget GlamCollectionWidget { get; init; }
    public InstanceWindow() : base(
        "Collections - Instance")
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new System.Numerics.Vector2(300, 300),
            MaximumSize = new System.Numerics.Vector2(1500, 1500)
        };
        FiltersWidget = new FiltersWidget();
        CollectionWidget = new CollectionWidget(FiltersWidget);
        GlamCollectionWidget = new CollectionWidget(FiltersWidget);
    }

    private List<uint> items = new();
    private bool enteringInstance = false;
    public unsafe void OnFrameworkTick(Dalamud.Game.Framework framework)
    {
        var instanceId = GetCurrentInstance();
        if (Services.DataGenerator.InstancesDataParser.contentFinderConditionToItems.ContainsKey(instanceId))
        {
            items = Services.DataGenerator.InstancesDataParser.contentFinderConditionToItems[instanceId];
            if (enteringInstance || !enteringInstance)
            {
                IsOpen = true;
                enteringInstance = false;
            }
        }
        else
        {
            enteringInstance = true;
            IsOpen = false;
        }
    }

    private uint GetCurrentInstance()
    {
        var territoryType = Excel.GetExcelSheet<TerritoryType>().GetRow(Services.ClientState.TerritoryType);
        return territoryType.ContentFinderCondition.Row;
    }

    public void Dispose()
    {
    }

    private List<ICollectible> collectibles = new();
    private List<ICollectible> glamCollectibles = new();
    public override void OnOpen()
    {
        collectibles.Clear();
        glamCollectibles.Clear();
        foreach (var item in items)
        {
            var glamItem = Services.ItemManager.items.SelectMany(e => e.Value).Where(e => e.CollectibleUnlockItem.item.RowId == item).FirstOrDefault();
            if (glamItem != null)
            {
                glamItem.UpdateObtainedState();
                glamCollectibles.Add(glamItem);
                continue;
            }

            var mountItem = MountCollectible.GetCollection().Where(e =>
            {
                if (e.CollectibleUnlockItem != null)
                    return e.CollectibleUnlockItem.item.RowId == item;
                else
                    return false;
            }
            ).FirstOrDefault();
            if (mountItem != null)
            {
                mountItem.UpdateObtainedState();
                collectibles.Add(mountItem);
                continue;
            }

            var minionItem = MinionCollectible.GetCollection().Where(e =>
            {
                if (e.CollectibleUnlockItem != null)
                    return e.CollectibleUnlockItem.item.RowId == item;
                else
                    return false;
            }
            ).FirstOrDefault();
            if (minionItem != null)
            {
                minionItem.UpdateObtainedState();
                collectibles.Add(minionItem);
                continue;
            }
        }
    }

    public override void Draw()
    {
        //PopulateCollectibles();
        if (ImGui.BeginTable("instance-table", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.SizingFixedFit))
        {
            // Collections
            ImGui.TableNextColumn();
            ImGui.TableHeader("Collectibles");

            ImGui.TableNextRow(); // ImGuiTableRowFlags.None, UiHelper.GetLengthToBottomOfWindow() / 2
            ImGui.TableNextColumn();
            CollectionWidget.Draw(collectibles, false);

            // Glam
            if (glamCollectibles.Any())
            {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.TableHeader("Glamour");

                ImGui.TableNextRow(); // ImGuiTableRowFlags.None, UiHelper.GetLengthToBottomOfWindow()
                ImGui.TableNextColumn();
                GlamCollectionWidget.Draw(glamCollectibles);
            }

            ImGui.EndTable();
        }

    }
}

