namespace Collections;

public class JobSelectorWidget
{
    public Dictionary<ClassJobAdapter, bool> Filters;

    const int JobIconScale = 7;
    private Vector2 overrideItemSpacing = new(1, 1);

    private EventService EventService { get; init; }
    public JobSelectorWidget(EventService eventService)
    {
        EventService = eventService;
        var classJobs = Services.DataProvider.classJobs;
        Filters = classJobs.ToDictionary(entry => entry, entry => true);
        classRoleToClassJob = classJobs.GroupBy(entry => entry.GetClassRole()).ToDictionary(entry => entry.Key, entry => entry.ToList());
    }

    public void Draw()
    {
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
        if (ImGui.Button("Current Job"))
        {
            SetCurrentJob();
        }
        JobSelector();
    }

    private Dictionary<ClassRole, List<ClassJobAdapter>> classRoleToClassJob;
    private List<ClassRole> displayOrder = new List<ClassRole>() { ClassRole.Tank, ClassRole.Healer, ClassRole.Melee, ClassRole.Ranged, ClassRole.Caster };
    private void JobSelector()
    {
        
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, overrideItemSpacing);
        foreach (var classRole in displayOrder)
        {
            var classJobs = classRoleToClassJob[classRole];
            foreach (var classJob in classJobs)
            {
                var icon = classJob.GetIconLazy();
                if (icon != null)
                {
                    if (UiHelper.ImageToggleButton(icon, new Vector2(icon.Width / JobIconScale, icon.Height / JobIconScale), Filters[classJob]))
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

    private void SetAllState(bool state)
    {
        Filters = Filters.ToDictionary(e => e.Key, e => state);
        PublishFilterChangeEvent();
    }

    public void OnOpen()
    {
    }

    private void SetCurrentJob()
    {
        var matchingClassJob = Filters.Where(e => e.Key.RowId == Services.ClientState.LocalPlayer.ClassJob.Id);
        if (matchingClassJob.Any())
        {
            SetAllState(false);
            Filters[matchingClassJob.First().Key] = true;
            PublishFilterChangeEvent();
        }
    }

    private void PublishFilterChangeEvent()
    {
        EventService.Publish<FilterChangeEvent, FilterChangeEventArgs>(new FilterChangeEventArgs());
    }
}

