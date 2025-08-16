namespace Collections;

public class CollectionWidget
{
    private float iconSize = 65f;
    private int dynamicScrollingInitialSize = 200;
    private int dynamicScrollingIncrementsPerFrame = 40;
    private int pageSortWidgetWidth = "Sort By".Length * 13;
    private string searchFilter = ""; 
    private CollectibleSortOption PageSortOption { get; set; }
    private bool isGlam { get; init; } = false;
    private EventService EventService { get; init; }
    private TooltipWidget CollectibleTooltipWidget { get; init; }
    public CollectionWidget(EventService eventService, bool isGlam)
    {
        EventService = eventService;
        this.isGlam = isGlam;
        ResetDynamicScrolling();
        CollectibleTooltipWidget = new TooltipWidget(EventService);
        PageSortOption = new CollectibleSortOption("Patch", Comparer<ICollectible>.Create((c1, c2) => c2.PatchAdded.CompareTo(c1.PatchAdded)), false, null);
    }

    private int dynamicScrollingCurrentSize;
    private int obtainedState = 0;
    public unsafe void Draw(List<ICollectible> collectionList, bool expandAvailableRegion = true, bool enableFilters = true, string? collectionHeader = null)
    {
        // Draw filters
        if (enableFilters)
        {
            DrawFilters(collectionList);
            // Sort by user selection
            collectionList = PageSortOption.SortCollection(collectionList).ToList();
        }
        
        // Expand child on remaining window space
        if (expandAvailableRegion)
        {
            var scrollingChildSize = new Vector2(UiHelper.GetLengthToRightOfWindow(), UiHelper.GetLengthToBottomOfWindow());
            ImGui.BeginChild("scroll-area", scrollingChildSize);
        }

        // Dynamic scrolling
        if (EnableDynamicScrolling())
        {
            if (UiHelper.GetScrollPosition() < 0.5)
            {
                dynamicScrollingCurrentSize += dynamicScrollingIncrementsPerFrame;
            }
        }

        drawItemCount = 0;
        var iconsPerRow = GetIconsPerRow();

            // Add header
        if (collectionHeader != null)
        {
            ImGui.Selectable(collectionHeader);
        }

        for (var i = 0; i < collectionList.Count; i++)
        {
            // Dynamic scrolling
            if (EnableDynamicScrolling())
            {
                if (i > dynamicScrollingCurrentSize)
                    break;
            }

            var collectible = collectionList[i];
            var icon = collectible.GetIconLazy();

            if (icon is null)
            {
                continue;
            }

            // Check filters
            if (IsFiltered(collectible))
                continue;

            // Align item rows
            if (iconsPerRow != 0 && !(drawItemCount % iconsPerRow == 0))
            {
                ImGui.SameLine();
            }
            drawItemCount++;

            // Draw item
            DrawItem(collectible);
        }

        if (expandAvailableRegion)
        {
            ImGui.EndChild();
        }
    }

    private void DrawFilters(List<ICollectible> collectionList)
    {
        ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - pageSortWidgetWidth);
        ImGui.InputTextWithHint($"##changedItemsFilter{collectionList.Count}", "Filter...", ref searchFilter, 40);
        // default behavior cuts the dropdown a little bit off.
        ImGui.SameLine(ImGui.GetColumnWidth() - pageSortWidgetWidth + 4, 0);
        DrawSortOptions(collectionList);

        ImGui.Text("Show:");
        ImGui.SameLine();

        if (ImGui.RadioButton("All", ref obtainedState, 0))
            ResetDynamicScrolling();
        ImGui.SameLine();

        if (ImGui.RadioButton("Obtained", ref obtainedState, 1))
            ResetDynamicScrolling();
        ImGui.SameLine();

        if (ImGui.RadioButton("Unobtained", ref obtainedState, 2))
            ResetDynamicScrolling();

        if (isGlam)
        {
            // Preview Button
            if (ImGui.RadioButton("Preview", !Services.Configuration.ForceTryOn))
            {
                Services.Configuration.ForceTryOn = false;
            }
            ImGuiComponents.HelpMarker("Preview items on your character. Resets on window closing.\nDisabled for Mog Station items.");
            ImGui.SameLine();

            // Try On Button
            if (ImGui.RadioButton("Try On", Services.Configuration.ForceTryOn))
            {
                Services.Configuration.ForceTryOn = true;
            }
            ImGui.SameLine();

            // Reset Preview Button
            ImGui.PushStyleColor(ImGuiCol.Button, Services.WindowsInitializer.MainWindow.originalButtonColor);
            if (ImGui.Button("Reset Preview"))
            {
                // Personally, I think it shoud also remove equipped items.
                
                Services.PreviewExecutor.ResetAllPreview();
            }
            ImGui.PopStyleColor();
            ImGui.SameLine();

            // Reapply Preview Button
            ImGui.PushStyleColor(ImGuiCol.Button, Services.WindowsInitializer.MainWindow.originalButtonColor);
            if (ImGui.Button("Reapply Preview"))
            {
                EventService.Publish<ReapplyPreviewEvent, ReapplyPreviewEventArgs>(new ReapplyPreviewEventArgs());
            }
            ImGui.PopStyleColor();
        }
    }

    private unsafe void DrawSortOptions(List<ICollectible> collection)
    {
        List<CollectibleSortOption> sortOptions = [];
        if(collection.Count > 0) sortOptions = collection.First().GetSortOptions();
        ImGui.SetNextItemWidth(pageSortWidgetWidth);
        if (ImGui.BeginCombo($"##sortCollectionDropdown{collection.Count}", "Sort By", ImGuiComboFlags.HeightRegular))
        {
            foreach(var sortOpt in sortOptions)
            {
                bool selected = PageSortOption.Equals(sortOpt);
                if(ImGui.RadioButton(sortOpt.Name, selected))
                {
                    // if user already has clicked on button, swap sort order
                    if(selected)
                    {
                        sortOpt.Reverse = !sortOpt.Reverse;
                    }
                    else
                    {
                        sortOpt.Reverse = false;
                    }
                    PageSortOption = sortOpt;
                    selected = true;
                    ResetDynamicScrolling();
                }
                if(selected)
                {
                    ImGui.SameLine();
                    UiHelper.DisabledIconButton(sortOpt.GetSortIcon(), "");
                }
            }
            ImGui.EndCombo();
        }
    }

    private int drawItemCount = 0;
    private Vector4 defaultTint = new(1f, 1f, 1f, 1f);
    private unsafe void DrawItem(ICollectible collectible)
    {
        // for debouncing, prevents interaction and favorite at the same time.
        bool interact = false;
        bool debounce = false;
        var icon = collectible.GetIconLazy();

        ImGui.SetItemAllowOverlap();

        var tint = collectible.GetIsObtained() ? defaultTint : ColorsPalette.GREY2;

        if (ImGui.ImageButton(icon.GetWrapOrEmpty().Handle, new Vector2(iconSize, iconSize), default, new Vector2(1f, 1f), -1, default, tint))
        {
        }
        if (ImGui.IsItemClicked())
        {
            interact = true;
        }

        // Details on hover
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();

            ImGui.PushStyleColor(ImGuiCol.Text, ColorsPalette.GREY2);
            ImGui.Text("Right Click To Interact");
            ImGui.PopStyleColor();

            CollectibleTooltipWidget.DrawItemTooltip(collectible);
            ImGui.EndTooltip();
        }

        // Details on click
        if (ImGui.BeginPopupContextItem($"click-glam-item##{collectible.Name}", ImGuiPopupFlags.MouseButtonRight))
        {
            CollectibleTooltipWidget.DrawItemTooltip(collectible);
            ImGui.EndPopup();
        }

        // Favorite
        var isFavorite = collectible.IsFavorite();
        UiHelper.IconButtonWithOffset(drawItemCount, FontAwesomeIcon.Star, 33, 0, ref isFavorite, 0.9f);
        if(ImGui.IsItemClicked()) {
            debounce = true;
        }
        if (isFavorite != collectible.IsFavorite())
        {
            collectible.SetFavorite(isFavorite);
            EventService.Publish<FilterChangeEvent, FilterChangeEventArgs>(new FilterChangeEventArgs());
        }

        if(interact && !debounce)
        {
            Dev.Log($"Interacting with {collectible.Name}");
            collectible.Interact();
            if (isGlam)
            {
                Dev.Log("Publishing GlamourItemChangeEvent");
                EventService.Publish<GlamourItemChangeEvent, GlamourItemChangeEventArgs>(new GlamourItemChangeEventArgs((GlamourCollectible)collectible));
            }
        }
        
        // Mimicks the official FFXIV Yellow checkmark1
        var obtained = collectible.GetIsObtained();
        // shadow
        UiHelper.IconButtonWithOffset(drawItemCount, FontAwesomeIcon.Check, 32, -48, ref obtained, 1.1f, new Vector4(1f, .741f, .188f, 1).Darken(.7f), ColorsPalette.BLACK.WithAlpha(0));
        // color
        UiHelper.IconButtonWithOffset(drawItemCount, FontAwesomeIcon.Check, 33, -48, ref obtained, 1.0f, new Vector4(1f, .741f, .188f, 1), ColorsPalette.BLACK.WithAlpha(0));
    }

    private bool EnableDynamicScrolling()
    {
        return searchFilter == "" && obtainedState != 1; // Disable when searching or filtered by Obtained
    }

    public void ResetDynamicScrolling()
    {
        dynamicScrollingCurrentSize = dynamicScrollingInitialSize;
    }

    private int GetIconsPerRow()
    {
        return (int)Math.Floor((UiHelper.GetLengthToRightOfWindow() + UiHelper.UnitWidth() * 3) / (iconSize + (UiHelper.UnitWidth() * 2)));
    }

    private bool IsFiltered(ICollectible collectible)
    {
        // Search filter
        if (searchFilter != "")
            if (!collectible.Name.Contains(searchFilter, StringComparison.CurrentCultureIgnoreCase))
                return true;

        // Obtain state filter
        var obtained = collectible.GetIsObtained();
        if ((obtained && obtainedState == 2) || (!obtained && obtainedState == 1))
            return true;

        // Default
        return false;
    }
}
