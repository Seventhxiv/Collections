namespace Collections;

// Represents an item that is used to unlock a collectible
public interface ICollectibleKey
{
    public uint Id { get; init; }
    public string Name { get; init; }
    public List<CollectibleSource> CollectibleSources { get; init; }
    public List<CollectibleSourceCategory> GetSourceCategories();
    public bool GetIsTradeable();
    public int? GetMarketBoardPriceLazy();
}

public abstract class CollectibleKey<T> : ICollectibleKey where T : ExcelRow
{
    public abstract string Name { get; init; }
    public uint Id { get; init; }
    public List<CollectibleSource> CollectibleSources { get; init; } = new();
    public T excelRow { get; init; }

    public CollectibleKey((T, bool) input)
    {
        excelRow = input.Item1;
        Id = excelRow.RowId;
    }

    public abstract bool GetIsTradeable();
    public abstract List<CollectibleSourceCategory> GetSourceCategories();
    public abstract int? GetMarketBoardPriceLazy();
}

