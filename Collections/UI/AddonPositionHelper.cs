using Dalamud.Interface;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using System;
using System.Numerics;

namespace Collections;

public class AddonPositionHelper
{
    internal static unsafe Vector2? DrawPosForAddon(AtkUnitBase* addon, bool right = false)
    {
        if (addon == null)
        {
            return null;
        }

        var root = addon->RootNode;
        if (root == null)
        {
            return null;
        }

        var scaledHeight = addon->GetScaledHeight(true);
        var scaledWidth = addon->GetScaledWidth(true);
        return ImGuiHelpers.MainViewport.Pos + new Vector2(addon->X, addon->Y) + Vector2.UnitY * (scaledHeight) - Vector2.UnitY * ImGui.CalcTextSize("A") * 4;
        //var xModifier = right
        //    ? root->Width * addon->Scale - DropdownWidth()
        //    : 0;

        //return ImGuiHelpers.MainViewport.Pos
        //       + new Vector2(addon->X, addon->Y)
        //       + Vector2.UnitX * xModifier
        //       + Vector2.UnitY * ImGui.CalcTextSize("A") * 50
        //       - Vector2.UnitY * (ImGui.GetStyle().FramePadding.Y + ImGui.GetStyle().FrameBorderSize);
    }

    internal static float DropdownWidth()
    {
        // arrow size is GetFrameHeight
        return (ImGui.CalcTextSize("Collections").X + ImGui.GetStyle().ItemInnerSpacing.X * 2 + ImGui.GetFrameHeight()) * ImGuiHelpers.GlobalScale;

    }
    public static unsafe void DrawHelper(AtkUnitBase* addon, string id, bool right, Action dropdown)
    {
        var drawPos = DrawPosForAddon(addon, right);
        if (drawPos == null)
        {
            return;
        }

        //using (new HelperStyles())
        //{
        // get first frame
        ImGui.SetWindowPos(drawPos.Value, ImGuiCond.Appearing);
        //if (!ImGui.Begin($"##{id}"))
        //{
        //    ImGui.End();
        //    return;
        //}
        ImGui.Begin($"##{id}");
        dropdown();
        //ImGui.Begin($"##{id}", ImGuiWindowFlags.NoBackground, ImGuiWindowFlags.NoMove);
        //ImGui.SetNextItemWidth(DropdownWidth());

        //ImGui.SetWindowPos(drawPos.Value);

        //ImGui.End();
    }
}
