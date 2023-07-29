using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace Collections;

public class ContainerCollectibleSource : CollectibleSource
{
    private Item container { get; init; }
    private CollectibleUnlockItem CollectibleUnlockItem { get; init; }
    public ContainerCollectibleSource(uint containerId)
    {
        container = Excel.GetExcelSheet<Item>().GetRow(containerId);
        CollectibleUnlockItem = new CollectibleUnlockItem(container);
    }

    public override string GetName()
    {
        return container.Name;
    }

    private List<CollectibleSourceType> sourceType;// = new() { CollectibleSourceType.Event };
    public override List<CollectibleSourceType> GetSourceType()
    {
        if (sourceType != null)
        {
            return sourceType;
        }

        // Item originated from a container has its SourceType dictated by the container
        sourceType = CollectibleUnlockItem.GetSourceTypes();
        return sourceType;
    }

    public override bool GetIslocatable()
    {
        return false;
    }

    public override LocationEntry GetLocationEntry()
    {
        return null;
    }

    protected override int GetIconId()
    {
        return container.Icon;
    }
}
