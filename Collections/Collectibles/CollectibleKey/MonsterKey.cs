namespace Collections;

public class MonsterKey : CollectibleKey<(Monster, bool)>, ICreateable<MonsterKey, (Monster, bool)>
{
    public MonsterKey((Monster, bool) input) : base(input)
    {
    }

    public static MonsterKey Create((Monster, bool) input)
    {
        return new(input);
    }

    protected override string GetName((Monster, bool) input)
    {
        return input.Item1.name;
    }

    protected override uint GetId((Monster, bool) input)
    {
        return 0;
    }

    protected override List<ICollectibleSource> GetCollectibleSources((Monster, bool) input)
    {
        return new List<ICollectibleSource>() { new MonsterSource(input.Item1) };
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

