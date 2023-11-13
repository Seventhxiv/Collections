namespace Collections;

public class GlamourTab : IDrawable
{
    private List<ICollectible> filteredCollection { get; set; }

    private GlamourTreeWidget GlamourTreeWidget { get; init; }
    private JobSelectorWidget JobSelectorWidget { get; init; }
    private ContentFiltersWidget ContentFiltersWidget { get; init; }
    private EquipSlotsWidget EquipSlotsWidget { get; init; }
    private CollectionWidget CollectionWidget { get; init; }
    private EventService EventService { get; init; }
    public GlamourTab()
    {
        EventService = new EventService();
        GlamourTreeWidget = new GlamourTreeWidget(EventService);
        JobSelectorWidget = new JobSelectorWidget(EventService);
        ContentFiltersWidget = new ContentFiltersWidget(EventService, 2);
        EquipSlotsWidget = new EquipSlotsWidget(EventService);
        CollectionWidget = new CollectionWidget(EventService, true, EquipSlotsWidget);

        filteredCollection = Services.DataProvider.GlamourCollection[EquipSlotsWidget.activeEquipSlot];

        EventService.Subscribe<FilterChangeEvent, FilterChangeEventArgs>(OnPublish);
        // Set glamour set TODO - set this based on configuration
        GlamourTreeWidget.SetSelectedGlamourSet(0, 0);
    }

    const int GlamourSetWidth = 17;
    const int SpaceBetweenFilterWidgets = 3;

    public void Draw()
    {
        var textBaseWidth = ImGui.CalcTextSize("A").X;
        var textBaseHeight = ImGui.CalcTextSize("A").Y;

        // Table flags:
        // Borders - display borders
        // NoHostExtendX - makes each table not stretch all the way in X direction, since we're using multiple tables side-by-side this makes sense
        // SizingFixedFit - makes each table borders snap to the content width
        if (ImGui.BeginTable("glam-tree", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.SizingFixedFit))
        {
            ImGui.TableSetupColumn("Glamour Sets", ImGuiTableColumnFlags.None, textBaseWidth * GlamourSetWidth);
            ImGui.TableHeadersRow();

            ImGui.TableNextRow(ImGuiTableRowFlags.None, UiHelper.GetLengthToBottomOfWindow());
            ImGui.TableNextColumn();

            GlamourTreeWidget.Draw();

            ImGui.EndTable();
        }
        ImGui.SameLine();

        // Equip slot buttons
        ImGui.BeginGroup();
        if (ImGui.BeginTable("equip-slots", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.SizingFixedFit))
        {
            ImGui.TableSetupColumn("Equip Slots", ImGuiTableColumnFlags.None); // Not setting width here, allowing equip slot icon size to dictate width
            ImGui.TableHeadersRow();

            ImGui.TableNextRow(ImGuiTableRowFlags.None, UiHelper.GetLengthToBottomOfWindow());
            ImGui.TableNextColumn();
            EquipSlotsWidget.Draw();

            ImGui.EndTable();
        }
        ImGui.EndGroup();
        ImGui.SameLine();

        // Filters
        ImGui.BeginGroup();
        if (ImGui.BeginTable("filters", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.SizingFixedFit))
        {
            ImGui.TableNextColumn();
            ImGui.TableHeader("Equipped By");

            ImGui.TableNextRow(ImGuiTableRowFlags.None, (UiHelper.GetLengthToBottomOfWindow() / 2) - (textBaseHeight * SpaceBetweenFilterWidgets));
            ImGui.TableNextColumn();
            JobSelectorWidget.Draw();

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.TableHeader("Content Filters");

            ImGui.TableNextRow(ImGuiTableRowFlags.None, UiHelper.GetLengthToBottomOfWindow());
            ImGui.TableNextColumn();
            ContentFiltersWidget.Draw();

            ImGui.EndTable();
        }

        ImGui.EndGroup();

        // Glam collection
        ImGui.SameLine();
        ImGui.BeginGroup();

        //Dev.Start();
        CollectionWidget.Draw(filteredCollection);
        //var drawTime = Dev.Stop(false);
        //drawAvg = drawAvg + (drawTime - drawAvg) / drawCount;
        //drawCount++;
        //ImGui.Text(drawAvg.ToString());

        //ImGui.SameLine();
        //if (ImGui.Button("Reset"))
        //{
        //    drawCount = 1;
        //    drawAvg = 0;
        //}
        ImGui.Text("");

        ImGui.EndGroup();
    }

    private double drawCount = 1;
    private double drawAvg = 0;
    public void OnOpen()
    {
        CollectionWidget.maxDisplayItems = 200;

        var equipSlotList = Enum.GetValues(typeof(EquipSlot)).Cast<EquipSlot>().ToList();
        foreach (var equipSlot in equipSlotList)
        {
            if (Services.DataProvider.GlamourCollection.ContainsKey(equipSlot))
            {
                var itemList = Services.DataProvider.GlamourCollection[equipSlot];
                foreach (var item in itemList)
                {
                    item.UpdateObtainedState();
                }
            }
        }

        JobSelectorWidget.OnOpen();
    }

    public void OnPublish(FilterChangeEventArgs args)
    {
        ApplyFilters();
    }

    // Refreshed all filteres (1) Equip slot (2) content type (3) job
    private void ApplyFilters()
    {
        var contentFilters = ContentFiltersWidget.Filters.Where(d => d.Value).Select(d => d.Key);
        var jobFilters = JobSelectorWidget.Filters.Where(d => d.Value).Select(d => d.Key).ToList();

        // (1) Equip Slot filter
        filteredCollection = new List<ICollectible>(Services.DataProvider.GlamourCollection[EquipSlotsWidget.activeEquipSlot]).AsParallel()

        // (2) Content type filters
        .Where(c => c.CollectibleKey is not null)
        .Where(c => !contentFilters.Any() || contentFilters.Intersect(c.CollectibleKey.GetSourceTypes()).Any())

        // (3) job filters
        .Where(c =>
            {
                if (!jobFilters.Any())
                    return true;
                var itemJobs = c.CollectibleKey.item.Jobs;
                foreach (var jobFilter in jobFilters)
                {
                    var jobFilterAbbreviation = jobFilter.Job;
                    //var itemClassJobAbbreviations = classJobCategoryAdapter.GetClassJobAbbreviation();
                    foreach (var itemClassJobAbbreviation in itemJobs)
                    {
                        //var itemClassJobAbbreviationString = Enum.GetName(typeof(ClassJobAbbreviations), itemClassJobAbbreviation);
                        if (itemClassJobAbbreviation == jobFilterAbbreviation)
                        {
                            return true;
                        }
                    }
                }
                return false;
            })
        .ToList();
    }

    public void Dispose()
    {
    }

}

