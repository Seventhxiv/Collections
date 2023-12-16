namespace Collections;

public interface ICollectibleKey
{
    public string Name { get; init; }
    public uint Id { get; init; } // Debug purposes
    public List<ICollectibleSource> CollectibleSources { get; init; }
    public HashSet<SourceCategory> SourceCategories { get; init; }
    public bool GetIsTradeable();
    public int? GetMarketBoardPriceLazy();
}