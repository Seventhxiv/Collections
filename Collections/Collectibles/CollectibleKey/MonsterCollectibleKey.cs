namespace Collections;

public class MonsterCollectibleKey : ICollectibleKey, ICreateable<MonsterCollectibleKey, Monster>
{
    public string Name { get; init; }
    public uint Id { get; init; }
    public List<CollectibleSource> CollectibleSources { get; init; } = new();
    private List<CollectibleSourceCategory> sourceCategories;

    public MonsterCollectibleKey(Monster monster)
    {
        Name = monster.name;
        Id = 0; // TODO
        CollectibleSources.Add(new MonsterCollectibleSource(monster));
        sourceCategories = new List<CollectibleSourceCategory>();
    }

    public static MonsterCollectibleKey Create(Monster input)
    {
        return new(input);
    }

    public bool GetIsTradeable()
    {
        return false;
    }

    public List<CollectibleSourceCategory> GetSourceCategories()
    {
        return sourceCategories;
    }

    public int? GetMarketBoardPriceLazy()
    {
        throw new NotImplementedException();
    }
}

