namespace Collections;

public class AchievementKey : CollectibleKey<(Achievement, bool)>, ICreateable<AchievementKey, (Achievement, bool)>
{
    public AchievementKey((Achievement, bool) input) : base(input)
    {
    }

    public static AchievementKey Create((Achievement, bool) input)
    {
        return new(input);
    }

    protected override string GetName((Achievement, bool) input)
    {
        return input.Item1.Name.ToString();
    }

    protected override uint GetId((Achievement, bool) input)
    {
        return input.Item1.RowId;
    }

    protected override List<ICollectibleSource> GetCollectibleSources((Achievement, bool) input)
    {
        return new List<ICollectibleSource>() { new AchievementSource(input.Item1) };
    }

    protected override HashSet<SourceCategory> GetBaseSourceCategories()
    {
        return new HashSet<SourceCategory>() { SourceCategory.Achievement };
    }

    public override Tradeability GetIsTradeable()
    {
        return Tradeability.Untradeable;
    }
}

