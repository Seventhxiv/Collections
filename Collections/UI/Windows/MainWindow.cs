using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Collections;

public class MainWindow : Window, IDisposable
{
    private List<(string name, IDrawable window)> collectionWindows { get; init; }

    public MainWindow() : base(
        "Collections", ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new System.Numerics.Vector2(900, 650),
            MaximumSize = new System.Numerics.Vector2(2000, 1000)
        };

        collectionWindows = new List<(string name, IDrawable window)>()
        {
            ("Glamour", new GlamourWindow()),
            ("Mounts", new CollectionWindow(MountCollectible.GetCollection())),
            ("Minions",  new CollectionWindow(MinionCollectible.GetCollection()))
        };
    }

    public override void OnOpen()
    {
        Task.Run(() =>
        {
            foreach (var (_, window) in collectionWindows)
            {
                window.OnOpen();
            }
        });
    }

    public override void OnClose()
    {
        Services.GameFunctionsExecutor.ResetPreview();
    }

    public void Dispose()
    {
        foreach (var (_, window) in collectionWindows)
        {
            window.Dispose();
        }
    }

    public override void Draw()
    {
        if (ImGui.BeginTabBar("TabBar", ImGuiTabBarFlags.FittingPolicyScroll))
        {

            foreach (var (name, window) in collectionWindows)
            {
                if (ImGui.BeginTabItem($"{name}##{name}"))
                {
                    window.Draw();
                    ImGui.EndTabItem();
                }
            }
        }
        ImGui.EndTabBar();
    }

    public override void PreDraw()
    {
        //ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(20f / 255f, 21f / 255f, 20f / 255f, _alpha));
        //ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 30f);
        //ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding, 30f);
        //ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.085f, 0.080f, 0.085f, 0.866f));
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 1, 1, 0f));
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(4, 3));
    }

    public override void PostDraw()
    {
        //ImGui.PopStyleVar();
        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
    }

}

public interface IDrawable : IDisposable
{
    public void Draw();
    public void OnOpen();
}

