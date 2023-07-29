using ImGuiNET;
using System;
using System.Linq;

namespace Collections;

public class GlamourWindow : IDrawable
{
    private PaletteWidget PaletteWidget { get; init; }
    private GlamourTreeWidget GlamourTreeWidget { get; init; }
    private JobSelectorWidget JobSelectorWidget { get; init; }
    private FiltersWidget FiltersWidget { get; init; }
    private EquipSlotsWidget EquipSlotsWidget { get; init; }
    private CollectionWidget CollectionWidget { get; init; }

    public GlamourWindow()
    {
        PaletteWidget = new PaletteWidget();
        GlamourTreeWidget = new GlamourTreeWidget();
        JobSelectorWidget = new JobSelectorWidget();
        FiltersWidget = new FiltersWidget(2);
        EquipSlotsWidget = new EquipSlotsWidget(GlamourTreeWidget);
        CollectionWidget = new CollectionWidget(FiltersWidget, PaletteWidget,
        GlamourTreeWidget, JobSelectorWidget, EquipSlotsWidget);
    }

    public void Draw()
    {

        if (ImGui.BeginTable("glam-tree", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.SizingFixedFit))
        {
            ImGui.TableNextColumn();
            ImGui.TableHeader("Glamour Sets");

            //ImGui.BeginChild("glamour-tree", new Vector2(160, 1000));
            ImGui.TableNextRow(ImGuiTableRowFlags.None, UiHelper.GetLengthToBottomOfWindow());
            ImGui.TableNextColumn();
            GlamourTreeWidget.Draw();
            //ImGui.EndChild();

            ImGui.EndTable();
        }

        //UiHelper.Group(() => GlamourTreeWidget.Draw(), 160);

        // Equip slot buttons
        ImGui.SameLine();

        //ImGui.BeginChild("slot icons", new Vector2(100, 1000));
        //UiHelper.Group(() =>
        //{
        //ImGui.PushStyleColor(ImGuiCol.Text, PaletteWidget.pickedStain.VecColor);
        //ImGuiComponents.IconButton(FontAwesomeIcon.PaintBrush);
        //ImGui.PopStyleColor();

        //if (ImGui.BeginPopupContextItem("open-dye-picker", ImGuiPopupFlags.MouseButtonLeft))
        //{
        //    PaletteWidget.Draw();
        //    ImGui.EndPopup();
        //}
        ImGui.BeginGroup();
        if (ImGui.BeginTable("equip-slots", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.SizingFixedFit))
        {
            ImGui.TableNextColumn();
            ImGui.TableHeader("Equip Slots");

            ImGui.TableNextRow(ImGuiTableRowFlags.None, UiHelper.GetLengthToBottomOfWindow());
            ImGui.TableNextColumn();
            EquipSlotsWidget.Draw();

            ImGui.EndTable();
        }
        ImGui.EndGroup();
        //}, 50);
        ImGui.SameLine();

        // Filters
        ImGui.BeginGroup();
        if (ImGui.BeginTable("filters", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.SizingFixedFit))
        {
            ImGui.TableNextColumn();
            ImGui.TableHeader("Equipped By");

            ImGui.TableNextRow(ImGuiTableRowFlags.None, UiHelper.GetLengthToBottomOfWindow() / 2 - ImGui.CalcTextSize(" ").Y * 3);
            ImGui.TableNextColumn();
            JobSelectorWidget.Draw();

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.TableHeader("Content Filters");

            ImGui.TableNextRow(ImGuiTableRowFlags.None, UiHelper.GetLengthToBottomOfWindow());
            ImGui.TableNextColumn();
            FiltersWidget.Draw();

            ImGui.EndTable();
        }

        //FiltersWidget.Draw();
        ImGui.EndGroup();

        // Glam collection
        ImGui.SameLine();
        ImGui.BeginGroup();
        //ImGui.BeginChild("collection-widget");
        //ImGui.BeginChildFrame(1, new Vector2(500, 500));
        var itemList = Services.ItemManager.items[EquipSlotsWidget.activeEquipSlot];
        CollectionWidget.Draw(itemList);
        //ImGui.EndChild();
        ImGui.EndGroup();
    }

    public void OnOpen()
    {
        var equipSlotList = Enum.GetValues(typeof(EquipSlot)).Cast<EquipSlot>().ToList();
        foreach (var equipSlot in equipSlotList)
        {
            if (Services.ItemManager.items.ContainsKey(equipSlot))
            {
                var itemList = Services.ItemManager.items[equipSlot];
                foreach (var item in itemList)
                {
                    item.UpdateObtainedState();
                }
            }
        }

        JobSelectorWidget.OnOpen();
    }

    public void Dispose()
    {
    }

}

