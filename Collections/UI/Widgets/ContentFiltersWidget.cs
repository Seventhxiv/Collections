namespace Collections;

public class ContentFiltersWidget
{

    public Dictionary<CollectibleSourceCategory, bool> Filters = new();

    private Vector2 overrideItemSpacing = new(4, 3);
    private Vector2 buttonDrawSourceOffset = new(-3, -1);
    private Vector2 buttonDrawTargetOffset = new(3, 4);
    private float buttonRounding = 30f;
    private Vector2 iconSize = new(25, 25);

    private Dictionary<CollectibleSourceCategory, int> contentTypesToIconId = new()
    {
            {CollectibleSourceCategory.Tomestones, 65086},
            {CollectibleSourceCategory.Scrips, 65028},
            {CollectibleSourceCategory.HuntSeals, 65034},
            {CollectibleSourceCategory.Gil, 65002},
            {CollectibleSourceCategory.CompanySeals, 65005},
            {CollectibleSourceCategory.MGP, 65025},
            {CollectibleSourceCategory.MogStation, MogStationCollectibleSource.iconId},
            {CollectibleSourceCategory.PvP, 65014}, // 61806
            {CollectibleSourceCategory.Instance, InstanceCollectibleSource.defaultIconId},
            {CollectibleSourceCategory.Achievement, AchievementCollectibleSource.iconId},
            {CollectibleSourceCategory.Event, EventCollectibleSource.iconId},
            {CollectibleSourceCategory.IslandSanctuary, 65096},
            {CollectibleSourceCategory.DeepDungeon, 61824},
            {CollectibleSourceCategory.Quest, QuestCollectibleSource.iconId},
            {CollectibleSourceCategory.BeastTribe, 65016},
            {CollectibleSourceCategory.TreasureMaps, 61829},

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
        if (ImGui.BeginTable($"content-filters", columns))
        {
            var entriesPerColumn = contentTypesToIconId.Count / columns;
            for (var i = 0; i < columns; i++)
            { 
                ImGui.TableNextColumn();
                DrawColumn(entriesPerColumn * i, (entriesPerColumn * i) + entriesPerColumn);
            }
            ImGui.EndTable();
        }
    }

    private Dictionary<CollectibleSourceCategory, bool> hoveredFilters = new();
    private Dictionary<CollectibleSourceCategory, IDalamudTextureWrap> icons = new();
    private Dictionary<CollectibleSourceCategory, (Vector2, Vector2)> filtersLocation = new();
    private void DrawColumn(int startIndex = 0, int endIndex = 100)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, overrideItemSpacing);

        var i = 0;

        foreach (var (collectibleSourceType, iconId) in contentTypesToIconId)
        {
            if (i < startIndex || i >= endIndex)
            {
                i++;
                continue;
            }
            else
            {
                i++;
            }

            // Draw background rectangle based on snapshot location
            if (Filters[collectibleSourceType] || hoveredFilters[collectibleSourceType])
            {
                var rectColor = hoveredFilters[collectibleSourceType] ? ImGuiCol.ButtonHovered : ImGuiCol.ButtonActive;
                if (filtersLocation.ContainsKey(collectibleSourceType))
                {
                    ImGui.GetWindowDrawList().AddRectFilled(filtersLocation[collectibleSourceType].Item1, filtersLocation[collectibleSourceType].Item2, ImGui.GetColorU32(rectColor), buttonRounding);
                }
            }

            // Draw button as a group
            UiHelper.GroupWithMinWidth(() =>
            {
                ImGui.Image(icons[collectibleSourceType].ImGuiHandle, iconSize);
                ImGui.SameLine();
                ImGui.Text(Enum.GetName(typeof(CollectibleSourceCategory), collectibleSourceType));
            }, 20);

            // Left click to change filter
            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                var currentFilter = Filters[collectibleSourceType];
                ResetFilters();
                SetFilter(collectibleSourceType, !currentFilter);
            }

            // Right click to add filter
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                SetFilter(collectibleSourceType, !Filters[collectibleSourceType]);
            }

            // Save hover state
            if (ImGui.IsItemHovered())
            {
                hoveredFilters[collectibleSourceType] = true;
            }
            else
            {
                hoveredFilters[collectibleSourceType] = false;
            }

            // Store location for next frame background draw
            if (Filters[collectibleSourceType] || hoveredFilters[collectibleSourceType])
                filtersLocation[collectibleSourceType] = (new Vector2(
                    ImGui.GetItemRectMin().X + buttonDrawSourceOffset.X, ImGui.GetItemRectMin().Y + buttonDrawSourceOffset.Y),
                    new Vector2(ImGui.GetItemRectMax().X + buttonDrawTargetOffset.X, ImGui.GetItemRectMax().Y + buttonDrawTargetOffset.Y));
        }

        ImGui.PopStyleVar();
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
        PublishFilterChangeEvent();
    }

    private void LoadIcons()
    {
        icons = contentTypesToIconId.AsParallel().ToDictionary(c => c.Key, c => IconHandler.getIcon(c.Value));
    }

    private void InitializeDefaultFiltersState()
    {
        ResetFilters();
        ResetHoveredFilters();
    }

    // Not sending FilterChangeEvent on purpose
    private void ResetFilters()
    {
        foreach (var (sourceType, _) in contentTypesToIconId)
        {
            Filters[sourceType] = false;
        }
    }

    private void ResetHoveredFilters()
    {
        foreach (var (sourceType, _) in contentTypesToIconId)
        {
            hoveredFilters[sourceType] = false;
        }
    }

    private void PublishFilterChangeEvent()
    {
        EventService.Publish<FilterChangeEvent, FilterChangeEventArgs>(new FilterChangeEventArgs());
    }
}

