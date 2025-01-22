namespace Collections;

public class InstanceKey : CollectibleKey<(ContentFinderCondition, int)>, ICreateable<InstanceKey, (ContentFinderCondition, int)>
{
    public InstanceKey((ContentFinderCondition, int) input) : base(input)
    {
    }

    public static InstanceKey Create((ContentFinderCondition, int) input)
    {
        return new(input);
    }

    protected override string GetName((ContentFinderCondition, int) input)
    {
        return input.Item1.Name.ToString();
    }

    protected override uint GetId((ContentFinderCondition, int) input)
    {
        return input.Item1.RowId;
    }

    protected override List<ICollectibleSource> GetCollectibleSources((ContentFinderCondition, int) input)
    {
        return new List<ICollectibleSource>() { new InstanceSource(input.Item1) };
    }

    protected override HashSet<SourceCategory> GetBaseSourceCategories()
    {
        return new HashSet<SourceCategory>() { SourceCategory.Duty };
    }

    public override Tradeability GetIsTradeable()
    {
        return Tradeability.Untradeable;
    }
}

