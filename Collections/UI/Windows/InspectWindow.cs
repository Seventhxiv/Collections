using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace Collections;

public class InspectWindow : Window, IDisposable
{
    private int sourceMinionCount = 0;
    private int sourceMountCount = 0;
    public InspectWindow() : base(
        "Collections - Inspect", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(300, 150),
            MaximumSize = new Vector2(300, 150)
        };

        var mountCollection = Services.DataProvider.GetCollection<MountCollectible>();
        foreach (var mount in mountCollection)
        {
            mount.UpdateObtainedState();
        }

        sourceMountCount = mountCollection.Where(e => e.GetIsObtained()).Count();
    }

    public unsafe void OnFrameworkTick(IFramework framework)
    {
        var inspectAddon = (AtkUnitBase*)Services.GameGui.GetAddonByName("CharacterInspect", 1);
        IsOpen = !(inspectAddon == null || !inspectAddon->IsVisible);
    }

    public void Dispose()
    {
    }
    public override void Draw()
    {
        DrawImpl();
    }

    private string? targetCharName = null;
    private string? targetHomeWorld = null;
    public override void OnOpen()
    {
        var targetData = Services.LodestoneClient.SchedulePopulateCollectionData();
        if (targetData != null)
        {
            targetCharName = targetData.Value.Item1;
            targetHomeWorld = targetData.Value.Item2;
        }
    }

    //public unsafe void DrawIfVisible()
    //{
    //    var inspectAddon = (AtkUnitBase*)Services.GameGui.GetAddonByName("CharacterInspect", 1);
    //    if (inspectAddon == null || !inspectAddon->IsVisible)
    //    {
    //        return;
    //    }
    //    else
    //    {
    //        DrawA();
    //    }
    //}

    private CollectionData targetCollectionData = null;
    private unsafe void DrawImpl()
    {
        var inspectAddon = (AtkUnitBase*)Services.GameGui.GetAddonByName("CharacterInspect", 1);

        //var drawPos = AddonPositionHelper.DrawPosForAddon(inspectAddon, false);
        //if (drawPos == null)
        //{
        //    return;
        //}

        SetWindowPosition(inspectAddon);


        if (targetCharName != null)
        {
            if (Services.LodestoneClient.characterNameToCollectionData.ContainsKey(targetCharName))
            {
                targetCollectionData = Services.LodestoneClient.characterNameToCollectionData[targetCharName];
                if (targetCollectionData != null)
                {
                    DrawWindowContents();
                }
            }
        }
    }

    private unsafe void SetWindowPosition(AtkUnitBase* addon)
    {
        if (addon == null)
        {
            return;
        }

        var root = addon->RootNode;
        if (root == null)
        {
            return;
        }

        var scaledHeight = addon->GetScaledHeight(true);
        var scaledWidth = addon->GetScaledWidth(true);
        var pos = ImGuiHelpers.MainViewport.Pos + new Vector2(addon->X, addon->Y) + (Vector2.UnitX * scaledWidth);
        ImGui.SetWindowPos(pos);
    }

    private void DrawWindowContents()
    {
        if (ImGui.BeginTable("collection-comparison", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.SizingFixedFit))
        {
            ImGui.TableNextColumn();
            ImGui.TableHeader("Collection Type");
            ImGui.TableNextColumn();
            ImGui.TableHeader("YOU");
            ImGui.TableNextColumn();
            ImGui.TableHeader(targetCharName);

            // Mounts
            ImGui.TableNextRow(); // ImGuiTableRowFlags.None, UiHelper.GetLengthToBottomOfWindow()
            ImGui.TableNextColumn();
            ImGui.Text("Mounts");

            ImGui.TableNextColumn();
            ImGui.Text(sourceMountCount.ToString());

            ImGui.TableNextColumn();
            ImGui.Text(targetCollectionData.MountCount.ToString());

            // Minions
            ImGui.TableNextRow(); // ImGuiTableRowFlags.None, UiHelper.GetLengthToBottomOfWindow()
            ImGui.TableNextColumn();
            ImGui.Text("Minions");

            ImGui.TableNextColumn();
            ImGui.Text("?");

            ImGui.TableNextColumn();
            ImGui.Text(targetCollectionData.MinionCount.ToString());

            // Achievements
            ImGui.TableNextRow(); // ImGuiTableRowFlags.None, UiHelper.GetLengthToBottomOfWindow()
            ImGui.TableNextColumn();
            ImGui.Text("Achievements");

            ImGui.TableNextColumn();
            ImGui.Text("?");

            ImGui.TableNextColumn();
            ImGui.Text(targetCollectionData.AchievementCount.ToString());

            ImGui.EndTable();
        }
    }
}

