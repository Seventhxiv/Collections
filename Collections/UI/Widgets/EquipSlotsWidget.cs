using Dalamud.Interface.Textures;
using System.IO;

namespace Collections;

public class EquipSlotsWidget
{
    public EquipSlot activeEquipSlot { get; set; } = EquipSlot.Body;
    public Dictionary<EquipSlot, PaletteWidget> paletteWidgets = new();

    private Vector2 activeEquipSlotRectSize = new(60.2f, 60.2f);
    private Vector2 equipSlotBackgroundRectSize = new(56, 56);
    private Vector2 paletteWidgetButtonOffset = new(-34, 35);
    private Vector4 paletteWidgetButtonDefaultColor = ColorsPalette.WHITE;

    public GlamourSet currentGlamourSet { get; set; }
    private Dictionary<EquipSlot, bool> hoveredPaletteButton = new();

    private EventService EventService { get; init; }
    private TooltipWidget CollectibleTooltipWidget { get; init; }
    public EquipSlotsWidget(EventService eventService)
    {
        EventService = eventService;
        CollectibleTooltipWidget = new TooltipWidget(EventService);
        LoadEquipSlotIcons();
        InitializePaletteWidgets();
        eventService.Subscribe<GlamourSetChangeEvent, GlamourSetChangeEventArgs>(OnPublish);
        eventService.Subscribe<GlamourItemChangeEvent, GlamourItemChangeEventArgs>(OnPublish);
        eventService.Subscribe<DyeChangeEvent, DyeChangeEventArgs>(OnPublish);
    }

    private void DrawButtons()
    {
        //ImGui.PushStyleColor(ImGuiCol.Button, Services.WindowsInitializer.MainWindow.originalButtonColor);
        //ImGui.Button("Reapply Preview");
        //ImGui.Button("Reset Preview");
        //ImGui.Button("Add to Plate");
        //ImGui.PopStyleColor();
    }

    private void ApplyGlamourSetToPlate()
    {
        Dev.Log("Applying glamour set to plate");
        // TODO indication which items exist in Dresser
        foreach (var (equipSlot, glamourItem) in currentGlamourSet.Items)
        {
            PlatesExecutor.SetPlateItem(glamourItem.GetCollectible().ExcelRow, (byte)glamourItem.StainId);
        }
    }
    public unsafe void Draw()
    {
        DrawButtons();

        var bgColor = *ImGui.GetStyleColorVec4(ImGuiCol.WindowBg);
        foreach (var equipSlot in Services.DataProvider.SupportedEquipSlots)
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

            // Load collectible if set
            IDalamudTextureWrap icon = null;
            GlamourCollectible collectible = null;

            var glamourItem = currentGlamourSet.GetItem(equipSlot);
            if (glamourItem is not null)
            {
                collectible = glamourItem.GetCollectible();
                icon = collectible.GetIconLazy();
            }

            // Draw icon
            if (icon is null)
                icon = equipSlotIcons[equipSlot].GetWrapOrEmpty();

            // Draw equip slot buttons
            if (ImGui.ImageButton(icon.ImGuiHandle, new Vector2(48, 50)))
            {
                SetEquipSlot(equipSlot);
            }

            // Interaction with buttons (details / reset), Item must be set in this slot
            if (ImGui.IsItemHovered() && collectible is not null && !hoveredPaletteButton[equipSlot]) // hoveredPaletteButton is here to take presedence of it over this
            {
                // Details on hover
                ImGui.BeginTooltip();
                CollectibleTooltipWidget.DrawItemTooltip(collectible);
                ImGui.EndTooltip();

                // Reset on right click
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    currentGlamourSet.ClearEquipSlot(equipSlot);
                    Services.PreviewExecutor.ResetSlotPreview(equipSlot);
                }
            }

            // Set cursor on bottom right to draw Palette Widget button
            ImGui.SameLine();
            ImGui.SetItemAllowOverlap(); // Makes this button take precedence
            ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPos().X + paletteWidgetButtonOffset.X, ImGui.GetCursorPos().Y + paletteWidgetButtonOffset.Y));

            // Draw Palette Widget button
            var paletteButtonColor = paletteWidgets[equipSlot].ActiveStain.RowId == 0 ?
                paletteWidgetButtonDefaultColor : paletteWidgets[equipSlot].ActiveStain.VecColor;
            ImGui.PushStyleColor(ImGuiCol.Text, paletteButtonColor);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0, 0, 0, 0));
            ImGuiComponents.IconButton(FontAwesomeIcon.PaintBrush);
            ImGui.PopStyleColor();
            ImGui.PopStyleColor();

            // Open Palette Widget popup
            if (ImGui.BeginPopupContextItem($"palette-widget##{equipSlot}", ImGuiPopupFlags.MouseButtonLeft))
            {
                paletteWidgets[equipSlot].Draw();
                ImGui.EndPopup();
            }

            // Interaction with palette button (details / reset)
            if (ImGui.IsItemHovered(ImGuiHoveredFlags.DockHierarchy))
            {
                hoveredPaletteButton[equipSlot] = true;

                // Details on hover
                ImGui.BeginTooltip();
                var stainName = paletteWidgets[equipSlot].ActiveStain.RowId == 0 ? "No Dye Selected" : paletteWidgets[equipSlot].ActiveStain.Name;
                ImGui.Text(stainName);
                ImGui.EndTooltip();

                // Reset on right click
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    paletteWidgets[equipSlot].ResetStain();
                    EventService.Publish<DyeChangeEvent, DyeChangeEventArgs>(new DyeChangeEventArgs(equipSlot));
                }
            } else
            {
                hoveredPaletteButton[equipSlot] = false;
            }
        }
    }

    private void SetEquipSlot(EquipSlot equipSlot)
    {
        activeEquipSlot = equipSlot;
        EventService.Publish<FilterChangeEvent, FilterChangeEventArgs>(new FilterChangeEventArgs());
    }

    private Dictionary<EquipSlot, ISharedImmediateTexture> equipSlotIcons = new();
    private void LoadEquipSlotIcons()
    {
        foreach (var equipSlot in Services.DataProvider.SupportedEquipSlots)
        {
            var equipSlotName = Enum.GetName(typeof(EquipSlot), equipSlot);
            var iconPath = Path.Combine(Services.PluginInterface.AssemblyLocation.Directory?.FullName!, $"Data\\Resources\\{equipSlotName}.png");
            var icon = Services.TextureProvider.GetFromFile(iconPath);

            equipSlotIcons[equipSlot] = icon;
        }
    }

    private void InitializePaletteWidgets()
    {
        foreach (var equipSlot in Services.DataProvider.SupportedEquipSlots)
        {
            paletteWidgets[equipSlot] = new PaletteWidget(equipSlot, EventService);
            hoveredPaletteButton[equipSlot] = false;
        }
    }

    public void ResetPaletteWidgets()
    {
        foreach (var equipSlot in Services.DataProvider.SupportedEquipSlots)
        {
            paletteWidgets[equipSlot].ResetStain();
        }
    }

    public void OnPublish(GlamourSetChangeEventArgs args)
    {
        // Update currentGlamourSet reference
        currentGlamourSet = args.GlamourSet;

        // Reset palette state
        ResetPaletteWidgets();

        // Set pelette stain state
        foreach (var (equipSlot, glamourItem) in args.GlamourSet.Items)
        {
            paletteWidgets[equipSlot].ActiveStain = glamourItem.GetStain();
        }
    }

    public void OnPublish(GlamourItemChangeEventArgs args)
    {
        // Update current glamour set
        var item = args.Collectible.ExcelRow;
        currentGlamourSet.SetItem(item, paletteWidgets[item.EquipSlot].ActiveStain.RowId);
    }

    public void OnPublish(DyeChangeEventArgs args)
    {
        var glamourItem = currentGlamourSet.GetItem(args.EquipSlot);

        // If Dye changed for empty equip slot - do nothing
        if (glamourItem is null)
        {
            return;
        }

        var equipSlot = args.EquipSlot;

        // Update currentGlamourSet
        currentGlamourSet.GetItem(equipSlot).StainId = paletteWidgets[equipSlot].ActiveStain.RowId;

        // Refresh Preview
        Services.PreviewExecutor.PreviewWithTryOnRestrictions(
            glamourItem.GetCollectible(),
            paletteWidgets[equipSlot].ActiveStain.RowId,
            Services.Configuration.ForceTryOn
            );
    }
}

