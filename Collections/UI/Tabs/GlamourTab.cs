
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
        CollectionWidget = new CollectionWidget(EventService, true);

        ApplyFilters();

        EventService.Subscribe<FilterChangeEvent, FilterChangeEventArgs>(OnPublish);
        EventService.Subscribe<GlamourItemChangeEvent, GlamourItemChangeEventArgs>(OnPublish);
        EventService.Subscribe<GlamourSetChangeEvent, GlamourSetChangeEventArgs>(OnPublish);
        EventService.Subscribe<ReapplyPreviewEvent, ReapplyPreviewEventArgs>(OnPublish); 

        // GlamourTreeWidget will always have at least one Directory + Set.
        // Therefore once everything is initialized, set selection to first set (0, 0)
        GlamourTreeWidget.SetSelectedGlamourSet(0, 0, false);
    }

    private const int GlamourSetsWidgetWidth = 17;
    private const int SpaceBetweenFilterWidgets = 3;

    public void Draw()
    {
        Dev.Start();

        if (ImGui.BeginTable("glam-tree", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.SizingFixedFit))
        {
            ImGui.TableSetupColumn("Glamour Sets", ImGuiTableColumnFlags.None, UiHelper.UnitWidth() * GlamourSetsWidgetWidth);
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
        if (ImGui.BeginTable("filters", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.SizingFixedSame))
        {
            ImGui.TableNextColumn();
            ImGui.TableHeader("Equipped By");

            ImGui.TableNextRow(ImGuiTableRowFlags.None, (UiHelper.GetLengthToBottomOfWindow() / 2) - (UiHelper.UnitHeight() * SpaceBetweenFilterWidgets));
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

        CollectionWidget.Draw(filteredCollection);
        //ImGui.Text("");

        ImGui.EndGroup();

        //var drawTime = Dev.Stop(false);
        //DrawTimer(drawTime);
    }

    private double drawCount = 1;
    private double drawAvg = 0;
    private void DrawTimer(double drawTime)
    {
        drawAvg = drawAvg + (drawTime - drawAvg) / drawCount;
        drawCount++;
        ImGui.Text(drawAvg.ToString());

        ImGui.SameLine();
        if (ImGui.Button("Reset"))
        {
            drawCount = 1;
            drawAvg = 0;
        }

        if (drawCount > 1000)
        {
            drawCount = 1;
            drawAvg = 0;
        }
        ImGui.Text(" ");
    }

    public void OnOpen()
    {
        Dev.Log();

        CollectionWidget.ResetDynamicScrolling();

        Task.Run(() =>
        {
            foreach (var collectible in Services.DataProvider.GetCollection<GlamourCollectible>())
            {
                collectible.UpdateObtainedState();
            }
        });
    }

    private void ApplyFilters()
    {
        // Refresh dynamic scrolling
        CollectionWidget.ResetDynamicScrolling();

        // Refresh all filteres (1) Equip slot (2) content type (3) job
        var contentFilters = ContentFiltersWidget.Filters.Where(d => d.Value).Select(d => d.Key);
        var jobFilters = JobSelectorWidget.Filters.Where(d => d.Value).Select(d => d.Key).ToList();

        // (1) Equip Slot filter
        filteredCollection = Services.DataProvider.GetCollection<GlamourCollectible>()
            .Where(c => c.CollectibleKey.item.EquipSlot == EquipSlotsWidget.activeEquipSlot).AsParallel()

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

        // Order
        .OrderByDescending(c => c.IsFavorite())
        .ThenByDescending(c => c.CollectibleKey.item.LevelEquip)
        .ThenByDescending(c => c.Name)
        .ToList();
    }

    public void OnPublish(GlamourItemChangeEventArgs args)
    {
        var equipSlot = args.Collectible.CollectibleKey.item.EquipSlot;
        var stainId = EquipSlotsWidget.paletteWidgets[equipSlot].ActiveStain.RowId;
        Services.PreviewExecutor.PreviewWithTryOnRestrictions(args.Collectible, stainId, Services.Configuration.ForceTryOn);
    }

    public void OnPublish(GlamourSetChangeEventArgs args)
    {
        // On some events (during initializing) we're suppressing preview
        if (!args.Preview)
            return;

        // Reset glamour state. TODO reset Try On
        Services.PreviewExecutor.ResetAllPreview();

        // Preview the selected set
        foreach (var (equipSlot, glamourItem) in args.GlamourSet.Items)
        {
            Services.PreviewExecutor.PreviewWithTryOnRestrictions(glamourItem.GetCollectible(), glamourItem.StainId, false);
        }
    }

    public void OnPublish(FilterChangeEventArgs args)
    {
        ApplyFilters();
    }

    public void OnPublish(ReapplyPreviewEventArgs args)
    {
        Services.PreviewExecutor.ResetAllPreview();
        foreach (var (equipSlot, glamourItem) in EquipSlotsWidget.currentGlamourSet.Items)
        {
            var collectible = CollectibleCache<GlamourCollectible, ItemAdapter>.Instance.GetObject(glamourItem.ItemId);
            Services.PreviewExecutor.PreviewWithTryOnRestrictions(collectible, glamourItem.StainId, Services.Configuration.ForceTryOn);
        }
    }

    public void Dispose()
    {
    }

    public void OnClose()
    {
        GlamourTreeWidget.SaveGlamourTreeToConfiguration();
    }
}

