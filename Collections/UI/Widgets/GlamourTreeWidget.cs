using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using Collections.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using static Collections.UiHelper;

namespace Collections;

public class GlamourTreeWidget
{
    public List<(string name, List<GlamourSet> setList)> GlamourTree = new();
    public GlamourSet CurrentGlamourSet = new();

    public GlamourTreeWidget()
    {
        AddDefaultGlamourSet();
    }

    public void Draw()
    {
        //ImGui.Text("                                                              ");
        // Always have at least a default set
        if (!GlamourTree.SelectMany(e => e.setList).Any())
        {
            AddDefaultGlamourSet();
        }

        // Add folder
        UiHelper.IconButtonWithPopUpInputText(FontAwesomeIcon.Folder, ref setName, (s) => AddEmptyGlamourFolder(s));
        ImGui.SameLine();

        // Add set
        UiHelper.IconButtonWithPopUpInputText(FontAwesomeIcon.Plus, ref folderName, (s) => AddDefaultGlamourSet(s));

        // Folder tree
        DrawGlamourSetList();
    }

    private (int tree_index, int set_index) selected = (-1, -1);
    //private GlamourSet selected; TODO
    private (int tree_index, int set_index) hoveredWhileDragging = (-1, -1);
    private (int tree_index, int set_index) dropSource = (-1, -1);
    private (int tree_index, int set_index)? dropTarget = null;
    private void DrawGlamourSetList()
    {
        // Style
        ImGui.PushStyleColor(ImGuiCol.Header, new Vector4(0.306f, 0.143f, 0.532f, 0.811f)); // selected input
        ImGui.PushStyleColor(ImGuiCol.Separator, colors[CommonColor.purple]);
        var bgColor = ImGui.GetStyle().Colors[(int)ImGuiCol.ChildBg];
        var oldFontSize = ImGui.GetFont().FontSize;
        var oldScale = ImGui.GetFont().Scale;
        //ImGui.GetFont().Scale *= 1f;
        //ImGui.GetFont().FontSize *= 1.2f;
        //ImGui.PushFont(ImGui.GetFont());
        var n = 0;

        foreach (var (name, setList) in GlamourTree)
        {
            n = GlamourTree.IndexOf((name, setList));
            if (n == GlamourTree.Count - 1) break;
            var treeFlags = ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.NavLeftJumpsBackHere; //  | ImGuiTreeNodeFlags.SpanAvailWidth
            if (hoveredWhileDragging == (n, -1))
            {
                treeFlags |= ImGuiTreeNodeFlags.Selected;
            }
            //ImGui.BeginChild($"tree-nodex##{n}");
            if (ImGui.TreeNodeEx($"{name}##tree-node-{name}", treeFlags))
            {

                if (ImGui.BeginDragDropTarget())
                {
                    hoveredWhileDragging = (n, -1);
                    //selected = (n, k);
                    if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                    {
                        dropTarget = (n, 0);
                    }
                    ImGui.EndDragDropTarget();
                }
                //if (hoveredWhileDragging == (n, -1))
                //{
                //    ImGui.Separator();
                //}
                foreach (var glamourSet in setList)
                {
                    var k = setList.IndexOf(glamourSet);

                    //ImGui.PushStyleColor(ImGuiCol.HeaderActive, new Vector4(0.198f, 0.811f, 0.198f, 0.353f)); // clicked (for a second)
                    //var actualSelected = (selected == k) && ImGui.IsMouseDown(ImGuiMouseButton.Left);
                    //ImGui.SetItemAllowOverlap();

                    // Replace current glamour set on select
                    if (ImGui.Selectable($"   {glamourSet.name}##glam-set-{n}{k}", selected == (n, k), ImGuiSelectableFlags.None, new System.Numerics.Vector2(170, 0))) // | hoveredWhileDragging == (glamourNode.Key, k)
                    {
                        CurrentGlamourSet = glamourSet;
                        selected = (n, k);
                    }

                    if (ImGui.BeginDragDropSource())
                    {
                        // Not used really
                        unsafe
                        {
                            int* tesnum = &k;
                            ImGui.SetDragDropPayload($"payload{k}", new IntPtr(tesnum), sizeof(int));
                        }

                        // Mark dragged index
                        dropSource = (n, k);
                        ImGui.EndDragDropSource();
                    }

                    if (ImGui.BeginDragDropTarget())
                    {
                        // Highlight hovered targets as selected
                        hoveredWhileDragging = (n, k);

                        // If dropped - move glamour set to the appropriate folder
                        if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                        {
                            dropTarget = (n, k);
                        }
                        ImGui.EndDragDropTarget();
                    }

                    // Add colored seperator below to mark the drop location
                    if (hoveredWhileDragging == (n, k))
                    {
                        ImGui.Separator();
                    }
                    // Tie hover and select together
                    //if (ImGui.IsItemHovered())
                    //{
                    //    selected = (glamourNode.Key, k);
                    //}
                    //ImGui.PopID();
                }
                ImGui.TreePop();
            }
            //ImGui.EndChild();
        }

        n = GlamourTree.Count - 1;
        var glamListNoFolder = GlamourTree[n].setList;
        foreach (var glamourSet in glamListNoFolder)
        {
            var k = glamListNoFolder.IndexOf(glamourSet);

            //ImGui.PushStyleColor(ImGuiCol.HeaderActive, new Vector4(0.198f, 0.811f, 0.198f, 0.353f)); // clicked (for a second)
            //var actualSelected = (selected == k) && ImGui.IsMouseDown(ImGuiMouseButton.Left);
            //ImGui.SetItemAllowOverlap();

            //ImGui.BeginGroup();//($"no-folder-child##{n}{k}", new Vector2(100, 20));
            // Replace current glamour set on select
            var treeFlags = ImGuiTreeNodeFlags.Leaf; // ImGuiTreeNodeFlags.SpanAvailWidth | 
            if (selected == (n, k))
            { treeFlags |= ImGuiTreeNodeFlags.Selected; }
            //ImGui.TreeNodeEx($":-> {glamourSet.name}##glam-set-{n}{k}", treeFlags); // | hoveredWhileDragging == (n, k)
            if (ImGui.Selectable($"   {glamourSet.name}##glam-set-{n}{k}", selected == (n, k), ImGuiSelectableFlags.None, new System.Numerics.Vector2(170, 0))) // | hoveredWhileDragging == (glamourNode.Key, k)
            {
                CurrentGlamourSet = glamourSet;
                selected = (n, k);
            }

            if (ImGui.BeginDragDropSource())
            {
                // Not used really
                unsafe
                {
                    int* tesnum = &k;
                    ImGui.SetDragDropPayload($"payload{k}", new IntPtr(tesnum), sizeof(int));
                }

                // Mark dragged index
                dropSource = (n, k);
                ImGui.EndDragDropSource();
            }

            if (ImGui.BeginDragDropTarget())
            {
                // Highlight hovered targets as selected
                hoveredWhileDragging = (n, k);

                // If dropped - move glamour set to the appropriate folder
                if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                {
                    dropTarget = (n, k);
                }
                ImGui.EndDragDropTarget();
            }

            // Add colored seperator below to mark the drop location
            if (hoveredWhileDragging == (n, k))
            {
                ImGui.Separator();
            }
            //ImGui.EndGroup();
        }
        //ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        //ImGui.GetWindowDrawList().AddLine();
        // Reset hovered highlight if no mouse input
        if (!ImGui.IsMouseDown(ImGuiMouseButton.Left))
        {
            hoveredWhileDragging = (-1, -1);
        }

        if (dropTarget != null)
        {
            // Move item to another folder
            if (dropTarget?.tree_index != dropSource.tree_index)
            {
                if (GlamourTree[(int)dropTarget?.tree_index].setList.Count == (int)dropTarget?.set_index)
                    GlamourTree[(int)dropTarget?.tree_index].setList.Add(GlamourTree[dropSource.tree_index].setList[dropSource.set_index]);
                else
                    GlamourTree[(int)dropTarget?.tree_index].setList.Insert((int)dropTarget?.set_index + 1, GlamourTree[dropSource.tree_index].setList[dropSource.set_index]);
                GlamourTree[dropSource.tree_index].setList.RemoveAt(dropSource.set_index);
            }

            else
            {
                var list = GlamourTree[dropSource.tree_index].setList;
                var element = list[dropSource.set_index];
                var removeIndex = 0;

                // Move item down in same folder
                if (dropTarget?.set_index > dropSource.set_index)
                {
                    list.Insert((int)dropTarget?.set_index + 1, element);
                    removeIndex = dropSource.set_index;
                }

                // Move item up in same folder
                else
                {
                    list.Insert((int)dropTarget?.set_index + 1, element);
                    removeIndex = dropSource.set_index + 1;
                }
                list.RemoveAt(removeIndex);
            }
            // TODO: move selected if the selected element moved
            if (selected == (dropSource.tree_index, dropSource.set_index))
                selected = ((int)dropTarget?.tree_index, (int)dropTarget?.set_index);
            dropTarget = null;
        }

        //ImGui.palle
        //ImGui.GetFont().Scale = oldScale;
        //ImGui.GetFont().FontSize = oldFontSize;
        //ImGui.PopFont();
    }

    private string setName = "";
    private void AddDefaultGlamourSet(string key = "Default", GlamourSet glamourSet = null)
    {
        if (glamourSet == null)
        {
            glamourSet = new GlamourSet() { name = key };
        }
        if (!GlamourTree.Any())
        {
            GlamourTree.Add(("no-folder", new List<GlamourSet>()));
        }
        GlamourTree[GlamourTree.Count - 1].setList.Add(glamourSet);
    }

    private string folderName = "";
    private void AddEmptyGlamourFolder(string name = "Default")
    {
        GlamourTree.Insert(GlamourTree.Count - 1, (name, new List<GlamourSet>()));
    }
}
