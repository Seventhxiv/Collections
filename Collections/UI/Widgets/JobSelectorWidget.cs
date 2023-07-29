using ImGuiNET;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Collections;

public class JobSelectorWidget
{
    public Dictionary<ClassJobEntity, bool> classJobFilters;

    private Dictionary<ClassRole, List<ClassJobEntity>> classRoleToClassJob;
    private List<ClassRole> displayOrder = new List<ClassRole>() { ClassRole.Tank, ClassRole.Healer, ClassRole.Melee, ClassRole.Ranged, ClassRole.Caster };

    public JobSelectorWidget()
    {
        var classJobs = Services.ItemManager.classJobs;
        classJobFilters = classJobs.ToDictionary(entry => entry, entry => false);
        classRoleToClassJob = classJobs.GroupBy(entry => entry.GetClassRole()).ToDictionary(entry => entry.Key, entry => entry.ToList());
    }

    public void Draw()
    {
        //ImGui.CollapsingHeader("Equipped By", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.Leaf);
        JobSelector();

        if (ImGui.Button("Enable All"))
        {
            SetAllState(true);
        }
        ImGui.SameLine();
        if (ImGui.Button("Disable All"))
        {
            SetAllState(false);
        }
        ImGui.SameLine();
        if (ImGui.Button("Set Current Job"))
        {
            SetCurrentJob();
        }
    }

    private void JobSelector()
    {
        foreach (var classRole in displayOrder)
        {
            var classJobs = classRoleToClassJob[classRole];
            foreach (var classJob in classJobs)
            {
                var icon = classJob.GetIconLazy();
                if (icon != null)
                {
                    if (UiHelper.ImageToggleButton(icon, new Vector2(icon.Width / 7, icon.Height / 7), classJobFilters[classJob]))
                    {
                        classJobFilters[classJob] = !classJobFilters[classJob];
                    }
                }
                ImGui.SameLine();
            }
            ImGui.Text("");
        }
    }

    private void SetAllState(bool state)
    {
        classJobFilters = classJobFilters.ToDictionary(e => e.Key, e => state);
    }

    public void OnOpen()
    {
        SetCurrentJob();
    }

    private void SetCurrentJob()
    {
        SetAllState(false);

        var classJob = classJobFilters.Where(e => e.Key.RowId == Services.ClientState.LocalPlayer.ClassJob.Id).First().Key;
        classJobFilters[classJob] = true;
    }
}

