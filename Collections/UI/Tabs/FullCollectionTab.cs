
using System.Collections.Immutable;

namespace Collections;

public class FullCollectionTab : IDrawable
{
    const int ContentFiltersWidth = 17;

    private ContentFiltersWidget ContentFiltersWidget { get; init; }
    private CollectionWidget CollectionWidget { get; init; }
    private EventService EventService { get; init; }
    private decimal? patchSelected = null;
    Dictionary<decimal, string> patches = new Dictionary<decimal, string>{};
    public FullCollectionTab()
    {
        EventService = new EventService();
        filteredCollection = LoadInitialCollection();
        // 
        CollectionWidget = new CollectionWidget(EventService, false, Services.DataProvider.GetCollection<MinionCollectible>().First().GetSortOptions());
        ContentFiltersWidget = new ContentFiltersWidget(EventService, 1, filteredCollection);
        EventService.Subscribe<FilterChangeEvent, FilterChangeEventArgs>(OnPublish);
        ApplyFilters();
    }

    public void Draw()
    {
        var textBaseWidth = ImGui.CalcTextSize("A").X;
        // Filters
        ImGui.BeginGroup();
        ImGui.SetNextItemWidth(textBaseWidth * ContentFiltersWidth);
        if (ImGui.BeginCombo($"##filterByPatchDropdown", patchSelected == null ? "Select Patch" : patches[patchSelected.Value], ImGuiComboFlags.HeightRegular))
        {
            if (ImGui.RadioButton("View All Items", patchSelected == null))
            {
                patchSelected = null;
                ApplyFilters();
            }
            foreach (var (patch, displayName) in patches)
            {
                bool selected = patchSelected == patch;
                if (ImGui.RadioButton(displayName, selected))
                {
                    patchSelected = patch;
                    ApplyFilters();
                }
            }
            ImGui.EndCombo();
        }
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
        // Draw collections
        DrawCollections();
        ImGui.EndChild();
    }

    public void DrawCollections()
    {
        CollectionWidget.Draw(filteredCollection, enableFilters: true);
    }

    public void OnPublish(FilterChangeEventArgs args)
    {
        ApplyFilters();
    }

    private List<ICollectible> filteredCollection { get; set; }
    private void ApplyFilters()
    {
        // Not going to store 2 lists. Just pull new one on change.
        var contentFilters = ContentFiltersWidget.Filters.Where(d => d.Value).Select(d => d.Key);
        filteredCollection = CollectionWidget.PageSortOption.SortCollection(LoadInitialCollection())
            .Where(c => c.CollectibleKey is not null)
            .Where(c => !contentFilters.Any() || (
                contentFilters.Intersect(c.CollectibleKey.SourceCategories).Any() &&
                // Don't include collectibles that don't have a source populated
                c.CollectibleKey.SourceCategories.Count != 0))
            .Where(c => (patchSelected == null) || (c.PatchAdded == (decimal)patchSelected))
            .Where(c => !CollectionWidget.IsFiltered(c))
            .ToList();
    }

    private List<ICollectible> LoadInitialCollection()
    {
        return Services.DataProvider.GetCollections().Values.Aggregate((full, current) => [.. full, .. current]);
    }

    public void OnOpen()
    {
        Dev.Log();

        Task.Run(() =>
        {
            foreach (var collectible in filteredCollection)
            {
                collectible.UpdateObtainedState();
                patches[collectible.PatchAdded] = collectible.GetDisplayPatch();
            }
            patches = patches.AsParallel().OrderByDescending((p) => p.Key).ToDictionary();
        });
    }

    public void OnClose()
    {
    }

    public void Dispose()
    {
    }
}

