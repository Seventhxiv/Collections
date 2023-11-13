namespace Collections;

public class CollectionWidget
{
    private float iconSize = 55f;

    private string hint = "";
    private bool isGlam { get; init; } = false;
    private int iconsPerRow = 1;

    private EventService EventService { get; init; }
    private EquipSlotsWidget EquipSlotsWidget { get; init; }
    private CollectibleTooltip ItemDetailsWidget { get; init; }
    public CollectionWidget(EventService eventService, bool isGlam, EquipSlotsWidget equipSlotsWidget = null)
    {
        EventService = eventService;
        EquipSlotsWidget = equipSlotsWidget;
        this.isGlam = isGlam;
        ItemDetailsWidget = new CollectibleTooltip();
    }

    public int maxDisplayItems = 200;
    private int obtainedState = 0;
    public unsafe void Draw(List<ICollectible> collectionList, bool fillWindow = true)
    {

        var j = 0;
        ImGui.InputTextWithHint($"##changedItemsFilter{collectionList.Count}", "Filter...", ref hint, 40);

        ImGui.RadioButton("All", ref obtainedState, 0);
        ImGui.SameLine();
        ImGui.RadioButton("Obtained", ref obtainedState, 1);
        ImGui.SameLine();
        ImGui.RadioButton("Unobtained", ref obtainedState, 2);

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

        if ((ImGui.GetScrollMaxY() - ImGui.GetScrollY()) / (ImGui.GetScrollMaxY() + 1) < 0.5)
        {
            maxDisplayItems += 30;
        }

        drawItemCount = 0;

        iconsPerRow = (int)Math.Floor(UiHelper.GetLengthToRightOfWindow() / (iconSize + (ImGui.CalcTextSize(" ").X * 4)));

        foreach (var collectible in collectionList)
        {
            // Maintain maximum amount
            j++;
            if (j > maxDisplayItems)
                break;

            // Filter based on hint
            if (hint != "")
            {
                if (!collectible.GetName().Contains(hint, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }
            }

            // Draw item
            DrawItem(collectible);
        }
        if (fillWindow)
            ImGui.EndChild();
    }

    private bool tryOn = false;
    private int drawItemCount = 0;
    private unsafe void DrawItem(ICollectible collectible)
    {
        var obtained = collectible.GetIsObtained();
        if (obtained)
        {
            if (obtainedState == 2)
            {
                return;
            }
        } else
        {
            if (obtainedState == 1)
            {
                return;
            }    
        }
        var icon = collectible.GetIconLazy();

        // Missing:
        //commandeered magitek armor
        //    red baron
        //    marid
        //    midgarsormr
        //    true griffin

        // Display icon
        if (icon is null || collectible.CollectibleKey is null)
        {
            return;
        }

        if (!(drawItemCount % iconsPerRow == 0))
        {
            ImGui.SameLine();
        }
        drawItemCount++;

        ImGui.SetItemAllowOverlap();

        var tint = new Vector4(1f, 1f, 1f, 1f);
        if (!collectible.GetIsObtained())
        {
            tint = ColorsPalette.GREY2;
        }

        if (ImGui.ImageButton(icon.ImGuiHandle, new Vector2(iconSize, iconSize), default(Vector2), new Vector2(1f, 1f), -1, default(Vector4), tint))
        {
            if (isGlam)
            {
                var stain = EquipSlotsWidget.equipSlotToPalette[EquipSlotsWidget.activeEquipSlot].ActiveStain;
                var stainId = stain == null ? 0 : stain.RowId;

                Services.GameFunctionsExecutor.PreviewGlamourWithTryOnRestrictions(collectible, stainId, tryOn);

                var glamourItem = new GlamourItem() { glamourCollectible = (GlamourCollectible)collectible, stain = EquipSlotsWidget.equipSlotToPalette[EquipSlotsWidget.activeEquipSlot].ActiveStain };
                EventService.Publish<GlamourItemChangeEvent, GlamourItemChangeEventArgs>(new GlamourItemChangeEventArgs(glamourItem));
                //GlamourTreeWidget.CurrentGlamourSet.set[EquipSlotsWidget.activeEquipSlot] = new GlamourSet.GlamourItem() { glamourCollectible = (GlamourCollectible)collectible, stain = EquipSlotsWidget.equipSlotToPalette[EquipSlotsWidget.activeEquipSlot].ActiveStain };
            }
        }

        // Details on hover
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ItemDetailsWidget.DrawItemTooltip(collectible);
            ImGui.EndTooltip();
        }

        // Details on click
        if (ImGui.BeginPopupContextItem($"click-glam-item##{collectible.GetName()}", ImGuiPopupFlags.MouseButtonRight))
        {
            ItemDetailsWidget.DrawItemTooltip(collectible);
            ImGui.EndPopup();
        }

        // Favorite
        var isFavorite = collectible.isFavorite;
        UiHelper.IconButtonWithOffset(drawItemCount, FontAwesomeIcon.Star, 40, 0, ref isFavorite);
        collectible.isFavorite = isFavorite;

        // Green checkmark
        //UiHelper.IconButtonWithOffset(drawItemCount, FontAwesomeIcon.Check, 40, 150, ref obtained);
    }
}
