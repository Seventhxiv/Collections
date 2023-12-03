namespace Collections;

public class ContentFiltersWidget
{

    public Dictionary<CollectibleSourceCategory, bool> Filters = new();

    private Vector2 iconSize = new(25, 25);
    private const int WidgetWidth = 31;

    private Dictionary<CollectibleSourceCategory, IDalamudTextureWrap> icons = new();
    private Dictionary<CollectibleSourceCategory, int> contentTypesToIconId = new()
    {
            {CollectibleSourceCategory.Gil, 65002},
            {CollectibleSourceCategory.Scrips, 65028},
            {CollectibleSourceCategory.MGP, 65025},
            {CollectibleSourceCategory.PvP, 65014}, // 61806
            {CollectibleSourceCategory.Duty, InstanceCollectibleSource.defaultIconId},
            {CollectibleSourceCategory.Quest, QuestCollectibleSource.iconId},
            {CollectibleSourceCategory.Event, EventCollectibleSource.iconId},
            {CollectibleSourceCategory.Tomestones, 65086},
            {CollectibleSourceCategory.DeepDungeon, 61824},
            {CollectibleSourceCategory.BeastTribes, 65016},
            {CollectibleSourceCategory.MogStation, MogStationCollectibleSource.iconId},
            {CollectibleSourceCategory.Achievement, AchievementCollectibleSource.iconId},
            {CollectibleSourceCategory.CompanySeals, 65005},
            {CollectibleSourceCategory.IslandSanctuary, 65096},
            {CollectibleSourceCategory.HuntSeals, 65034},
            {CollectibleSourceCategory.TreasureHunts, 000115}, //61829
            {CollectibleSourceCategory.Crafting, 62202},

            // Here for reference:
            //{CollectibleSourceType.WolfMarks, 65014},
            //{CollectibleSourceType.CenturioSeals, 65034},
            //{CollectibleSourceType.AlliedSeals, 65024},
            //{CollectibleSourceType.Nuts, 65068},
            //{CollectibleSourceType.Other, 61807},
    };

    private int columns { get; init; }
    private EventService EventService { get; init; }
    public ContentFiltersWidget(EventService eventService, int columns = 1, List<ICollectible> collection = null)
    {
        EventService = eventService;
        this.columns = columns;
        FilterAvailableContentTypes(collection);
        InitializeDefaultFiltersState();
        LoadIcons();
    }

    public void Draw()
    {
        // Invisible button to set width for the table, otherwise it is cutoff by the width of it's cell (which is dictated by EquipSlotWidget above it)
        ImGui.InvisibleButton("invisible", new Vector2(UiHelper.UnitWidth() * WidgetWidth, 0));

        if (ImGui.BeginTable($"content-filters", columns, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY))
        {
            var i = -1;
            foreach (var (collectibleSourceType, iconId) in contentTypesToIconId)
            {
                // Maintain rows / columns
                i++;
                if (i % columns == 0)
                {
                    ImGui.TableNextRow(ImGuiTableRowFlags.None, 0);
                }
                ImGui.TableNextColumn();
                
                // Background color if selected
                if (Filters[collectibleSourceType])
                    ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.GetColorU32(ImGuiCol.ButtonActive));

                // Draw cell
                DrawCell(collectibleSourceType);

            }
            ImGui.EndTable();
        }
    }

    private void DrawCell(CollectibleSourceCategory collectibleSourceCategory)
    {
        // Draw button as a group
        UiHelper.GroupWithMinWidth(() =>
        {
            ImGui.Image(icons[collectibleSourceCategory].ImGuiHandle, iconSize);
            ImGui.SameLine();
            ImGui.Text(collectibleSourceCategory.GetEnumName().ToSentence().TrimStart(' '));
            //ImGui.Text(hint1);
        }, ImGui.GetContentRegionAvail().X);

        // Left click to set filter
        if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
        {
            ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.GetColorU32(ImGuiCol.ButtonActive));
            var currentFilter = Filters[collectibleSourceCategory];
            ResetFilters();
            SetFilter(collectibleSourceCategory, !currentFilter);
        }

        // Right click to remove filter
        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
        {
            SetFilter(collectibleSourceCategory, !Filters[collectibleSourceCategory]);
        }

        // Save hover state
        if (ImGui.IsItemHovered())
        {
            ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.GetColorU32(ImGuiCol.ButtonHovered));
        }
    }

    private void FilterAvailableContentTypes(List<ICollectible> collection)
    {
        if (collection != null)
        {
            var sourceTypes = collection.Where(c => c.CollectibleKey != null).SelectMany(c => c.CollectibleKey.GetSourceTypes()).ToHashSet();

            foreach (var (sourceType, iconId) in contentTypesToIconId)
            {
                if (!sourceTypes.Contains(sourceType))
                {
                    contentTypesToIconId.Remove(sourceType);
                }
            }
        }
    }

    private void SetFilter(CollectibleSourceCategory sourceType, bool value)
    {
        Filters[sourceType] = value;
        EventService.Publish<FilterChangeEvent, FilterChangeEventArgs>(new FilterChangeEventArgs());
    }

    private void LoadIcons()
    {
        icons = contentTypesToIconId.AsParallel().ToDictionary(c => c.Key, c => IconHandler.getIcon(c.Value));
    }

    private void InitializeDefaultFiltersState()
    {
        ResetFilters();
    }

    // Not sending FilterChangeEvent
    private void ResetFilters()
    {
        foreach (var (sourceType, _) in contentTypesToIconId)
        {
            Filters[sourceType] = false;
        }
    }
}

