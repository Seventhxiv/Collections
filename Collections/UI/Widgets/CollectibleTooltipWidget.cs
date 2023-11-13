namespace Collections;

public class CollectibleTooltip
{
    public void DrawItemTooltip(ICollectible collectible)
    {
        var icon = collectible.GetIconLazy();
        var CollectibleKey = collectible.CollectibleKey;
        ItemAdapter item = null;
        if (CollectibleKey != null)
        {
            item = CollectibleKey.item;
        }

        // Icon
        if (icon != null)
        {
            ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Width, icon.Height));
            ImGui.SameLine();
        }

        // Top right icons: Obtained / Wish list / Favourite
        if (ImGui.BeginTable("collectible-tooltip-icons", 2, ImGuiTableFlags.None))
        {
            // Stretch to push the icons in 2nd columns to the right
            ImGui.TableSetupColumn("one", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("two", ImGuiTableColumnFlags.None);
            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            // Item name
            ImGui.Text($"{collectible.GetName()}");

            // Level + Jobs
            if (item.IsEquipment)
            {
                ImGui.Text($"Lv. {item.LevelEquip}");
                ImGui.Text($"{item.ClassJobCategory.Value.Name}");
            }

            // Marketplace price / Untradeable
            if (CollectibleKey.GetIsTradeable())
            {
                var price = CollectibleKey.GetMarketBoardPriceLazy();
                if (price != null)
                {
                    ImGui.Text($"Market Price: {price}");
                } else
                {
                    ImGui.Text("Market Price: Fetching...");
                }
            }
            else
            {
                ImGui.Text("Untradeable");
            }

            ImGui.TableNextColumn();

            // Obtained
            var isObtained = collectible.GetIsObtained();
            UiHelper.IconButtonStateful(0, FontAwesomeIcon.Check, ref isObtained, ColorsPalette.GREY2, ColorsPalette.WHITE);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(isObtained ? "Obtained" : "Unobtained");
            }
            ImGui.SameLine();

            // Wish List TODO
            var isWishlisted = collectible.isFavorite;
            UiHelper.IconButtonStateful(1, FontAwesomeIcon.Hourglass, ref isWishlisted, ColorsPalette.GREY2, ColorsPalette.WHITE);
            collectible.isFavorite = isWishlisted;
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(isWishlisted ? "In Wishlist" : "Add to Wishlist");
            }
            ImGui.SameLine();

            // Favorite
            var isFavorite = collectible.isFavorite;
            UiHelper.IconButtonStateful(0, FontAwesomeIcon.Star, ref isFavorite, ColorsPalette.GREY2, ColorsPalette.YELLOW);
            collectible.isFavorite = isFavorite;
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(isFavorite ? "Favorite" : "Add to Favorite");
            }

            // Wiki links
            var originalButtonColor = Services.WindowsInitializer.MainWindow.originalButtonColor;
            ImGui.PushStyleColor(ImGuiCol.Button, originalButtonColor);
            if (ImGui.Button("Gamer Escape"))
            {
                CollectibleKey.OpenGamerEscape();
            }
            ImGui.PopStyleColor();

            ImGui.EndTable();
        }
        
        if (CollectibleKey == null)
        {
            return;
        }

        // Source list
        DrawSources(CollectibleKey);
    }

    private int hoveredRowIndex = -1;
    private unsafe void DrawSources(CollectibleKey CollectibleKey)
    {
        var unlockSources = CollectibleKey.CollectibleSources;
        IDalamudTextureWrap icon;

        // Disable hovering over header
        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, *ImGui.GetStyleColorVec4(ImGuiCol.TableHeaderBg));

        try
        {
            if (ImGui.BeginTable("collectible-sources", 1, ImGuiTableFlags.NoHostExtendX))
            {
                // Header row
                ImGui.TableSetupColumn("Sources", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableHeadersRow();

                for (var i = 0; i < unlockSources.Count; i++)
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();

                    var source = unlockSources[i];

                    // Color Cell if hovered
                    if (hoveredRowIndex == i)
                    {
                        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.GetColorU32(ImGuiCol.ButtonHovered));
                        hoveredRowIndex = -1;
                    }

                    ImGui.BeginGroup();

                    // Source row
                    if (source is ShopCollectibleSource)
                    {
                        ShopCollectibleSource shopSource = (ShopCollectibleSource)source;
                        foreach (var costItem in shopSource.costItems)
                        {
                            icon = costItem.collectibleKey.GetIconLazy();
                            if (icon != null)
                            {
                                ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Width / 3, icon.Height / 3));
                                ImGui.SameLine();
                            }
                            ImGui.Text(costItem.collectibleKey.item.Name + " x" + costItem.amount);
                            ImGui.SameLine();
                        }
                        var npcName = shopSource.ENpcResident != null ? shopSource.ENpcResident.Singular.ToString() : "Unknown NPC";
                        var locationName = shopSource.GetLocationEntry() != null ? shopSource.GetLocationEntry().TerritoryType.PlaceName.Value.Name.ToString() : "Unknown Location";
                        ImGui.Text("at " + npcName + ", " + locationName);
                    }
                    else
                    {
                        icon = source.GetIconLazy();
                        if (icon != null)
                        {
                            ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Width / 3, icon.Height / 3));
                            ImGui.SameLine();
                        }
                        ImGui.Text($"{source.GetName()}");

                    }
                    ImGui.EndGroup();

                    if (source.GetIslocatable())
                    {
                        // Color Cell if hovered
                        if (ImGui.IsItemHovered())
                        {
                            hoveredRowIndex = i;
                            ImGui.SetTooltip("Open Map");
                        }

                        // Open map link on click
                        if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                        {
                            var locationEntry = source.GetLocationEntry();
                            MapFlag.Place(locationEntry.TerritoryType, locationEntry.Xorigin, locationEntry.Yorigin);
                        }
                    }
                }

                ImGui.EndTable();
            }
        } catch (Exception) {
            ImGui.PopStyleColor();
            throw;
        }

        ImGui.PopStyleColor();
    }
}
