namespace Collections;

public class ContainerSource : CollectibleSource
{
    private ItemAdapter container { get; init; }
    private ICollectibleKey CollectibleKey { get; init; }
    public ContainerSource(uint containerId)
    {
        container = ExcelCache<ItemAdapter>.GetSheet().GetRow(containerId);
        CollectibleKey = CollectibleKeyCache<ItemKey, ItemAdapter>.Instance.GetObject((container, true));
    }

    public override string GetName()
    {
        return container.Name;
    }

    private List<SourceCategory> sourceType;
    public override List<SourceCategory> GetSourceCategories()
    {
        if (sourceType != null)
        {
            return sourceType;
        }

        // Item originated from a container has its SourceType dictated by the container
        sourceType = CollectibleKey.SourceCategories.ToList();
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
