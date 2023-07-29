using ImGuiNET;
using System.Collections.Generic;

namespace Collections;

public class CollectionWindow : IDrawable
{
    private List<ICollectible> collection { get; init; }
    protected FiltersWidget FiltersWidget { get; init; }
    protected CollectionWidget CollectionWidget { get; init; }

    public CollectionWindow(List<ICollectible> collection)
    {
        this.collection = collection;
        FiltersWidget = new FiltersWidget(1, collection);
        CollectionWidget = new CollectionWidget(FiltersWidget);
    }

    // Default draw layout
    public virtual void Draw()
    {

        // Filters
        ImGui.BeginGroup();
        if (ImGui.BeginTable("filters-widget1", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.NoHostExtendX)) //  
        {
            ImGui.TableNextColumn();
            ImGui.TableHeader("Content Filters");

            ImGui.TableNextRow(ImGuiTableRowFlags.None, UiHelper.GetLengthToBottomOfWindow());
            ImGui.TableNextColumn();
            FiltersWidget.Draw();
            ImGui.EndTable();
        }
        ImGui.EndGroup();

        // Glam collection
        ImGui.SameLine();
        ImGui.BeginChild("collection");
        CollectionWidget.Draw(collection);
        ImGui.EndChild();
    }

    public void OnOpen()
    {
        foreach (var collectible in collection)
        {
            collectible.UpdateObtainedState();
        }
    }

    public void Dispose()
    {
    }
}

