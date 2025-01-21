using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace Collections;

public class GlamourTreeWidget
{
    private GlamourSet currentGlamourSet;
    private GlamourTree glamourTree = new();
    private IconHandler glamourPlateIconHandler = new IconHandler(000125);
    private IconHandler copyExamineIconHandler = new IconHandler(060642);
    private Vector2 iconSize = new(21, 21);

    private EventService EventService { get; init; }
    public GlamourTreeWidget(EventService eventService)
    {
        EventService = eventService;
        InitializeGlamourTreeFromConfiguration();

        if (glamourTree.Directories.Count == 0)
        {
            AddDirectory("Main");
        }

        if (glamourTree.Directories.First().GlamourSets.Count == 0)
        {
            AddGlamourSet("Default");
        }
    }

    public void Draw()
    {
        // Icon buttons
        DrawIconButtons();

        // Folder tree
        DrawGlamourTree();
    }

    private void DrawIconButtons()
    {
        // Directory icon
        UiHelper.IconButtonWithPopUpInputText(FontAwesomeIcon.Folder, AddDirectory);
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Add Directory");
        }
        ImGui.SameLine();

        // Glamour Set icon
        UiHelper.IconButtonWithPopUpInputText(FontAwesomeIcon.Plus, AddGlamourSet);
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Add Set");
        }
        ImGui.SameLine();

        // Add to Glamour Plate Button
        var icon = glamourPlateIconHandler.GetIconLazy();
        if (icon is not null)
        {
            var isInPlateWindow = PlatesExecutor.IsInPlateWindow();

            if (!isInPlateWindow)
                ImGui.PushStyleColor(ImGuiCol.Text, ColorsPalette.GREY2);

            //ImGui.Text(FontAwesomeIcon.addre.ToIconString());
            //ImGui.SameLine();
            //if (ImGui.ImageButton(icon.ImGuiHandle, iconSize))
            if (ImGuiComponents.IconButton(FontAwesomeIcon.Copy))
            {
                if (isInPlateWindow)
                    ApplyGlamourSetToPlate();
            }
            if (!isInPlateWindow)
                ImGui.PopStyleColor();

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text("Add to Glamour Plate");

                if (!isInPlateWindow)
                {
                    ImGui.TextColored(ColorsPalette.RED, "Only usable in Plate Creation window.");
                }
                ImGui.EndTooltip();
            }
            ImGui.SameLine();
        }

        // Copy from Examine window Button
        icon = copyExamineIconHandler.GetIconLazy();
        if (icon is not null)
        {
            var isInspecting = IsInspecting();

            if (!isInspecting)
                ImGui.PushStyleColor(ImGuiCol.Text, ColorsPalette.GREY2);

            //ImGui.ImageButton(icon.ImGuiHandle, iconSize);
            //UiHelper.InputText("examine-button", isInspecting ? CopyInspectTargetToGlamourSet : null);
            UiHelper.IconButtonWithPopUpInputText(FontAwesomeIcon.ArrowCircleDown, isInspecting ? CopyInspectTargetToGlamourSet : null);

            if (!isInspecting)
                ImGui.PopStyleColor();

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text("Copy from Examine window");

                if (!isInspecting)
                {
                    ImGui.TextColored(ColorsPalette.RED, "Only usable in Examine window.");
                }
                ImGui.EndTooltip();
            }
        }
    }

    private (int directory, int glamourSet) selected;
    private (int directory, int glamourSet) dropSource = (-1, -1);
    private void DrawGlamourTree()
    {

        for (var n = 0; n < glamourTree.Directories.Count; n++)
        {
            var directory = glamourTree.Directories[n];

            var treeFlags = ImGuiTreeNodeFlags.None | ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.SpanAvailWidth;

            // Directory tree node
            if (ImGui.TreeNodeEx($"{directory.Name}##{n}", treeFlags))
            {
                // Directory - context menu
                if (ImGui.BeginPopupContextItem())
                {
                    ImGui.Button($"Rename##directory-rename{n}");
                    UiHelper.InputText($"Rename##directory-rename{n}", (name) => RenameDirectory(n, name));

                    if (ImGui.Button($"Delete##directory-delete{n}"))
                    {
                        DeleteDirectory(n);
                    }
                    ImGui.EndPopup();
                }

                // Directory - drop target
                if (ImGui.BeginDragDropTarget())
                {
                    ImGui.Separator();
                    if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                    {
                        MoveGlamourSet(n, 0);
                    }
                    ImGui.EndDragDropTarget();
                }

                // Glamour Sets
                for (var k = 0; k < directory.GlamourSets.Count; k++)
                {
                    var glamourSet = directory.GlamourSets[k];

                    // Selectable Glamour Set
                    var textBaseWidth = ImGui.CalcTextSize("A").X;
                    ImGui.SetNextItemWidth(textBaseWidth * 5);
                    if (ImGui.Selectable($"   {glamourSet.Name}##{n}{k}", selected == (n, k), ImGuiSelectableFlags.None))
                    {
                        // Change selected set if (actually) clicked on a different set
                        if (selected != (n, k))
                        {
                            SetSelectedGlamourSet(n, k, true);
                        }
                    }

                    // Glamour Set - context menu
                    if (ImGui.BeginPopupContextItem())
                    {
                        ImGui.Button($"Rename##{n}{k}");
                        UiHelper.InputText($"Rename##{n}{k}", (name) => RenameGlamourSet(n, k, name));

                        if (ImGui.Button($"Delete##{n}{k}"))
                        {
                            DeleteGlamourSet(n, k);
                        }
                        ImGui.EndPopup();
                    }

                    // Glamour Set - drop source
                    if (ImGui.BeginDragDropSource())
                    {
                        // Not used really
                        unsafe
                        {
                            var i = 0;
                            int* tesnum = &i;
                            ImGui.SetDragDropPayload($"payload{i}", new IntPtr(tesnum), sizeof(int));
                        }

                        // Capture drag source state
                        dropSource = (n, k);
                        ImGui.EndDragDropSource();
                    }

                    // Glamour Set - drop target
                    if (ImGui.BeginDragDropTarget())
                    {
                        // Add colored seperator under
                        ImGui.Separator();

                        // If dropped - move glamour set to the appropriate folder
                        if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                        {
                            // Add to next set in the list
                            MoveGlamourSet(n, k + 1);
                        }
                        ImGui.EndDragDropTarget();
                    }

                }
                ImGui.TreePop();
            }
        }
    }

    private void AddGlamourSet(string setName)
    {
        glamourTree.Directories.First().GlamourSets.Add(new GlamourSet(setName));
    }

    private void AddDirectory(string directoryName)
    {
        glamourTree.Directories.Add(new GlamourDirectory(directoryName));
    }

    public void SetSelectedGlamourSet(int directoryIndex, int glamourSetIndex, bool preview)
    {
        // Set internal state
        selected = (directoryIndex, glamourSetIndex);
        currentGlamourSet = glamourTree.Directories[directoryIndex].GlamourSets[glamourSetIndex];

        // Publish GlamourSetChangeEvent
        EventService.Publish<GlamourSetChangeEvent, GlamourSetChangeEventArgs>(new GlamourSetChangeEventArgs(currentGlamourSet, preview));
    }

    private void RenameGlamourSet(int directoryIndex, int glamourSetIndex, string name)
    {
        glamourTree.Directories[directoryIndex].GlamourSets[glamourSetIndex].Name = name;
    }

    private void RenameDirectory(int directoryIndex, string name)
    {
        glamourTree.Directories[directoryIndex].Name = name;
    }

    private void DeleteGlamourSet(int directoryIndex, int glamourSetIndex)
    {
        // Dont allow deleting last set TODO - make rename/delete buttons grey out
        if (glamourTree.Directories.SelectMany(e => e.GlamourSets).Count() == 1)
        {
            return;
        }

        glamourTree.Directories[directoryIndex].GlamourSets.RemoveAt(glamourSetIndex);

        if (selected == (directoryIndex, glamourSetIndex))
        {
            SetSelectedGlamourSetToFirst();
        }
    }

    private void DeleteDirectory(int directoryIndex)
    {
        // Dont allow deleting last directory
        if (glamourTree.Directories.Count == 1)
        {
            return;
        }
        glamourTree.Directories.RemoveAt(directoryIndex);

        if (selected.directory == directoryIndex)
        {
            SetSelectedGlamourSetToFirst();
        }
    }

    private void SetSelectedGlamourSetToFirst()
    {
        for (var n = 0; n < glamourTree.Directories.Count; n++)
        {
            if (glamourTree.Directories[n].GlamourSets.Any())
            {
                SetSelectedGlamourSet(n, 0, true);
            }
        }
    }

    // Move selected glamour set under this target index
    private void MoveGlamourSet(int targetDirectory, int targetGlamourSet)
    {
        // Determine offset if moving in the same directory
        var targetOffset = 0;
        if (targetDirectory == dropSource.directory)
        {
            if (targetGlamourSet > dropSource.glamourSet)
            {
                targetOffset = -1;
            }
        }

        // Update directory tree
        var sourceGlamourSet = glamourTree.Directories[dropSource.directory].GlamourSets[dropSource.glamourSet];
        glamourTree.Directories[dropSource.directory].GlamourSets.RemoveAt(dropSource.glamourSet);
        glamourTree.Directories[targetDirectory].GlamourSets.Insert(targetGlamourSet + targetOffset, sourceGlamourSet);

        // Update selected glamour set if moved
        var selectedMove = dropSource == selected;
        if (selectedMove)
        {
            SetSelectedGlamourSet(targetDirectory, targetGlamourSet + targetOffset, true);
        }
    }

    private void ApplyGlamourSetToPlate()
    {
        Dev.Log("Applying glamour set to plate");
        // TODO add indication on which items exist in Dresser
        foreach (var (equipSlot, glamourItem) in currentGlamourSet.Items)
        {
            PlatesExecutor.SetPlateItem(glamourItem.GetCollectible().ExcelRow, (byte)glamourItem.StainId);
        }
    }

    // TODO move this to some other module
    private unsafe bool IsInspecting()
    {
        var inspectAgent = AgentInspect.Instance();
        return inspectAgent->AgentInterface.IsAgentActive();
    }

    private unsafe void CopyInspectTargetToGlamourSet(string hint)
    {
        Dev.Log("Copying inspected target to glamour set");

        // Add new glamour set
        AddGlamourSet(hint);

        // New glamour set added in first directory (0) on the last slot (Count-1)
        SetSelectedGlamourSet(0, glamourTree.Directories[0].GlamourSets.Count - 1, false);

        var itemSheet = ExcelCache<ItemAdapter>.GetSheet()!;
        var container = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Examine);

        // Add inspected items to set
        for (var i = 0; i < 13; i++)
        {
            var invSlot = container->GetInventorySlot(i);
            if (invSlot is not null)
            {
                var itemId = invSlot->GlamourId != 0 ? invSlot->GlamourId : invSlot->ItemId;
                var stainId = invSlot->Stains[0];
                var item = itemSheet.GetRow(itemId);
                if (Services.DataProvider.SupportedEquipSlots.Contains(item.Value.EquipSlot))
                {
                    currentGlamourSet.SetItem(item.Value, stainId);
                }
            }
        }
    }

    public void SaveGlamourTreeToConfiguration()
    {
        Dev.Log("Saving glamour tree to configuration...");
        Services.Configuration.GlamourTree = glamourTree;
        Services.Configuration.Save();
    }

    private void InitializeGlamourTreeFromConfiguration()
    {
        Dev.Log("Loading glamour tree from configuration...");
        glamourTree = Services.Configuration.GlamourTree;
    }
}
