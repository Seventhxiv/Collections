using Dalamud.Interface.Components;
using System.IO;
using static Collections.UiHelper;

namespace Collections;

public class EquipSlotsWidget
{
    public EquipSlot activeEquipSlot { get; set; } = EquipSlot.Body;
    public Dictionary<EquipSlot, PaletteWidget> equipSlotToPalette = new();

    private Vector2 activeEquipSlotRectSize = new(60.2f, 60.2f);
    private Vector2 equipSlotBackgroundRectSize = new(56, 56);
    private Vector2 paletteWidgetButtonOffset = new(-34, 35);
    private Vector4 paletteWidgetButtonDefaultColor = ColorsPalette.WHITE;

    public static readonly List<EquipSlot> equipSlots = new()
    {
        EquipSlot.MainHand,
        EquipSlot.OffHand,
        EquipSlot.Head,
        EquipSlot.Body,
        EquipSlot.Gloves,
        EquipSlot.Legs,
        EquipSlot.Feet,
    };

    private GlamourSet currentGlamourSet { get; set; }
    private Dictionary<EquipSlot, bool> hoveredPaletteButton = new();

    private EventService EventService { get; init; }
    public EquipSlotsWidget(EventService eventService)
    {
        EventService = eventService;
        LoadEquipSlotIcons();
        InitializePaletteWidgets();
        eventService.Subscribe<GlamourSetChangeEvent, GlamourSetChangeEventArgs>(OnPublish);
        eventService.Subscribe<GlamourItemChangeEvent, GlamourItemChangeEventArgs>(OnPublish);
        eventService.Subscribe<DyeChangeEvent, DyeChangeEventArgs>(OnPublish);
    }

    public unsafe void Draw()
    {
        IDalamudTextureWrap icon;
        var bgColor = *ImGui.GetStyleColorVec4(ImGuiCol.WindowBg);
        foreach (var equipSlot in equipSlots)
        {
            if (Services.DataProvider.GlamourCollection.ContainsKey(equipSlot))
            {
                // Draw blue rect border over active equip slot
                var origPos = ImGui.GetCursorPos();
                if (activeEquipSlot == equipSlot)
                {
                    ImGui.SetCursorPos(new Vector2(origPos.X - 1.8f, origPos.Y - 1.8f));
                    ImGui.ColorButton(equipSlot.ToString(), ColorsPalette.BLUE, ImGuiColorEditFlags.NoTooltip, activeEquipSlotRectSize);
                    ImGui.SetCursorPos(origPos);
                }
                ImGui.SetCursorPos(origPos);

                // Draw bg rect over all equip slots
                origPos = ImGui.GetCursorPos();
                ImGui.ColorButton(equipSlot.ToString(), bgColor, ImGuiColorEditFlags.NoTooltip, equipSlotBackgroundRectSize);
                ImGui.SetCursorPos(origPos);

                ImGui.SetItemAllowOverlap();

                // Set icon (Glam piece selected)
                if (currentGlamourSet.set.ContainsKey(equipSlot))
                {
                    icon = currentGlamourSet.set[equipSlot].glamourCollectible.GetIconLazy();
                }
                // Set icon (Empty)
                else
                {
                    icon = equipSlotIcons[equipSlot];
                }

                // Draw equip slot buttons
                if (ImGui.ImageButton(icon.ImGuiHandle, new Vector2(48, 50)))
                {
                    SetEquipSlot(equipSlot);
                }

                // Interaction with buttons (details / reset)
                if (ImGui.IsItemHovered() && !hoveredPaletteButton[equipSlot]) // hoveredPaletteButton is here to take presedence of it over this
                {
                    // Details on hover
                    if (currentGlamourSet.set.ContainsKey(equipSlot))
                    {
                        ImGui.BeginTooltip();
                        var itemName = currentGlamourSet.set[equipSlot].glamourCollectible.GetName();
                        ImGui.Text(itemName);
                        ImGui.EndTooltip();
                    }

                    // Reset on right click
                    if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                    {
                        currentGlamourSet.set.Remove(equipSlot);
                    }
                }

                // Set cursor on bottom right to draw Palette Widget button
                ImGui.SameLine();
                ImGui.SetItemAllowOverlap(); // Makes this button take precedence
                ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPos().X + paletteWidgetButtonOffset.X, ImGui.GetCursorPos().Y + paletteWidgetButtonOffset.Y));

                // Draw Palette Widget button
                var paletteButtonColor = equipSlotToPalette[equipSlot].ActiveStain == null ?
                    paletteWidgetButtonDefaultColor : equipSlotToPalette[equipSlot].ActiveStain.VecColor;
                ImGui.PushStyleColor(ImGuiCol.Text, paletteButtonColor);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0, 0, 0, 0));
                //ImGuiComponents.IconButton(FontAwesomeIcon.Square);
                ImGuiComponents.IconButton(FontAwesomeIcon.PaintBrush);
                //ImGuiComponents.IconButton(FontAwesomeIcon.SprayCan);
                //ImGuiComponents.IconButton(FontAwesomeIcon.PaintBrush);
                ImGui.PopStyleColor();
                ImGui.PopStyleColor();

                // Open Palette Widget popup
                if (ImGui.BeginPopupContextItem($"palette-widget##{equipSlot}", ImGuiPopupFlags.MouseButtonLeft))
                {
                    equipSlotToPalette[equipSlot].Draw();
                    ImGui.EndPopup();
                }

                // Interaction with palette button (details / reset)
                if (ImGui.IsItemHovered(ImGuiHoveredFlags.DockHierarchy))
                {
                    hoveredPaletteButton[equipSlot] = true;

                    // Details on hover
                    ImGui.BeginTooltip();
                    var currentStain = equipSlotToPalette[equipSlot].ActiveStain;
                    var stainName = currentStain == null ? "No Dye Selected" : currentStain.Name;
                    ImGui.Text(stainName);
                    ImGui.EndTooltip();

                    // Reset on right click
                    if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                    {
                        equipSlotToPalette[equipSlot].ResetStain();
                    }
                } else
                {
                    hoveredPaletteButton[equipSlot] = false;
                }

                // Refresh Preview/Try-On on dye change
                //if (currentGlamourSet.set.ContainsKey(equipSlot))
                //{
                //    if (equipSlotToPalette[equipSlot].StateChangeFlag)
                //    {
                //        var tryOn = false;
                //        PreviewGlamour(equipSlot, tryOn);
                //        currentGlamourSet.set[equipSlot].stain = equipSlotToPalette[equipSlot].ActiveStain;
                //        equipSlotToPalette[equipSlot].StateChangeFlag = false;
                //    }
                //}
            }
        }
    }

    private void SetEquipSlot(EquipSlot equipSlot)
    {
        activeEquipSlot = equipSlot;
        EventService.Publish<FilterChangeEvent, FilterChangeEventArgs>(new FilterChangeEventArgs());
    }

    private void PreviewGlamour(EquipSlot equipSlot, bool tryOn)
    {
        var collectible = currentGlamourSet.set[equipSlot].glamourCollectible;
        var stain = equipSlotToPalette[equipSlot].ActiveStain;
        var stainId = stain == null ? 0 : stain.RowId;
        Services.GameFunctionsExecutor.PreviewGlamourWithTryOnRestrictions(collectible, stainId, tryOn);
    }

    private Dictionary<EquipSlot, IDalamudTextureWrap> equipSlotIcons = new();
    private void LoadEquipSlotIcons()
    {
        foreach (var equipSlot in equipSlots)
        {
            var equipSlotName = Enum.GetName(typeof(EquipSlot), equipSlot);
            var iconPath = Path.Combine(Services.PluginInterface.AssemblyLocation.Directory?.FullName!, $"Data\\Resources\\{equipSlotName}.png");
            var icon = Services.PluginInterface.UiBuilder.LoadImage(iconPath);
            equipSlotIcons[equipSlot] = icon;
        }
    }

    private void InitializePaletteWidgets()
    {
        foreach (var equipSlot in equipSlots)
        {
            equipSlotToPalette[equipSlot] = new PaletteWidget(equipSlot, EventService);
            hoveredPaletteButton[equipSlot] = false;
        }
    }

    public void ResetPaletteWidgets()
    {
        foreach (var equipSlot in equipSlots)
        {
            equipSlotToPalette[equipSlot].ResetStain();
        }
    }

    public void OnPublish(GlamourSetChangeEventArgs args)
    {
        currentGlamourSet = args.GlamourSet;

        // Reset glamour state. TODO reset Try On
        Services.GameFunctionsExecutor.ResetPreview();

        // Reset palette state
        ResetPaletteWidgets();

        // Preview the selected set
        foreach (var (equipSlot, glamourItem) in args.GlamourSet.set)
        {
            // Set pelette stain state
            equipSlotToPalette[equipSlot].ActiveStain = glamourItem.stain;

            // Preview
            var stainId = glamourItem.stain == null ? 0 : glamourItem.stain.RowId;
            Services.GameFunctionsExecutor.PreviewGlamourWithTryOnRestrictions(glamourItem.glamourCollectible, stainId, false);
        }
    }

    public void OnPublish(GlamourItemChangeEventArgs args)
    {
        // Find out equip slot
        var equipSlot = args.GlamourItem.glamourCollectible.CollectibleKey.item.EquipSlot;

        // Update glamour set state
        currentGlamourSet.set[equipSlot] = args.GlamourItem;

        // Update stain state

        // Preview - this should probably move to another module, but for now doing it here
        //PreviewGlamour(equipSlot, )
    }

    public void OnPublish(DyeChangeEventArgs args)
    {
        if (currentGlamourSet.set.ContainsKey(args.EquipSlot))
        {
            var tryOn = false;
            PreviewGlamour(args.EquipSlot, tryOn);
            currentGlamourSet.set[args.EquipSlot].stain = equipSlotToPalette[args.EquipSlot].ActiveStain;
        }
    }
}

