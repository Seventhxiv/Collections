namespace Collections;

public class AchievementCollectibleKey : CollectibleKey<Achievement>, ICreateable<AchievementCollectibleKey, (Achievement, bool)>
{
    public override string Name { get; init; }

    private List<CollectibleSourceCategory> sourceCategories;

    public AchievementCollectibleKey((Achievement, bool) input) : base(input)
    {
        Name = excelRow.Name;
        CollectibleSources.Add(new AchievementCollectibleSource(input.Item1));
        sourceCategories = new List<CollectibleSourceCategory>() { CollectibleSourceCategory.Achievement };
    }

    public static AchievementCollectibleKey Create((Achievement, bool) input)
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

