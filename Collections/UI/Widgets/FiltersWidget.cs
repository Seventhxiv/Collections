using ImGuiNET;
using ImGuiScene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Collections;

public class FiltersWidget
{

    public Dictionary<CollectibleSourceType, bool> filters = new();

    private Dictionary<CollectibleSourceType, int> scopedConfig = new();
    private Dictionary<CollectibleSourceType, int> config = new()
    {
            {CollectibleSourceType.Tomestones, 65086},
            //{CollectibleSourceType.WolfMarks, 65014},
            {CollectibleSourceType.Scrips, 65028},
            {CollectibleSourceType.HuntSeals, 65034},
            {CollectibleSourceType.Gil, 65002},
            {CollectibleSourceType.CompanySeals, 65005},
            {CollectibleSourceType.MGP, 65025},
            {CollectibleSourceType.MogStation, MogStationCollectibleSource.iconId},
            //{CollectibleSourceType.CenturioSeals, 65034},
            //{CollectibleSourceType.AlliedSeals, 65024},
            //{CollectibleSourceType.Nuts, 65068},
            {CollectibleSourceType.PvP, 65014}, //61806
            {CollectibleSourceType.Instance, InstanceCollectibleSource.defaultIconId},
            {CollectibleSourceType.Achievement, AchievementCollectibleSource.iconId},
            {CollectibleSourceType.Event, EventCollectibleSource.iconId},
            {CollectibleSourceType.IslandSanctuary, 65096},
            {CollectibleSourceType.DeepDungeon, 61824},
            {CollectibleSourceType.Quest, QuestCollectibleSource.iconId},
            {CollectibleSourceType.BeastTribe, 65016},
            {CollectibleSourceType.TreasureMaps, 61829},
            //{CollectibleSourceType.Other, 61807},
    };

    private int columns = 1;
    public FiltersWidget(int columns = 1, List<ICollectible> collection = null)
    {
        this.columns = columns;
        BuildFiltersIcons(collection);
    }

    public void Draw()
    {
        if (columns == 1)
        {
            DrawFilters();
        }
        else
        {
            if (ImGui.BeginTable($"filters-widget-internal##{columns}", columns)) // ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.SizingFixedFit
            {

                //if (columns == 1)
                //{
                //    ImGui.TableNextColumn();
                //    DrawFilters();
                //}
                //else
                //{
                ImGui.TableNextColumn();
                DrawFilters(0, config.Count / 2);
                ImGui.TableNextColumn();
                DrawFilters(config.Count / 2, 100);
                //}

                ImGui.EndTable();
            }
        }
    }

    public Dictionary<CollectibleSourceType, bool> filtersHovered = new();
    private Dictionary<CollectibleSourceType, TextureWrap> filtersIcons = new();
    private Dictionary<CollectibleSourceType, (Vector2, Vector2)> filtersLocation = new();
    private void DrawFilters(int startIndex = 0, int endIndex = 100)
    {
        var currPost = ImGui.GetCursorPos();
        var diffX = 0f;
        var diffY = 0f;
        var i = 0;
        foreach (var (collectibleSourceType, iconId) in config)
        {
            if (i < startIndex || i >= endIndex)
            {
                i++;
                continue;
            }
            i++;
            if (!filters.ContainsKey(collectibleSourceType))
            {
                filters[collectibleSourceType] = false;
                filtersHovered[collectibleSourceType] = false;
            }
            var icon = filtersIcons[collectibleSourceType];

            // Draw rectangle for bg on active
            //if (filters[collectibleSourceType])
            //ImGui.GetWindowDrawList().AddRectFilled(ImGui.GetItemRectMin(), ImGui.GetItemRectMax(), 0xFF00FF00, 0.5f);

            // Draw over group
            if (filters[collectibleSourceType] || filtersHovered[collectibleSourceType])
            {
                if (filtersLocation.ContainsKey(collectibleSourceType))
                {
                    ImGui.GetWindowDrawList().AddRectFilled(filtersLocation[collectibleSourceType].Item1, filtersLocation[collectibleSourceType].Item2, ImGui.GetColorU32(ImGuiCol.ButtonActive), 30f);
                }
            }

            UiHelper.Group(() =>
            {
                ImGui.Image(icon.ImGuiHandle, new Vector2(25, 25));
                ImGui.SameLine();
                ImGui.Text(Enum.GetName(typeof(CollectibleSourceType), collectibleSourceType));
            }, 20);

            // Store button rec
            //if (!filtersLocation.ContainsKey(collectibleSourceType))
            //{
            //    filtersLocation[collectibleSourceType] = (new Vector2(ImGui.GetItemRectMin().X - diffX, ImGui.GetItemRectMin().Y - diffY), new Vector2(ImGui.GetItemRectMax().X - diffX + 5, ImGui.GetItemRectMax().Y - diffY + 3));
            //}

            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                filters[collectibleSourceType] = !filters[collectibleSourceType];
            }

            if (ImGui.IsItemHovered())
            {
                filtersLocation[collectibleSourceType] = (new Vector2(ImGui.GetItemRectMin().X - diffX, ImGui.GetItemRectMin().Y - diffY), new Vector2(ImGui.GetItemRectMax().X - diffX + 5, ImGui.GetItemRectMax().Y - diffY + 3));
                filtersHovered[collectibleSourceType] = true;
            }
            else
            {
                filtersHovered[collectibleSourceType] = false;
            }

            //if (i % 3 != 0)
            //    ImGui.SameLine();
        }
        ImGui.Text("");
    }

    private void BuildFiltersIcons(List<ICollectible> collection)
    {
        if (collection != null)
        {
            var sourceTypes = collection.Where(c => c.CollectibleUnlockItem != null).SelectMany(c => c.CollectibleUnlockItem.GetSourceTypes()).ToHashSet();

            foreach (var (sourceType, iconId) in config)
            {
                if (!sourceTypes.Contains(sourceType))
                {
                    config.Remove(sourceType);
                }
            }
        }
        filtersIcons = config.AsParallel().ToDictionary(c => c.Key, c => IconHandler.getIcon(c.Value));
    }
}

