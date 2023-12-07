namespace Collections;

public class InstanceCollectibleKey : CollectibleKey<ContentFinderCondition>, ICreateable<InstanceCollectibleKey, (ContentFinderCondition, bool)>
{
    public override string Name { get; init; }

    private List<CollectibleSourceCategory> sourceCategories;

    public InstanceCollectibleKey((ContentFinderCondition, bool) input) : base(input)
    {
        Name = excelRow.Name;
        CollectibleSources.Add(new InstanceCollectibleSource(input.Item1));
        sourceCategories = new List<CollectibleSourceCategory>() { CollectibleSourceCategory.Duty };
    }

    public static InstanceCollectibleKey Create((ContentFinderCondition, bool) input)
    {
        return new(input);
    }

    public override bool GetIsTradeable()
    {
        return false;
    }

    public override List<CollectibleSourceCategory> GetSourceCategories()
    {
        return sourceCategories;
    }

    public override int? GetMarketBoardPriceLazy()
    {
        throw new NotImplementedException();
    }
}

