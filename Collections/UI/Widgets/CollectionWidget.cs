using Collections.Types;
using Dalamud.Interface;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using static Collections.UiHelper;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace Collections;

public class CollectionWidget : IDisposable
{
    private string hint = "";
    private bool isGlam { get; init; } = false;
    private float iconSize = 80f;
    private int iconsPerRow = 1;

    private PaletteWidget PaletteWidget { get; init; }
    private GlamourTreeWidget GlamourTreeWidget { get; init; }
    private JobSelectorWidget JobSelectorWidget { get; init; }
    private FiltersWidget FiltersWidget { get; init; }
    private EquipSlotsWidget EquipSlotsWidget { get; init; }
    public CollectionWidget(FiltersWidget filtersWidget, PaletteWidget paletteWidget = null,
        GlamourTreeWidget glamourTreeWidget = null, JobSelectorWidget jobSelectorWidget = null, EquipSlotsWidget equipSlotsWidget = null)
    {
        FiltersWidget = filtersWidget;
        PaletteWidget = paletteWidget;
        GlamourTreeWidget = glamourTreeWidget;
        JobSelectorWidget = jobSelectorWidget;
        EquipSlotsWidget = equipSlotsWidget;
        if (glamourTreeWidget != null && paletteWidget != null && jobSelectorWidget != null && equipSlotsWidget != null)
        {
            isGlam = true;
        }
    }

    public void Dispose()
    {
    }

    public unsafe void Draw(List<ICollectible> collectionList, bool fillWindow = true, int maxItems = 20000)
    {

        var j = 0;
        var activeFilters = FiltersWidget.filters.Where(d => d.Value).Select(d => d.Key);
        var activeJobFilters = new List<ClassJobEntity>();
        if (isGlam)
            activeJobFilters = JobSelectorWidget.classJobFilters.Where(d => d.Value).Select(d => d.Key).ToList();
        ImGui.InputTextWithHint($"##changedItemsFilter{collectionList.Count}", "Filter...", ref hint, 40);

        if (isGlam)
        {
            if (ImGui.RadioButton("Preview", tryOn == false))
            {
                tryOn = false;
            }
            ImGui.SameLine();
            ImGui.TextDisabled("(?)");
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text("Preview items ingame.");
                ImGui.Text("Not available for Mog Station items.");
                ImGui.Text("Appearance will be reset when closing this window.");
                ImGui.EndTooltip();
            }
            ImGui.SameLine();
            if (ImGui.RadioButton("Fitting Room", tryOn == true))
            {
                tryOn = true;
            }
            ImGui.SameLine();
            if (ImGui.Button("Reset Preview"))
            {
                Services.GameFunctionsExecutor.ResetPreview();
            }
        }

        //ImGui.BeginChildFrame(1, new System.Numerics.Vector2(500, 500));
        //var collectionHeight = ImGui.GetWindowSize().Y - ImGui.CalcTextSize(" ").Y * 3.5f;

        if (fillWindow)
        {
            var scrollingChildSize = new Vector2(UiHelper.GetLengthToRightOfWindow(), UiHelper.GetLengthToBottomOfWindow());
            //scrollingChildSize = new Vector2(0, 0);
            ImGui.BeginChild("scrolling", scrollingChildSize);
        }
        drawItemCount = 0;

        iconsPerRow = (int)Math.Floor(UiHelper.GetLengthToRightOfWindow() / (iconSize + (ImGui.CalcTextSize(" ").X * 4)));

        foreach (var collectible in collectionList)
        {
            // Maintain maximum amount
            j++;
            //if (j > maxItems)
            //{
            //    break;
            //}

            // Filter based on hint
            if (hint != "")
            {
                if (!collectible.GetName().Contains(hint, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }
            }

            // Get CollectibleUnlockItem, and underlying Item
            var CollectibleUnlockItem = collectible.CollectibleUnlockItem;

            // Filter based on source
            var matchedFilter = true;
            if (activeFilters.Any())
            {
                if (CollectibleUnlockItem != null)
                {
                    matchedFilter = activeFilters.Intersect(CollectibleUnlockItem.GetSourceTypes()).Any();
                }
                else
                {
                    continue;
                }
            }
            //var matchedFilter = true;
            if (!matchedFilter)
                continue;

            // Filter based on job
            if (activeJobFilters.Any())
            {
                matchedFilter = false;
                var classJobCategory = CollectibleUnlockItem.item.ClassJobCategory.Value;
                foreach (var jobFilter in activeJobFilters)
                {
                    if ((bool)classJobCategory.GetType().GetProperty(jobFilter.Abbreviation).GetValue(classJobCategory))
                    {
                        matchedFilter = true;
                    }
                }
            }
            if (!matchedFilter)
                continue;

            //ImGui.BeginGroup();
            DrawItem(collectible);
            //ImGui.EndGroup();
            //ImGui.Separator();
        }
        if (fillWindow)
            ImGui.EndChild();
        //ImGui.EndChildFrame();
    }

    private bool tryOn = false;
    private int drawItemCount = 0;
    private unsafe void DrawItem(ICollectible collectible)
    {
        var icon = collectible.GetIconLazy();
        Item item = null;
        if (collectible.CollectibleUnlockItem != null)
        {
            item = collectible.CollectibleUnlockItem.item;
        }
        // Missing:
        //commandeered magitek armor
        //    red baron
        //    marid
        //    midgarsormr
        //    true griffin

        // Display icon
        if (icon != null)
        {

            //Dev.Log(UiHelper.GetLengthToRightOfWindow().ToString() + ", " + iconSize.ToString());

            //var iconsPerRow = Math.Floor(UiHelper.GetLengthToRightOfWindow() / iconSize);
            //Dev.Log(iconsPerRow.ToString());
            if (!(drawItemCount % iconsPerRow == 0))
            {
                ImGui.SameLine();
            }
            //iconSize.great
            //if (UiHelper.GetLengthToRightOfWindow() > iconSize)
            //{
            //    ImGui.SameLine();
            //}
            //else
            //{
            //    Dev.Log(drawItemCount.ToString());
            //}
            //if (drawItemCount == 3)
            //{
            //    Dev.Log(UiHelper.GetLengthToRightOfWindow().ToString() + ", " + iconSize.ToString());
            //}
            drawItemCount++;

            ImGui.SetItemAllowOverlap();

            var tint = new Vector4(1f, 1f, 1f, 1f);
            if (!collectible.GetIsObtained())
            {
                tint = colors[CommonColor.grey2];
            }

            if (ImGui.ImageButton(icon.ImGuiHandle, new Vector2(iconSize, iconSize), default(Vector2), new Vector2(1f, 1f), -1, default(Vector4), tint))
            {
                if (isGlam)
                {
                    var stainId = EquipSlotsWidget.equipSlotToPalette[EquipSlotsWidget.activeEquipSlot].pickedStain.RowId;
                    if (tryOn || collectible.CollectibleUnlockItem.GetSourceTypes().Contains(CollectibleSourceType.MogStation))
                    {
                        Services.GameFunctionsExecutor.TryOn(item.RowId, stainId);
                    }
                    else
                    {
                        Services.GameFunctionsExecutor.ChangeEquip(item, (int)stainId);
                        GlamourTreeWidget.CurrentGlamourSet.set[EquipSlotsWidget.activeEquipSlot] = new GlamourSet.GlamourItem() { item = item, icon = icon, stain = EquipSlotsWidget.equipSlotToPalette[EquipSlotsWidget.activeEquipSlot].pickedStain };
                    }
                }
            }
        }


        // Details on hover
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            DrawItemDetails(collectible);
            ImGui.EndTooltip();
        }

        // Details on click
        if (ImGui.BeginPopupContextItem($"click-glam-item##{collectible.GetName()}", ImGuiPopupFlags.MouseButtonRight))
        {
            DrawItemDetails(collectible);
            ImGui.EndPopup();
        }

        var isFavorite = collectible.isFavorite;
        UiHelper.IconButtonWithOffset(drawItemCount, FontAwesomeIcon.Star, 40, 0, ref isFavorite);
        collectible.isFavorite = isFavorite;
    }

    private int selectedX = -1;
    private void DrawItemDetails(ICollectible collectible)
    {
        var icon = collectible.GetIconLazy();
        var CollectibleUnlockItem = collectible.CollectibleUnlockItem;
        Item item = null;
        if (CollectibleUnlockItem != null)
        {
            item = CollectibleUnlockItem.item;
        }

        var popStyleColor = false;

        // Item Icon
        if (icon != null)
        {
            ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Width, icon.Height));
            ImGui.SameLine();
        }

        ImGui.BeginGroup();
        // Item name
        ImGui.Text($"{collectible.GetName()}");

        // Jobs
        if (isGlam)
        {
            ImGui.Text($"Lv. {item.LevelEquip}");
            ImGui.Text($"{item.ClassJobCategory.Value.Name}");
        }

        // Favourite
        ImGui.PushStyleColor(ImGuiCol.Text, collectible.isFavorite ? colors[CommonColor.yellow] : colors[CommonColor.grey]);
        ImGui.Text("Favourite");
        ImGui.PopStyleColor();

        // Obtained
        if (collectible.GetIsObtained())
        {
            ImGui.Text($"Obtained");
        }
        else
        {
            ImGui.Text($"Not Obtained");
        }


        ImGui.EndGroup();

        if (CollectibleUnlockItem == null)
        {
            return;
        }

        // External link
        if (ImGui.Button("Open Gamer Escape##{item.RowId}"))
        {
            CollectibleUnlockItem.OpenGamerEscape();
        }

        // Shops
        var unlockSources = CollectibleUnlockItem.CollectibleSources;
        var x = 0u;
        ImGui.GetTextLineHeightWithSpacing();
        var sourceLineSize = new Vector2(ImGui.CalcTextSize("a").X * 60, ImGui.GetTextLineHeightWithSpacing() * 1.3f);
        foreach (var source in unlockSources)
        {
            x++;
            if (selectedX == x)
            {
                ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.306f, 0.143f, 0.532f, 0.811f));
                selectedX = -1;
                popStyleColor = true;
            }
            ImGui.BeginChild($"item-source##{CollectibleUnlockItem.item.RowId}{x}", sourceLineSize, true, ImGuiWindowFlags.NoScrollbar);
            //ImGui.BeginChildFrame(x, sourceLineSize);
            //ImGui.BeginGroup();
            if (source is ShopCollectibleSource)
            {
                ShopCollectibleSource shopSource = (ShopCollectibleSource)source;
                foreach (var costItem in shopSource.costItems)
                {
                    icon = costItem.CollectibleUnlockItem.GetIconLazy();
                    if (icon != null)
                    {
                        ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Width / 3, icon.Height / 3));
                        ImGui.SameLine();
                    }
                    ImGui.Text(costItem.CollectibleUnlockItem.item.Name + " x" + costItem.amount);
                    ImGui.SameLine();
                }
                var locationEntry = shopSource.GetLocationEntry();
                var npcName = shopSource.ENpcResident != null ? shopSource.ENpcResident.Singular.ToString() : "Unknown NPC";
                var locationName = shopSource.GetLocationEntry() != null ? shopSource.GetLocationEntry().TerritoryType.PlaceName.Value.Name.ToString() : "Unknown Location";
                ImGui.Text(" at " + npcName + ", " + locationName);
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
            ImGui.EndChild();
            //ImGui.EndChildFrame();
            //ImGui.EndGroup();
            if (popStyleColor)
            {
                ImGui.PopStyleColor();
                popStyleColor = false;
            }
            //ImGui.PopStyleColor();
            if (source.GetIslocatable())
            {
                // Open map link on click
                if (ImGui.IsItemHovered())
                {
                    selectedX = (int)x;
                    ImGui.SetTooltip("Open map link");
                }

                if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                {
                    var locationEntry = source.GetLocationEntry();
                    MapFlag.Place(locationEntry.TerritoryType, locationEntry.Xorigin, locationEntry.Yorigin);
                }
            }
        }

        // Marketplace price
        if (CollectibleUnlockItem.GetIsTradeable())
        {
            var price = CollectibleUnlockItem.GetMarketBoardPriceLazy();
            if (price != null)
            {
                ImGui.Text($"Market Price: {price}");
            }
        }
    }
}
