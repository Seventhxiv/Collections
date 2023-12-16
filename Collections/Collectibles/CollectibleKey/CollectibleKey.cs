namespace Collections;

public abstract class CollectibleKey<T> : ICollectibleKey
{
    public string Name { get; init; }
    public uint Id { get; init; }
    public T Input { get; init; }
    public List<ICollectibleSource> CollectibleSources { get; init; } = new();
    public HashSet<SourceCategory> SourceCategories { get; init; }

    protected abstract string GetName(T input);
    protected abstract uint GetId(T input);
    protected abstract List<ICollectibleSource> GetCollectibleSources(T input);
    protected abstract HashSet<SourceCategory> GetBaseSourceCategories();
    public abstract bool GetIsTradeable();

    public CollectibleKey(T input)
    {
        Input = input;
        Name = GetName(input);

        CollectibleSources = GetCollectibleSources(input);

        SourceCategories = GetBaseSourceCategories();
        SourceCategories.UnionWith(CollectibleSources.SelectMany(e => e.GetSourceCategories()));
    }

    public virtual int? GetMarketBoardPriceLazy()
    {
        throw new NotImplementedException();
    }
}
