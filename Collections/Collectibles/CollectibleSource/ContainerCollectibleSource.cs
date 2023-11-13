namespace Collections;

public class ContainerCollectibleSource : CollectibleSource
{
    private ItemAdapter container { get; init; }
    private CollectibleKey CollectibleKey { get; init; }
    public ContainerCollectibleSource(uint containerId)
    {
        container = Excel.GetExcelSheet<ItemAdapter>().GetRow(containerId);
        CollectibleKey = new CollectibleKey(container);
    }

    public override string GetName()
    {
        return container.Name;
    }

    private List<CollectibleSourceCategory> sourceType;
    public override List<CollectibleSourceCategory> GetSourceCategories()
    {
        if (sourceType != null)
        {
            return sourceType;
        }

        // Item originated from a container has its SourceType dictated by the container
        sourceType = CollectibleKey.GetSourceTypes();
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
