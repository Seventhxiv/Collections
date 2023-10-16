using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Internal;
using ImGuiNET;
using ImGuiScene;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Collections;

public class UiHelper
{
    public static Dictionary<CommonColor, Vector4> colors = new()
    {
        { CommonColor.white, new Vector4(1.000f, 1.000f, 1.000f, 1.000f)},
        { CommonColor.grey, new Vector4(0.378f, 0.378f, 0.352f, 1.000f)},
        { CommonColor.grey2, new Vector4(0.682f, 0.682f, 0.682f, 0.95f)},
        { CommonColor.black, new Vector4(0.000f, 0.000f, 0.000f, 1.000f)},
        { CommonColor.yellow, new Vector4(0.915f, 0.904f, 0.155f, 1f)},
        { CommonColor.darkerYellow, new Vector4(0.821f, 0.788f, 0.082f, 0.706f)},
        { CommonColor.purple, new Vector4(0.306f, 0.143f, 0.532f, 0.811f)},
        { CommonColor.blue, new Vector4(0.344f, 0.436f, 0.960f, 0.786f)},
    };

    public enum CommonColor
    {
        white,
        grey,
        grey2,
        black,
        yellow,
        purple,
        darkerYellow,
        blue,
    }

    public static float GetLengthToBottomOfWindow()
    {
        var cursorY = ImGui.GetCursorPos().Y;
        var windowY = ImGui.GetWindowSize().Y;
        var unitY = ImGui.CalcTextSize(" ").Y;
        return windowY - cursorY - unitY;
    }

    public static float GetLengthToRightOfWindow()
    {
        var cursorX = ImGui.GetCursorPos().X;
        var windowX = ImGui.GetWindowSize().X;
        var unitX = ImGui.CalcTextSize(" ").X;
        return windowX - cursorX - unitX;
    }

    public static void Group(Action draw, int minWidth)
    {
        ImGui.BeginGroup();
        ImGui.InvisibleButton("ignore", new Vector2(minWidth, 1));
        draw();
        ImGui.EndGroup();
    }
    public static void IconButtonWithOffset(int id, FontAwesomeIcon fontAwesomeIcon, int Xoffset, int Yoffset, ref bool state)
    {
        // ID
        ImGui.PushID(id);

        // Handle pos offset
        var previousPos = ImGui.GetCursorPos();
        ImGui.SameLine();
        ImGui.SetItemAllowOverlap(); // Makes this button take precedence
        ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPos().X - Xoffset, ImGui.GetCursorPos().Y - Yoffset));

        // Font
        ImGui.PushStyleColor(ImGuiCol.Text, state ? colors[CommonColor.yellow] : colors[CommonColor.grey]);

        // Button
        if (ImGuiComponents.IconButton(fontAwesomeIcon.ToIconString(), new Vector4(0, 0, 0, 0), new Vector4(0, 0, 0, 0), new Vector4(0, 0, 0, 0)))
        {
            state = !state;
        }

        //if (ImGui.IsItemHovered())
        //{
        //    ImGui.BeginTooltip();
        //    ImGui.Text("Favorite");
        //    ImGui.EndTooltip();
        //}

        // Reset state
        ImGui.SetCursorPos(previousPos);
        ImGui.PopStyleColor();
        ImGui.PopID();
    }
    public static void IconButtonWithPopUpInputText(FontAwesomeIcon fontAwesomeIcon, ref string input, Action<string> onInput)
    {
        if (ImGuiComponents.IconButton(fontAwesomeIcon.ToIconString()))
        {
            input = "";
        }
        if (ImGui.BeginPopupContextItem($"##{fontAwesomeIcon}", ImGuiPopupFlags.MouseButtonLeft))
        {
            if (ImGui.IsKeyPressed(ImGuiKey.Escape))
                ImGui.CloseCurrentPopup();

            if (ImGui.IsWindowAppearing())
                ImGui.SetKeyboardFocusHere();

            var enterPressed = ImGui.InputTextWithHint("##newName", "Enter New Name...", ref input, 50, ImGuiInputTextFlags.EnterReturnsTrue);

            if (enterPressed)
            {
                if (input != "")
                {
                    onInput(input);
                    ImGui.CloseCurrentPopup();
                }
            }

            ImGui.EndPopup();
        }
    }

    public static bool ImageToggleButton(IDalamudTextureWrap icon, Vector2 size, bool state)
    {
        var tintColor = new Vector4(1f, 1f, 1f, 1f);
        if (!state)
        {
            tintColor = new Vector4(0.5f, 0.5f, 0.5f, 0.85f);
        }
        return ImGui.ImageButton(icon.ImGuiHandle, size, Vector2.Zero, Vector2.One, 1, Vector4.Zero, tintColor);
    }


    public static void ItemBox()
    {
        var tabs = new List<string> { "1", "2", "3", "4" };
        var y = 3;
        ImGui.ListBox("x", ref y, tabs.ToArray(), 4);
    }

    public static void scaledText(string text, float scale)
    {
        //ImGui.SetWindowFontScale((float)1.3);
        var old_scale = ImGui.GetFont().Scale;
        ImGui.GetFont().Scale *= scale;
        ImGui.PushFont(ImGui.GetFont());
        ImGui.Text(text);
        ImGui.GetFont().Scale = old_scale;
        ImGui.PopFont();
        //ImGui.SetWindowFontScale(1);
    }

    public static void copyText()
    {
        //if (ImGuiNative.igSelectable_Bool(text.Path, 0, ImGuiSelectableFlags.None, Vector2.Zero) != 0)
        //    ImGuiNative.igSetClipboardText(text.Path);
    }


    public static void RectOverLastItem()
    {
        ImGui.GetWindowDrawList().AddRect(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), 0xFF00FF00, 0.5f);
    }

    public static void CustomToolBarDump()
    {
        //ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 10f);
        //var TabBarPos = new Vector2(ImGui.GetWindowPos().X + 23, ImGui.GetWindowPos().Y - 30);
        //ImGui.SetNextWindowPos(TabBarPos);
        //ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.736f, 0.736f, 0.736f, 0.751f));
        //ImGui.SetNextWindowSize(new Vector2(1100, 7), ImGuiCond.Always);
        //ImGui.Begin("some-tabs", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove); // ImGuiWindowFlags.NoBackground |
        ////ImGui.Text("AAAAAAAAAAAAAAAAAAAAAAAAAAA");
        ////ImGui.GetWindowContentRegionMin();
        ////ImGui.GetWindowDrawList().AddRect(ImGui.GetWindowContentRegionMin(), new System.Numerics.Vector2(ImGui.GetWindowContentRegionMin().X + 200, ImGui.GetWindowContentRegionMin().Y + 200), 0xFF00FF00, 0.5f);
        ////ImGui.GetWindowDrawList().PushClipRect(ImGui.GetWindowContentRegionMin(), new System.Numerics.Vector2(ImGui.GetWindowContentRegionMin().X + 200, ImGui.GetWindowContentRegionMin().Y + 200));
        ////ImGui.SetNextWindowBgAlpha(1f);

        //ImGui.Text("Collections   ");
        //ImGui.SameLine();
        //ImGuiComponents.IconButton(Dalamud.Interface.FontAwesomeIcon.Socks);
        //if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
        //{
        //    //GlamourWindow.Draw();
        //    activeTab = 0;
        //}
        //ImGui.SameLine();
        //ImGuiComponents.IconButton(Dalamud.Interface.FontAwesomeIcon.Car);
        //if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
        //{
        //    //DrawMounts(hint);
        //    activeTab = 1;
        //}
        //ImGui.SameLine();
        //ImGuiComponents.IconButton(Dalamud.Interface.FontAwesomeIcon.WindowClose);
        //if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
        //{
        //    MainWindow.IsOpen = false;
        //}

        ////if (ImGui.IsWindowFocused())
        ////{
        ////    //ImGui.SetWindowFocus("some-tabs");
        ////    ImGui.SetWindowFocus("My Amazing Window");
        ////}

        //ImGui.End();
        //ImGui.PopStyleColor();
        //ImGui.PopStyleVar();
    }
}
