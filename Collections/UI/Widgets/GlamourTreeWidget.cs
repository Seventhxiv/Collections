namespace Collections;

public class GlamourTreeWidget
{
    private GlamourSet currentGlamourSet = new();
    private List<(string name, List<GlamourSet> setList)> glamourTree = new();

    private EventService EventService { get; init ; }
    public GlamourTreeWidget(EventService eventService)
    {
        EventService = eventService;
        AddDirectory("Main");
        AddGlamourSet("Default");
    }

    public void Draw()
    {
        // Directory icon
        UiHelper.IconButtonWithPopUpInputText(FontAwesomeIcon.Folder, (s) => AddDirectory(s));
        ImGui.SameLine();

        // Glamour Set icon
        UiHelper.IconButtonWithPopUpInputText(FontAwesomeIcon.Plus, (s) => AddGlamourSet(s));

        // Folder tree
        DrawGlamourSetList();
    }

    private (int directory, int glamourSet) selected;
    private (int directory, int glamourSet) dropSource = (-1, -1);
    private void DrawGlamourSetList()
    {

        for (var n = 0; n < glamourTree.Count; n++)
        {
            var (name, setList) = glamourTree[n];

            var treeFlags = ImGuiTreeNodeFlags.None | ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.SpanAvailWidth; // | ImGuiTreeNodeFlags.NavLeftJumpsBackHere | ImGuiTreeNodeFlags.FramePadding; //| ImGuiTreeNodeFlags.SpanAvailWidth;
            //if (hoveredWhileDragging == (n, null))
            //{
            //    treeFlags |= ImGuiTreeNodeFlags.Selected;
            //}

            // Directory tree node
            if (ImGui.TreeNodeEx($"{name}##{n}", treeFlags))
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
                for (var k = 0; k < setList.Count; k++)
                {

                    // Selectable Glamour Set
                    var textBaseWidth = ImGui.CalcTextSize("A").X;
                    ImGui.SetNextItemWidth(textBaseWidth * 5);
                    if (ImGui.Selectable($"   {setList[k].name}##{n}{k}", selected == (n, k), ImGuiSelectableFlags.None))
                    {
                        // Change selected set if (actually) clicked on a different set
                        if (selected != (n, k))
                        {
                            SetSelectedGlamourSet(n, k);
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
                            MoveGlamourSet(n, k+1);
                        }
                        ImGui.EndDragDropTarget();
                    }

                }
                ImGui.TreePop();
            }
        }
    }

    private void AddGlamourSet(string key)
    {
       var glamourSet = new GlamourSet() { name = key };
       glamourTree[0].setList.Add(glamourSet);
    }

    private void AddDirectory(string name)
    {
        glamourTree.Insert(glamourTree.Count, (name, new List<GlamourSet>()));
    }

    public void SetSelectedGlamourSet(int directoryIndex, int glamourSetIndex)
    {
        // Set internal state
        selected = (directoryIndex, glamourSetIndex);
        currentGlamourSet = glamourTree[directoryIndex].setList[glamourSetIndex];

        // Publish GlamourSetChangeEvent
        EventService.Publish<GlamourSetChangeEvent, GlamourSetChangeEventArgs>(new GlamourSetChangeEventArgs(currentGlamourSet));
    }

    private void RenameGlamourSet(int directoryIndex, int glamourSetIndex, string name)
    {
        glamourTree[directoryIndex].setList[glamourSetIndex].name = name;
    }

    private void RenameDirectory(int directoryIndex, string name)
    {
        var directory = glamourTree[directoryIndex];
        directory.name = name;
        glamourTree[directoryIndex] = directory;
    }

    private void DeleteGlamourSet(int directoryIndex, int glamourSetIndex)
    {
        // Dont allow deleting last set TODO - make rename/delete buttons grey out
        if (glamourTree.SelectMany(e => e.setList).Count() == 1)
        {
            return;
        }

        glamourTree[directoryIndex].setList.RemoveAt(glamourSetIndex);

        if (selected == (directoryIndex, glamourSetIndex))
        {
            SetSelectedGlamourSetToFirst();
        }
    }

    private void DeleteDirectory(int directoryIndex)
    {
        // Dont allow deleting last directory
        if (glamourTree.Count == 1)
        {
            return;
        }
        glamourTree.RemoveAt(directoryIndex);

        if (selected.directory == directoryIndex)
        {
            SetSelectedGlamourSetToFirst();
        }
    }

    private void SetSelectedGlamourSetToFirst()
    {
        for (var n = 0; n < glamourTree.Count; n++)
        {
            if (glamourTree[n].setList.Any())
            {
                SetSelectedGlamourSet(n, 0);
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
        var sourceGlamourSet = glamourTree[dropSource.directory].setList[dropSource.glamourSet];
        glamourTree[dropSource.directory].setList.RemoveAt(dropSource.glamourSet);
        glamourTree[targetDirectory].setList.Insert(targetGlamourSet + targetOffset, sourceGlamourSet);

        // Update selected glamour set if moved
        var selectedMove = dropSource == selected;
        if (selectedMove)
        {
            SetSelectedGlamourSet(targetDirectory, targetGlamourSet + targetOffset);
        }
    }
}
