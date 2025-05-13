namespace Collections;

public class PaletteWidget
{
    public StainAdapter ActiveStainPrimary = EmptyStain;
    public StainAdapter ActiveStainSecondary = EmptyStain;

    private Vector2 stainButtonSize = new(30, 30);
    private Vector2 stainButtonRectSize = new(35, 35);
    private Vector2 stainButtonRectOffset = new(-3, -3);
    private int stainMaxButtonsPerRow = 9;

    private static readonly Dictionary<int, List<StainAdapter>> StainsByShade = Services.DataProvider.SupportedStains.GroupBy(s => (int)s.Shade).ToDictionary(s => s.Key, s => s.ToList());
    private static readonly StainAdapter EmptyStain = (StainAdapter)ExcelCache<StainAdapter>.GetSheet().GetRow(0)!;

    private EquipSlot EquipSlot { get; init; }
    private EventService EventService { get; init; }
    public PaletteWidget(EquipSlot equipSlot, EventService eventService)
    {
        EquipSlot = equipSlot;
        EventService = eventService;
    }

    public void Draw()
    {
        var cursorPos = ImGui.GetCursorPos();
        ImGui.BeginGroup();
        DrawPalette();
        ImGui.EndGroup();

        //ImGui.SameLine();

        ImGui.SetCursorPos(new Vector2(cursorPos.X + 380, cursorPos.Y));
        DrawColorPicker();
    }


    public void DrawPalette()
    {
        // Help/Tooltips
        ImGui.PushStyleColor(ImGuiCol.Text, ColorsPalette.GREY2);
        ImGui.Text("Left Click for Dye Channel 1, Right Click for Dye Channel 2");
        ImGui.SameLine();
        ImGuiComponents.HelpMarker("Left click to select the closest color match to the highlighted color. \nHold shift and Left click to do the above for dye channel 2");
        ImGui.Separator();
        // Swap dye colors
        if(ImGuiComponents.IconButton(FontAwesomeIcon.DiagramNext))
        {
            var temp = ActiveStainPrimary;
            ActiveStainPrimary = ActiveStainSecondary;
            ActiveStainSecondary = temp;
            EventService.Publish<DyeChangeEvent, DyeChangeEventArgs>(new DyeChangeEventArgs(EquipSlot));
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Swap Dyes");
        }
        ImGui.Separator();
        var i = 0;
        foreach (var (shade, stainList) in StainsByShade)
        {
            i++;
            for (var j = 0; j < stainList.Count; j++)
            {
                var stain = stainList[j];
                var color = stain.RGBcolor;
                var colorVec = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, 1f);
                // Both dye slots set to same color
                if(ActiveStainPrimary.RowId == stain.RowId && ActiveStainSecondary.RowId == stain.RowId)
                {
                    var origPos = ImGui.GetCursorPos();
                    ImGui.SetCursorPos(new Vector2(origPos.X + stainButtonRectOffset.X, origPos.Y + stainButtonRectOffset.Y));
                    ImGui.ColorButton(stain.Name.ToString(), ColorsPalette.WHITE, ImGuiColorEditFlags.NoTooltip, stainButtonRectSize);
                    ImGui.SetCursorPos(origPos);
                }
                else if (ActiveStainPrimary.RowId == stain.RowId)
                {
                    var origPos = ImGui.GetCursorPos();
                    ImGui.SetCursorPos(new Vector2(origPos.X + stainButtonRectOffset.X, origPos.Y + stainButtonRectOffset.Y));
                    ImGui.ColorButton(stain.Name.ToString(), ColorsPalette.YELLOW, ImGuiColorEditFlags.NoTooltip, stainButtonRectSize);
                    ImGui.SetCursorPos(origPos);
                }
                else if (ActiveStainSecondary.RowId == stain.RowId)
                {
                    var origPos = ImGui.GetCursorPos();
                    ImGui.SetCursorPos(new Vector2(origPos.X + stainButtonRectOffset.X, origPos.Y + stainButtonRectOffset.Y));
                    // Bright blue for secondary select
                    ImGui.ColorButton(stain.Name.ToString(), new Vector4(.195f, .964f, .983f ,0), ImGuiColorEditFlags.NoTooltip, stainButtonRectSize);
                    ImGui.SetCursorPos(origPos);
                }

                // Draw button
                if (ImGui.ColorButton(stain.Name.ToString(), colorVec, ImGuiColorEditFlags.NoSidePreview | ImGuiColorEditFlags.NoSmallPreview | ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoTooltip, stainButtonSize))
                {} // KEEP THESE CURLY BRACES HERE
                if(ImGui.IsItemClicked(ImGuiMouseButton.Left))
                {
                    // toggle off if clicked on while active
                    if(stain.RowId == ActiveStainPrimary.RowId)
                    {
                        ActiveStainPrimary = EmptyStain;
                    }
                    else {
                        ActiveStainPrimary = stain;
                    }
                    pickedColor = new Vector3(ActiveStainPrimary.RGBcolor.R / 255f, ActiveStainPrimary.RGBcolor.G / 255f, ActiveStainPrimary.RGBcolor.B / 255f);
                    EventService.Publish<DyeChangeEvent, DyeChangeEventArgs>(new DyeChangeEventArgs(EquipSlot));
                }
                if(ImGui.IsItemClicked(ImGuiMouseButton.Right))
                {
                    // toggle off if clicked on while active
                    if(stain.RowId == ActiveStainSecondary.RowId)
                    {
                        ActiveStainSecondary = EmptyStain;
                    }
                    else {
                        ActiveStainSecondary = stain;
                    }
                    pickedColor = new Vector3(ActiveStainPrimary.RGBcolor.R / 255f, ActiveStainPrimary.RGBcolor.G / 255f, ActiveStainPrimary.RGBcolor.B / 255f);
                    EventService.Publish<DyeChangeEvent, DyeChangeEventArgs>(new DyeChangeEventArgs(EquipSlot));
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(stain.RowId.ToString());
                }

                // Dye name tooltip on hover
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text(stain.Name.ToString());
                    ImGui.EndTooltip();
                }

                //if (pickedStain == stain)
                //{
                //    UiHelper.RectOverLastItem();
                //}

                // Maintain rows
                var rowIndex = stainList.IndexOf(stain);
                var buttonsPerRow = i < 6 ? stainMaxButtonsPerRow : stainMaxButtonsPerRow + i;
                if ((rowIndex != stainList.Count - 1) && ((rowIndex + 1) % buttonsPerRow != 0))
                    ImGui.SameLine();

            }
            ImGui.Separator();
        }
    }

    private Vector3 pickedColor = new(0, 0, 0);
    private void DrawColorPicker()
    {
        ImGui.PushItemWidth(270);
        if (ImGui.ColorPicker3("##color-picker", ref pickedColor, ImGuiColorEditFlags.NoSidePreview | ImGuiColorEditFlags.NoTooltip | ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoOptions))
        {
            // Show original selection
            //var pickedColorVec4 = new Vector4(pickedColor.X, pickedColor.Y, pickedColor.Z, 1);
            //ImGui.ColorButton("color-button", pickedColorVec4);
            
            var newStain = StainColorConverter.FindClosestStain(pickedColor);

            // shift click on the color picker to modify second dye slot.
            if(ImGui.IsKeyDown(ImGuiKey.ModShift))
            {
                if(ActiveStainSecondary.RowId != newStain.RowId)
                {
                    ActiveStainSecondary = newStain;
                    EventService.Publish<DyeChangeEvent, DyeChangeEventArgs>(new DyeChangeEventArgs(EquipSlot));
                }
            }
            else if (ActiveStainPrimary.RowId != newStain.RowId)
            {
                ActiveStainPrimary = newStain;
                EventService.Publish<DyeChangeEvent, DyeChangeEventArgs>(new DyeChangeEventArgs(EquipSlot));
            }
        }
    }

    public void ResetStain()
    {
        ActiveStainPrimary = EmptyStain;
        ActiveStainSecondary = EmptyStain;
    }
}

