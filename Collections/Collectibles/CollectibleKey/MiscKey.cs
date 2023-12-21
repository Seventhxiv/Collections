namespace Collections;

public class MiscKey : CollectibleKey<(string, bool)>, ICreateable<MiscKey, (string, bool)>
{
    public MiscKey((string, bool) misc) : base(misc)
    {
    }

    public static MiscKey Create((string, bool) misc)
    {
        return new(misc);
    }

    protected override string GetName((string, bool) input)
    {
        return input.Item1;
    }

    protected override uint GetId((string, bool) input)
    {
        return 0;
    }

    protected override List<ICollectibleSource> GetCollectibleSources((string, bool) input)
    {
        return new List<ICollectibleSource>() { new MiscSource(input.Item1) };
    }

    protected override HashSet<SourceCategory> GetBaseSourceCategories()
    {
        return new HashSet<SourceCategory>();
    }

    public override Tradeability GetIsTradeable()
    {
        return Tradeability.Untradeable;
    }
}

