using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Collections;

public class MainWindow : Window, IDisposable
{
    public string? forceInstanceTab = null;
    public Vector4 originalButtonColor { get; set;}
    private List<(string name, IDrawable window)> collectionTabs { get; init; }

    public MainWindow() : base(
        "Collections", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoCollapse)
    {
        SetOriginalColors();
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(500, 300), // 900, 650
            MaximumSize = new Vector2(2000, 1000) // 2000, 1000
        };

        collectionTabs = new List<(string name, IDrawable window)>()
        {
            ("Glamour", new GlamourTab()),
            ("Mounts", new CollectionTab(Services.DataProvider.GetCollection<MountCollectible>())),
            ("Minions",  new CollectionTab(Services.DataProvider.GetCollection<MinionCollectible>())),
            ("Instance",  new InstanceTab()),
            ("Settings", new SettingsTab()),
        };
    }

    public unsafe void SetOriginalColors()
    {
        originalButtonColor = *ImGui.GetStyleColorVec4(ImGuiCol.Button);
    }

    public override void OnOpen()
    {
        Task.Run(() =>
        {
            foreach (var (_, window) in collectionTabs)
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
        foreach (var (_, window) in collectionTabs)
        {
            window.Dispose();
        }
    }

    public override void Draw()
    {
        if (ImGui.BeginTabBar("TabBar", ImGuiTabBarFlags.FittingPolicyScroll | ImGuiTabBarFlags.AutoSelectNewTabs))
        {
            foreach (var (name, window) in collectionTabs)
            {
                // Use AutoSelectNewTabs to focus the forceInstanceTab
                if (forceInstanceTab == name)
                {
                    forceInstanceTab = null;
                    continue;
                }
                if (ImGui.BeginTabItem($"{name}"))
                {
                    window.Draw();
                    ImGui.EndTabItem();
                }
            }
            ImGui.EndTabBar();
        }
    }

    public override void PreDraw()
    {
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 1, 1, 0f));
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(4, 3));
    }

    public override void PostDraw()
    {
        ImGui.PopStyleColor();
        ImGui.PopStyleVar();
    }

    // Resets preview under certain conditions (GPose open)
    public void OnFrameworkTick(IFramework framework)
    {
        if (IsOpen && GameActionsExecutor.IsInGPose())
        {
            Services.GameFunctionsExecutor.ResetPreview();
        }
    }

}

public interface IDrawable : IDisposable
{
    public void Draw();
    public void OnOpen();
}

