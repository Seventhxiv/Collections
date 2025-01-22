namespace Collections;

public class AchievementKey : CollectibleKey<(Achievement, int)>, ICreateable<AchievementKey, (Achievement, int)>
{
    public AchievementKey((Achievement, int) input) : base(input)
    {
    }

    public static AchievementKey Create((Achievement, int) input)
    {
        return new(input);
    }

    protected override string GetName((Achievement, int) input)
    {
        return input.Item1.Name.ToString();
    }

    protected override uint GetId((Achievement, int) input)
    {
        return input.Item1.RowId;
    }

    protected override List<ICollectibleSource> GetCollectibleSources((Achievement, int) input)
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

