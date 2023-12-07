namespace Collections;

public class CollectibleTooltipWidget
{
    private Vector2 iconSize = new(78, 78);
    private Vector2 sourceIconSize = new(20, 20);

    private EventService EventService { get; init; }
    public CollectibleTooltipWidget(EventService eventService)
    {
        EventService = eventService;
    }

    public unsafe void DrawItemTooltip(ICollectible collectible)
    {
        var icon = collectible.GetIconLazy();
        var collectibleKey = collectible.CollectibleKey;

        // Icon
        if (icon != null)
        {
            ImGui.Image(icon.ImGuiHandle, iconSize);
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
            ImGui.Text($"{collectible.Name}");

            // Level + Jobs
            if (collectibleKey is ItemCollectibleKey)
            {
                var item = ((ItemCollectibleKey)collectibleKey).excelRow;
                ImGui.Text($"Lv. {item.LevelEquip}");
                ImGui.Text($"{item.ClassJobCategory.Value.Name}");
            }

            // Marketplace price / Untradeable
            if (collectibleKey is not null && collectibleKey.GetIsTradeable())
            {
                var price = collectibleKey.GetMarketBoardPriceLazy();
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
            UiHelper.IconButtonStateful("obtained-button", FontAwesomeIcon.Check, ref isObtained, ColorsPalette.GREY2, ColorsPalette.WHITE);
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(isObtained ? "Obtained" : "Unobtained");
            }
            ImGui.SameLine();

            // Wish List
            var isWishlist = collectible.IsWishlist();
            if (UiHelper.IconButtonStateful("wishlist-button", FontAwesomeIcon.ShoppingCart, ref isWishlist, ColorsPalette.GREY2, ColorsPalette.YELLOW, isWishlist ? "In Wishlist" : "Add to Wishlist"))
            {
                collectible.SetWishlist(isWishlist);
                EventService.Publish<FilterChangeEvent, FilterChangeEventArgs>(new FilterChangeEventArgs());
            }
            ImGui.SameLine();

            // Favorite
            var isFavorite = collectible.IsFavorite();
            if (UiHelper.IconButtonStateful("favourite-button", FontAwesomeIcon.Star, ref isFavorite, ColorsPalette.GREY2, ColorsPalette.YELLOW, isFavorite ? "Favorite" : "Add to Favorite"))
            {
                collectible.SetFavorite(isFavorite);
                EventService.Publish<FilterChangeEvent, FilterChangeEventArgs>(new FilterChangeEventArgs());
            }

            // Wiki links
            var originalButtonColor = Services.WindowsInitializer.MainWindow.originalButtonColor;
            ImGui.PushStyleColor(ImGuiCol.Button, originalButtonColor);
            if (ImGui.Button("Gamer Escape"))
            {
                collectible.OpenGamerEscape();
            }
            ImGui.PopStyleColor();

            ImGui.EndTable();
        }
        
        if (collectibleKey == null)
        {
            return;
        }

        // Source list
        DrawSources(collectibleKey);
    }

    private int hoveredRowIndex = -1;
    private unsafe void DrawSources(ICollectibleKey CollectibleKey)
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
                            ImGui.Text(costItem.collectibleKey.Name + " x" + costItem.amount);
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
                            ImGui.Image(icon.ImGuiHandle, sourceIconSize);
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
                            ImGui.SetTooltip("Open Link");
                        }

                        // Open link on click
                        if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                        {
                            source.DisplayLocation();
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
