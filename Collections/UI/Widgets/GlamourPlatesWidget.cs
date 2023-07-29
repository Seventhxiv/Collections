using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;

namespace Collections;

public class GlamourPlatesWidget
{
    public unsafe void DrawIfVisible()
    {
        var inspectAddon = (AtkUnitBase*)Services.GameGui.GetAddonByName("MiragePrismMiragePlate", 1);
        if (inspectAddon == null || !inspectAddon->IsVisible)
        {
            return;
        }
        else
        {
            Draw(inspectAddon);
        }
    }

    private unsafe void Draw(AtkUnitBase* inspectAddon)
    {
        AddonPositionHelper.DrawHelper(inspectAddon, "glamaholic-helper-examine", false, this.DrawDropdown);
    }

    private unsafe void DrawDropdown()
    {
        if (ImGui.Button($"Open popup attempt"))
        {
            ImGui.BeginPopup("Plate");
            ImGui.Text("Yo");
            ImGui.EndPopup();
        }
    }
}

