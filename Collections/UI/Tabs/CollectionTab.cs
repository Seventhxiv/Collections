namespace Collections;

public class CollectionTab : IDrawable
{
    const int ContentFiltersWidth = 17;

    private int collectionSize;
    private int obtainedCount;

    private List<ICollectible> collection { get; init; }
    private ContentFiltersWidget ContentFiltersWidget { get; init; }
    private CollectionWidget CollectionWidget { get; init; }
    private EventService EventService { get; init; }
    public CollectionTab(List<ICollectible> collection)
    {
        EventService = new EventService();
        this.collection = collection;
        collectionSize = collection.Count();

        ContentFiltersWidget = new ContentFiltersWidget(EventService, 1, collection);
        CollectionWidget = new CollectionWidget(EventService, false, collection.Count > 0 ? collection.First().GetSortOptions() : null);
        ApplyFilters();
        EventService.Subscribe<FilterChangeEvent, FilterChangeEventArgs>(OnPublish);
    }

    public virtual void Draw()
    {
        var textBaseWidth = ImGui.CalcTextSize("A").X;

        ImGui.ProgressBar((float)obtainedCount / collectionSize, new(UiHelper.GetLengthToRightOfWindow() - UiHelper.UnitWidth() * 2, UiHelper.UnitHeight() * 1f), $"{obtainedCount}/{collectionSize}");

        // Filters
        ImGui.BeginGroup();
        if (ImGui.BeginTable("filters-widget", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.NoHostExtendX))
        {
            ImGui.TableSetupColumn("Content Filters", ImGuiTableColumnFlags.None, textBaseWidth * ContentFiltersWidth);
            ImGui.TableHeadersRow();

            ImGui.TableNextRow(ImGuiTableRowFlags.None, UiHelper.GetLengthToBottomOfWindow());
            ImGui.TableNextColumn();
            ContentFiltersWidget.Draw();
            ImGui.EndTable();
        }
        ImGui.EndGroup();

        // Glam collection
        ImGui.SameLine();
        ImGui.BeginChild("collection");
        CollectionWidget.Draw(filteredCollection);
        ImGui.EndChild();
    }

    public void OnPublish(FilterChangeEventArgs args)
    {
        ApplyFilters();
    }

    private List<ICollectible> filteredCollection { get; set; }
    private void ApplyFilters()
    {
        var contentFilters = ContentFiltersWidget.Filters.Where(d => d.Value).Select(d => d.Key);
        filteredCollection = CollectionWidget.PageSortOption.SortCollection(collection)
            .Where(c => c.CollectibleKey is not null)
            .Where(c => !contentFilters.Any() || (
                contentFilters.Intersect(c.CollectibleKey.SourceCategories).Any() &&
                // Don't include collectibles that don't have a source populated
                c.CollectibleKey.SourceCategories.Count != 0))
            .Where(c => !CollectionWidget.IsFiltered(c))
            .ToList();
    }

    public void OnOpen()
    {
        Dev.Log();

        Task.Run(() =>
        {
            foreach (var collectible in collection)
            {
                collectible.UpdateObtainedState();
            }
            obtainedCount = collection.Count(e => e.GetIsObtained());
        });
    }

    public void OnClose()
    {
    }

    public void Dispose()
    {
    }
}

