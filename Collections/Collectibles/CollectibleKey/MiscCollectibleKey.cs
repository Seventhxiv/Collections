namespace Collections;

public class MiscCollectibleKey : ICollectibleKey, ICreateable<MiscCollectibleKey, string>
{
    public string Name { get; init; }
    public uint Id { get; init; }
    public List<CollectibleSource> CollectibleSources { get; init; } = new();

    private List<CollectibleSourceCategory> sourceCategories = new();

    public MiscCollectibleKey(string misc)
    {
        Name = misc;
        CollectibleSources.Add(new MiscCollectibleSource(misc));
    }

    public static MiscCollectibleKey Create(string misc)
    {
        return new(misc);
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

