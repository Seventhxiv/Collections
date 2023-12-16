namespace Collections;

public class InstanceKey : CollectibleKey<(ContentFinderCondition, bool)>, ICreateable<InstanceKey, (ContentFinderCondition, bool)>
{
    public InstanceKey((ContentFinderCondition, bool) input) : base(input)
    {
    }

    public static InstanceKey Create((ContentFinderCondition, bool) input)
    {
        return new(input);
    }

    protected override string GetName((ContentFinderCondition, bool) input)
    {
        return input.Item1.Name;
    }

    protected override uint GetId((ContentFinderCondition, bool) input)
    {
        return input.Item1.RowId;
    }

    protected override List<ICollectibleSource> GetCollectibleSources((ContentFinderCondition, bool) input)
    {
        return new List<ICollectibleSource>() { new InstanceSource(input.Item1) };
    }

    protected override HashSet<SourceCategory> GetBaseSourceCategories()
    {
        return new HashSet<SourceCategory>() { SourceCategory.Duty };
    }

    public override bool GetIsTradeable()
    {
        return false;
    }
}

