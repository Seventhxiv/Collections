namespace Collections;

public class ContainerCollectibleSource : CollectibleSource
{
    private ItemAdapter container { get; init; }
    private CollectibleKey CollectibleKey { get; init; }
    public ContainerCollectibleSource(uint containerId)
    {
        container = ExcelCache<ItemAdapter>.GetSheet().GetRow(containerId);
        CollectibleKey = CollectibleKeyCache.Instance.GetObject((container, true));
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

    public override void DisplayLocation()
    {
    }

    protected override int GetIconId()
    {
        return container.Icon;
    }
}
