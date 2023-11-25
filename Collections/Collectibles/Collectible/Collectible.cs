namespace Collections;

// Represents something that can be collected (Glamour, Mount, Minion, etc..)
public interface ICollectible
{
    public string Name { get; init; }
    public CollectibleKey CollectibleKey { get; init; }
    public bool IsFavorite();
    public void SetFavorite(bool favorite);
    public bool IsWishlist();
    public void SetWishlist(bool wishlist);
    public void OpenGamerEscape();
    public bool GetIsObtained();
    public void UpdateObtainedState();
    public IDalamudTextureWrap GetIconLazy();
    public void Interact();
}

public abstract class Collectible<T> : ICollectible where T : ExcelRow
{
    public abstract string Name { get; init; }
    public abstract void UpdateObtainedState();
    public abstract void Interact();
    protected abstract int GetIconId();

    public CollectibleKey CollectibleKey { get; init; }
    protected abstract T excelRow { get; set; }
    protected IconHandler IconHandler { get; init; }

    public Collectible(T excelRow)
    {
        this.excelRow = excelRow;
        IconHandler = new IconHandler(GetIconId());
    }

    public static string GetCollectionName()
    {
        throw new NotImplementedException();
    }

    public void OpenGamerEscape()
    {
        WikiOpener.OpenGamerEscape(Name);
    }

    protected bool isObtained = false;
    public bool GetIsObtained()
    {
        return isObtained;
    }

    public bool IsFavorite()
    {
        if (CollectibleKey is not null)
            return Services.Configuration.Favorites.Contains(CollectibleKey.item.RowId);
        else
            return false;
    }

    public void SetFavorite(bool favorite)
    {
        var itemId = CollectibleKey.item.RowId;
        if (favorite)
        {
            Dev.Log($"Adding {itemId} to Favorites");
            Services.Configuration.Favorites.Add(itemId);
        }
        else
        {
            Dev.Log($"Removing {itemId} from Favorites");
            Services.Configuration.Favorites.Remove(itemId);
        }
        Services.Configuration.Save();
    }

    public bool IsWishlist()
    {
        if (CollectibleKey is not null)
            return Services.Configuration.WishListed.Contains(CollectibleKey.item.RowId);

        return false;
    }

    public void SetWishlist(bool wishlist)
    {
        var itemId = CollectibleKey.item.RowId;
        if (wishlist)
        {
            Dev.Log($"Adding {itemId} to Wishlist");
            Services.Configuration.WishListed.Add(itemId);
        }
        else
        {
            Dev.Log($"Removing {itemId} from Wishlist");
            Services.Configuration.WishListed.Remove(itemId);
        }
        Services.Configuration.Save();
    }

    public IDalamudTextureWrap GetIconLazy()
    {
        return IconHandler.GetIconLazy();
    }
}
