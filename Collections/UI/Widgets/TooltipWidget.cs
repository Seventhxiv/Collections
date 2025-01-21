namespace Collections;

public class TooltipWidget
{
    private Vector2 iconSize = new(82, 82);
    private Vector2 sourceIconSize = new(23, 23);

    private EventService EventService { get; init; }
    public TooltipWidget(EventService eventService)
    {
        EventService = eventService;
    }

    public unsafe void DrawItemTooltip(ICollectible collectible)
    {
        var icon = collectible.GetIconLazy();
        var collectibleKey = collectible.CollectibleKey;
        UiHelper.GroupWithMinWidth(() => { }, UiHelper.UnitWidth() * 52);
        // Icon
        if (icon != null)
        {
            ImGui.Image(icon.GetWrapOrEmpty().ImGuiHandle, iconSize);
            ImGui.SameLine();
        }

        // Description + Top right icons (Obtained / Wish list / Favourite)
        if (ImGui.BeginTable("collectible-tooltip-icons", 2, ImGuiTableFlags.None))
        {
            // Stretch to push the icons in 2nd columns to the right
            ImGui.TableSetupColumn("one", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("two", ImGuiTableColumnFlags.None);
            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            // Item name
            ImGui.Text($"{collectible.GetDisplayName()}");

            // Hints
            DrawHints(collectible);

            // Marketplace price / Untradeable
            if (collectibleKey is not null)
            {
                DrawTradeableHint(collectibleKey);
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

            // Wiki link
            var originalButtonColor = Services.WindowsInitializer.MainWindow.originalButtonColor;
            ImGui.PushStyleColor(ImGuiCol.Button, originalButtonColor);
            if (ImGui.Button("Gamer Escape"))
            {
                collectible.OpenGamerEscape();
            }

            // Copy name
            if (ImGui.Button(" Copy Name "))
            {
                ImGui.SetClipboardText(collectible.Name);
            }
            ImGui.PopStyleColor();

            ImGui.EndTable();
        }

        // Secondary Description
        if (collectible.Description != string.Empty)
        {
            ImGui.PushTextWrapPos(UiHelper.UnitWidth() * 50);//UiHelper.GetLengthToRightOfWindow());
            ImGui.PushStyleColor(ImGuiCol.Text, ColorsPalette.GREY2);
            ImGui.TextUnformatted(collectible.Description);
            ImGui.PopStyleColor();
            ImGui.PopTextWrapPos();
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
        ISharedImmediateTexture icon;

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
                    if (source is ShopSource shopSource)
                    {
                        foreach (var costItem in shopSource.costItems)
                        {
                            icon = costItem.collectibleKey.GetIconLazy();
                            if (icon != null)
                            {
                                ImGui.Image(icon.GetWrapOrEmpty().ImGuiHandle, new Vector2(icon.GetWrapOrEmpty().Width / 3, icon.GetWrapOrEmpty().Height / 3));
                                ImGui.SameLine();
                            }

                            ImGui.AlignTextToFramePadding();
                            ImGui.Text(costItem.collectibleKey.Name + " x" + costItem.amount);
                            ImGui.SameLine();
                        }
                        var npcName = shopSource.ENpcResident != null ? shopSource.ENpcResident.Value.Singular.ToString() : "Unknown NPC";
                        var locationName = shopSource.GetLocationEntry() != null ? shopSource.GetLocationEntry().TerritoryType.PlaceName.Value.Name.ToString() : "Unknown Location";
                        ImGui.AlignTextToFramePadding();
                        ImGui.Text("at " + npcName + ", " + locationName);
                    }
                    else
                    {
                        icon = source.GetIconLazy();
                        if (icon != null)
                        {
                            ImGui.Image(icon.GetWrapOrEmpty().ImGuiHandle, sourceIconSize);
                            ImGui.SameLine();
                        }
                        ImGui.PushTextWrapPos(UiHelper.UnitWidth() * 50);
                        ImGui.AlignTextToFramePadding();
                        ImGui.Text($"{source.GetName()}");
                        ImGui.PopTextWrapPos();

                    }

                    if (source.GetIslocatable())
                    {
                        ImGui.SameLine();
                        ImGuiComponents.IconButton(FontAwesomeIcon.Link);
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
        }
        catch (Exception)
        {
            ImGui.PopStyleColor();
            throw;
        }

        ImGui.PopStyleColor();
    }

    public void DrawHints(ICollectible collectible)
    {
        if (collectible is null)
        {
            ImGui.Text(" ");
        }

        if (collectible.PrimaryHint.Description != string.Empty)
            DrawHintModule(collectible.PrimaryHint);

        if (collectible.SecondaryHint.Description != string.Empty)
            DrawHintModule(collectible.SecondaryHint);
    }

    public void DrawHintModule(HintModule hintmodule)
    {
        if (hintmodule.Icon is not null)
        {
            UiHelper.DisabledIconButton((FontAwesomeIcon)hintmodule.Icon, hintmodule.Description);
        }
        else
        {
            ImGui.Text(hintmodule.Description);
        }
    }

    public unsafe void DrawTradeableHint(ICollectibleKey collectibleKey)
    {
        var tradeability = collectibleKey.GetIsTradeable();
        if (tradeability == Tradeability.Tradeable)
        {
            var price = collectibleKey.GetMarketBoardPriceLazy();
            var priceText = price is not null ? ((int)price).ToString("N0") : "Fetching Price...";
            UiHelper.DisabledIconButton(FontAwesomeIcon.SackDollar, $"{priceText}");
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("Marketboard price");
            }
        }
        else if (tradeability == Tradeability.UntradeableSingle)
        {
            var buttonBaseColor = *ImGui.GetStyleColorVec4(ImGuiCol.Button);
            ImGui.PushStyleColor(ImGuiCol.Text, ColorsPalette.GREY2);
            ImGuiComponents.IconButtonWithText(FontAwesomeIcon.SackXmark, "Untradeable", buttonBaseColor, buttonBaseColor, buttonBaseColor);
            ImGui.PopStyleColor();
        }
    }
}
