using System.Numerics;
using static Collections.UiHelper;

namespace Collections;

public class PaletteWidget
{
    public StainAdapter ActiveStain = null;

    private Vector2 stainButtonSize = new(30, 30);
    private Vector2 stainButtonRectSize = new(35, 35);
    private Vector2 stainButtonRectOffset = new(-3, -3);
    private int stainMaxButtonsPerRow = 9;

    private static readonly Dictionary<int, List<StainAdapter>> StainsDict = Services.DataProvider.stains.GroupBy(s => (int)s.Shade).ToDictionary(s => s.Key, s => s.ToList());

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
        var i = 0;
        foreach (var (shade, stainList) in StainsDict)
        {
            i++;
            for (var j = 0; j < stainList.Count; j++)
            {
                var stain = stainList[j];
                var color = stain.RGBcolor;
                var colorVec = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, 1f);

                if (ActiveStain == stain)
                {
                    var origPos = ImGui.GetCursorPos();
                    ImGui.SetCursorPos(new Vector2(origPos.X + stainButtonRectOffset.X, origPos.Y + stainButtonRectOffset.Y));
                    ImGui.ColorButton(stain.Name, ColorsPalette.YELLOW, ImGuiColorEditFlags.NoTooltip, stainButtonRectSize);
                    ImGui.SetCursorPos(origPos);
                }

                // Draw button
                if (ImGui.ColorButton(stain.Name, colorVec, ImGuiColorEditFlags.NoSidePreview | ImGuiColorEditFlags.NoSmallPreview | ImGuiColorEditFlags.NoTooltip, stainButtonSize))
                {
                    ActiveStain = stain;
                    pickedColor = new Vector3(ActiveStain.RGBcolor.R / 255f, ActiveStain.RGBcolor.G / 255f, ActiveStain.RGBcolor.B / 255f);
                    EventService.Publish<DyeChangeEvent, DyeChangeEventArgs>(new DyeChangeEventArgs(EquipSlot, ActiveStain));
                }

                // Dye name tooltip on hover
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text(stain.Name);
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

            if (newStain is not null && ActiveStain != newStain)
            {
                ActiveStain = newStain;
                EventService.Publish<DyeChangeEvent, DyeChangeEventArgs>(new DyeChangeEventArgs(EquipSlot, ActiveStain));
            }

            //ImGui.Text(color);
            //ImGui.Text(ActiveStain.Name);
        }
    }

    public void ResetStain()
    {
        ActiveStain = null;
    }
}

