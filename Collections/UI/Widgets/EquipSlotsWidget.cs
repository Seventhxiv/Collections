using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Internal;
using ImGuiNET;
using ImGuiScene;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Collections;

public class EquipSlotsWidget
{
    public EquipSlot activeEquipSlot { get; set; } = EquipSlot.Chest;
    public Dictionary<EquipSlot, PaletteWidget> equipSlotToPalette = new();

    private GlamourTreeWidget GlamourTreeWidget { get; init; }
    public EquipSlotsWidget(GlamourTreeWidget glamourTreeWidget)
    {
        GlamourTreeWidget = glamourTreeWidget;
        BuildEquipSlotIcons();
    }

    private List<EquipSlot> equipSlotList = new()
    {
        EquipSlot.MainHand,
        EquipSlot.OffHand,
        EquipSlot.Head,
        EquipSlot.Chest,
        EquipSlot.Hands,
        EquipSlot.Legs,
        EquipSlot.Feet,
    };

    //Enum.GetValues(typeof(EquipSlot)).Cast<EquipSlot>().ToList();
    public unsafe void Draw()
    {
        IDalamudTextureWrap icon;
        var bgColor = *ImGui.GetStyleColorVec4(ImGuiCol.WindowBg);
        foreach (var equipSlot in equipSlotList)
        {
            if (Services.ItemManager.items.ContainsKey(equipSlot))
            {
                var origPos = ImGui.GetCursorPos();
                if (activeEquipSlot == equipSlot)
                {
                    ImGui.SetCursorPos(new Vector2(origPos.X - 1.8f, origPos.Y - 1.8f));
                    ImGui.ColorButton(equipSlot.ToString(), UiHelper.colors[UiHelper.CommonColor.blue], ImGuiColorEditFlags.NoTooltip, new Vector2(60.2f, 60.2f));
                    ImGui.SetCursorPos(origPos);
                }
                ImGui.SetCursorPos(origPos);

                origPos = ImGui.GetCursorPos();
                //Vector4 color = UiHelper.colors[UiHelper.CommonColor.grey2];
                ImGui.ColorButton(equipSlot.ToString(), bgColor, ImGuiColorEditFlags.NoTooltip, new Vector2(56, 56));
                ImGui.SetCursorPos(origPos);

                ImGui.SetItemAllowOverlap();
                if (GlamourTreeWidget.CurrentGlamourSet.set.ContainsKey(equipSlot))
                {
                    icon = GlamourTreeWidget.CurrentGlamourSet.set[equipSlot].icon;
                }
                else
                {
                    icon = equipSlotIcons[equipSlot];
                }

                if (ImGui.ImageButton(icon.ImGuiHandle, new Vector2(48, 50)))
                {
                    activeEquipSlot = equipSlot;
                }

                if (!equipSlotToPalette.ContainsKey(equipSlot))
                {
                    equipSlotToPalette[equipSlot] = new PaletteWidget();
                }

                var previousPos = ImGui.GetCursorPos();
                ImGui.SameLine();
                ImGui.SetItemAllowOverlap(); // Makes this button take precedence
                ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPos().X - 34, ImGui.GetCursorPos().Y + 35));

                ImGui.PushStyleColor(ImGuiCol.Text, equipSlotToPalette[equipSlot].pickedStain.VecColor);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0, 0, 0, 0));
                ImGuiComponents.IconButton(FontAwesomeIcon.PaintBrush);
                ImGui.PopStyleColor();
                ImGui.PopStyleColor();

                if (ImGui.BeginPopupContextItem($"open-dye-picker##{equipSlot}", ImGuiPopupFlags.MouseButtonLeft))
                {
                    equipSlotToPalette[equipSlot].Draw();
                    ImGui.EndPopup();
                }

                if (GlamourTreeWidget.CurrentGlamourSet.set.ContainsKey(equipSlot))
                {
                    if (equipSlotToPalette.ContainsKey(equipSlot))
                    {
                        if (equipSlotToPalette[equipSlot].updatedState)
                        {
                            var tryOn = false;
                            var item = GlamourTreeWidget.CurrentGlamourSet.set[equipSlot].item;
                            var stainId = equipSlotToPalette[equipSlot].pickedStain.RowId;

                            if (tryOn)
                            {
                                Services.GameFunctionsExecutor.TryOn(item.RowId, stainId);
                            }
                            else
                            {
                                Services.GameFunctionsExecutor.ChangeEquip(item, (int)stainId);
                            }
                            equipSlotToPalette[equipSlot].updatedState = false;
                        }
                    }
                }
            }
        }
    }

    private Dictionary<EquipSlot, IDalamudTextureWrap> equipSlotIcons = new();
    private void BuildEquipSlotIcons()
    {
        foreach (var equipSlot in equipSlotList)
        {
            var equipSlotName = Enum.GetName(typeof(EquipSlot), equipSlot);
            var iconPath = Path.Combine(Services.PluginInterface.AssemblyLocation.Directory?.FullName!, $"Data\\Resources\\{equipSlotName}.png");
            var icon = Services.PluginInterface.UiBuilder.LoadImage(iconPath);
            equipSlotIcons[equipSlot] = icon;
        }
    }
}

