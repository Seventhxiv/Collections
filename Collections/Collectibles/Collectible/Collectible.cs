using ImGuiScene;
using Lumina.Excel;
using System.Collections.Generic;

namespace Collections;

// Represents a collectible (Glamour, Mount, Minion, etc..)
public abstract class Collectible<T> : ICollectible where T : ExcelRow
{
    // Interface
    public abstract string GetName();
    public abstract void UpdateObtainedState();
    public bool isFavorite { get; set; } = false;

    // Properties
    public CollectibleUnlockItem CollectibleUnlockItem { get; set; }
    protected abstract T excelRow { get; set; }
    protected IconHandler IconHandler { get; init; }

    // Constructor
    public Collectible(T excelRow)
    {
        this.excelRow = excelRow;
        IconHandler = new IconHandler(GetIconId());
    }

    // Internal implementation
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
    public TextureWrap GetIconLazy()
    {
        return IconHandler.GetIconLazy();
    }

    public static List<ICollectible> GetCollection()
    {
        return new List<ICollectible>();
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

public interface ICollectible
{
    public string GetName();
    public void UpdateObtainedState();
    public CollectibleUnlockItem CollectibleUnlockItem { get; set; }
    public bool isFavorite { get; set; }
    public void OpenGamerEscape();
    public bool GetIsObtained();
    public TextureWrap GetIconLazy();
    public int SortKey();
}

//public interface ICollectibleCollector
//{
//    abstract static List<ICollectible> GetCollection();
//    abstract static string GetCollectionName();
//}
