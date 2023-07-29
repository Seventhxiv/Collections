using ImGuiNET;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Collections;

public class PaletteWidget
{
    public StainEntity pickedStain;
    public bool updatedState;
    //public Dictionary<EquipSlot, StainEntity> equipSlotToStain = new();
    //public EquipSlot? currentSlot;

    public List<StainEntity> stains;
    private Dictionary<int, List<StainEntity>> stainsDict;
    public PaletteWidget()
    {
        stains = Services.ItemManager.stains;
        stainsDict = stains.GroupBy(s => (int)s.Shade).ToDictionary(s => s.Key, s => s.ToList());
        pickedStain = stains.FirstOrDefault();
    }

    public void Draw()
    {
        ImGui.BeginGroup();
        DrawPalette();
        ImGui.EndGroup();

        ImGui.SameLine();

        DrawDyePicker();
    }


    public void DrawPalette()
    {
        foreach (var (shade, stainList) in stainsDict)
        {
            foreach (var stain in stainList)
            {
                var color = stain.RGBcolor;
                var colorVec = new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, 1f);

                if (pickedStain == stain)
                {
                    var origPos = ImGui.GetCursorPos();
                    ImGui.SetCursorPos(new Vector2(origPos.X - 2, origPos.Y - 2));
                    ImGui.ColorButton(stain.Name, UiHelper.colors[UiHelper.CommonColor.yellow], ImGuiColorEditFlags.NoTooltip, new Vector2(27, 27));
                    ImGui.SetCursorPos(origPos);
                }

                // Draw button
                if (ImGui.ColorButton(stain.Name, colorVec, ImGuiColorEditFlags.NoSidePreview | ImGuiColorEditFlags.NoSmallPreview))
                {
                    pickedStain = stain;
                    updatedState = true;
                    pickedColor = new Vector3(pickedStain.RGBcolor.R / 255f, pickedStain.RGBcolor.G / 255f, pickedStain.RGBcolor.B / 255f);
                }

                //if (pickedStain == stain)
                //{
                //    UiHelper.RectOverLastItem();
                //}

                // Maintain rows
                if ((stainList.IndexOf(stain) != stainList.Count - 1) && (stain.SubOrder % 10 != 0))
                    ImGui.SameLine();
            }
        }
    }

    private Vector3 pickedColor = new(0, 0, 0);
    private void DrawDyePicker()
    {
        var origStain = pickedStain;
        if (ImGui.ColorPicker3("##default-glamour-set", ref pickedColor))
        {

            // Show original selection
            var pickedColorVec4 = new Vector4(pickedColor.X, pickedColor.Y, pickedColor.Z, 1);
            ImGui.ColorButton("color-button", pickedColorVec4);

            pickedStain = ColorHelper.FindClosestStain(pickedColor);

            if (origStain != pickedStain)
            {
                updatedState = true;
            }
            //ImGui.Text(color);
            ImGui.Text(pickedStain.Name);
        }
        //ImGui.EndPopup();
        //}
    }
}

