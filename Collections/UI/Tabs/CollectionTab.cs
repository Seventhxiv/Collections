namespace Collections;

public class CollectionTab : IDrawable
{
    const int ContentFiltersWidth = 17;

    private List<ICollectible> collection { get; init; }
    private ContentFiltersWidget ContentFiltersWidget { get; init; }
    private CollectionWidget CollectionWidget { get; init; }
    private EventService EventService { get; init; }
    public CollectionTab(List<ICollectible> collection)
    {
        EventService = new EventService();
        this.collection = collection;
        ContentFiltersWidget = new ContentFiltersWidget(EventService, 1, collection);
        CollectionWidget = new CollectionWidget(EventService, false);

        filteredCollection = collection;
        EventService.Subscribe<FilterChangeEvent, FilterChangeEventArgs>(OnPublish);
    }

    public virtual void Draw()
    {
        var textBaseWidth = ImGui.CalcTextSize("A").X;

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
        filteredCollection = collection.AsParallel()
            .Where(c => c.CollectibleKey is not null)
            .Where(c => !contentFilters.Any() || contentFilters.Intersect(c.CollectibleKey.SourceCategories).Any())
            // Order
            .OrderByDescending(c => c.IsFavorite())
            .ThenByDescending(c => c.Name)
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
        });
    }

    public void OnClose()
    {
    }

    public void Dispose()
    {
    }
}

