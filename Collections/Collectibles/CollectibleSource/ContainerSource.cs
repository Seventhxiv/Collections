namespace Collections;

public class ContainerSource : CollectibleSource
{
    private ItemAdapter container { get; init; }
    private ICollectibleKey CollectibleKey { get; init; }
    public ContainerSource(uint containerId, int initDepth)
    {
        container = ExcelCache<ItemAdapter>.GetSheet().GetRow(containerId).Value;
        CollectibleKey = CollectibleKeyCache<ItemKey, ItemAdapter>.Instance.GetObject((container, initDepth));
    }


    public override string GetName()
    {
        return container.Name.ToString();
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
