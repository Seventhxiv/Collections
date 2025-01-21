namespace Collections;

public class JobSelectorWidget
{
    public Dictionary<ClassJobAdapter, bool> Filters;

    private const int JobIconScale = 7;
    private const int IconSize = 30;
    private Vector2 overrideItemSpacing = new(2, 1);

    private EventService EventService { get; init; }
    public JobSelectorWidget(EventService eventService)
    {
        EventService = eventService;
        var classJobs = Services.DataProvider.SupportedClassJobs;
        Filters = classJobs.ToDictionary(entry => entry, entry => true);
        classRoleToClassJob = classJobs.GroupBy(entry => entry.ClassRole).ToDictionary(entry => entry.Key, entry => entry.ToList());
    }

    public void Draw()
    {
        // Draw Buttons
        ImGui.PushStyleColor(ImGuiCol.Button, Services.WindowsInitializer.MainWindow.originalButtonColor);
        if (ImGui.Button("Enable All"))
        {
            SetAllState(true, true);
        }
        ImGui.SameLine();
        if (ImGui.Button("Disable All"))
        {
            SetAllState(false, true);
        }
        ImGui.SameLine();
        if (ImGui.Button("Current Job"))
        {
            SetCurrentJob();
        }
        ImGui.PopStyleColor();

        // Draw job icons
        JobSelector();
    }

    private Dictionary<ClassRole, List<ClassJobAdapter>> classRoleToClassJob;
    private List<ClassRole> displayOrder = new List<ClassRole>() { ClassRole.Tank, ClassRole.Healer, ClassRole.Melee, ClassRole.Ranged, ClassRole.Caster };
    private void JobSelector()
    {

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, overrideItemSpacing);
        foreach (var classRole in displayOrder)
        {
            var classRoleJobs = classRoleToClassJob[classRole];
            foreach (var classJob in classRoleJobs)
            {
                var icon = classJob.GetIconLazy();
                if (icon != null)
                {
                    // Button
                    UiHelper.ImageToggleButton(icon, new Vector2(IconSize, IconSize), Filters[classJob]);

                    // Left click - Switch to selection
                    if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                    {
                        var newState = IsAllActive() ? true : !Filters[classJob];
                        SetAllState(false, false);
                        Filters[classJob] = newState;
                        PublishFilterChangeEvent();
                    }

                    // Right click - Toggle selection
                    if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                    {
                        Filters[classJob] = !Filters[classJob];
                        PublishFilterChangeEvent();
                    }
                }
                ImGui.SameLine();
            }
            ImGui.Text("");
        }
        ImGui.PopStyleVar();
    }

    private void SetAllState(bool state, bool publishEvent)
    {
        Filters = Filters.ToDictionary(e => e.Key, e => state);
        if (publishEvent)
            PublishFilterChangeEvent();
    }

    private void SetCurrentJob()
    {
        var matchingClassJob = Filters.Where(e => e.Key.RowId == Services.ClientState.LocalPlayer.ClassJob.RowId);
        if (matchingClassJob.Any())
        {
            SetAllState(false, false);
            Filters[matchingClassJob.First().Key] = true;
            PublishFilterChangeEvent();
        }
    }

    private bool IsAllActive()
    {
        return !Filters.Where(e => e.Value == false).Any();
    }

    private void PublishFilterChangeEvent()
    {
        EventService.Publish<FilterChangeEvent, FilterChangeEventArgs>(new FilterChangeEventArgs());
    }
}

