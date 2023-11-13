namespace Collections;

// Represents something that can be collected (Glamour, Mount, Minion, etc..)
public interface ICollectible
{
    public string GetName();
    public string Name { get; init; }
    public void UpdateObtainedState();
    public CollectibleKey CollectibleKey { get; init; }
    public bool isFavorite { get; set; } // TODO move to dedicated storage
    public void OpenGamerEscape(); // TODO move out
    public bool GetIsObtained();
    public IDalamudTextureWrap GetIconLazy();
    public int SortKey();
}


public abstract class Collectible<T> : ICollectible where T : ExcelRow
{
    public abstract string GetName();
    public abstract void UpdateObtainedState();
    public bool isFavorite { get; set; } = false;
    public abstract string Name { get; init; }

    public CollectibleKey CollectibleKey { get; init; }
    protected abstract T excelRow { get; set; }
    protected IconHandler IconHandler { get; init; }

    public Collectible(T excelRow)
    {
        this.excelRow = excelRow;
        IconHandler = new IconHandler(GetIconId());
    }

    public void OpenGamerEscape()
    {
        GamerEscapeLink.OpenItem(GetName());
    }

    protected bool isObtained = false;
    public bool GetIsObtained()
    {
        return isObtained;
    }

    protected abstract int GetIconId();
    public IDalamudTextureWrap GetIconLazy()
    {
        return IconHandler.GetIconLazy();
    }

    public static string GetCollectionName()
    {
        return "";
    }

    public virtual int SortKey()
    {
        return 0;
    }
}
