namespace Collections;

public abstract class Collectible<T> : ICollectible where T : struct, IExcelRow<T>
{
    public string Name { get; init; }
    public uint Id { get; init; }
    public string CollectionName;
    public abstract void UpdateObtainedState();
    public abstract void Interact();
    public HintModule PrimaryHint { get; init; }
    public HintModule SecondaryHint { get; init; }
    public string Description { get; init; }

    protected abstract int GetIconId();
    protected abstract uint GetId();
    protected abstract string GetName();
    protected abstract string GetDescription();
    protected abstract HintModule GetPrimaryHint();
    protected abstract HintModule GetSecondaryHint();
    protected abstract string GetCollectionName();

    public ICollectibleKey CollectibleKey { get; init; }
    public T ExcelRow { get; set; }
    protected IconHandler IconHandler { get; init; }
    protected List<CollectibleSortOption> SortOptions = [
        // same with sorting by ID, eventualy will be replaced with 'sort by patch'
        new CollectibleSortOption("Id", Comparer<ICollectible>.Create((c1, c2) => c2.Id.CompareTo(c1.Id)), false, (FontAwesomeIcon.SortNumericDown, FontAwesomeIcon.SortNumericUp)),
        new CollectibleSortOption("Name", Comparer<ICollectible>.Create((c1, c2) => c1.Name.CompareTo(c2.Name)), false, (FontAwesomeIcon.SortAlphaUp, FontAwesomeIcon.SortAlphaDown)),
        // comparing c2 to c1 to modify default sort behavior
        new CollectibleSortOption("Obtained", Comparer<ICollectible>.Create((c1, c2) => c2.GetIsObtained().CompareTo(c1.GetIsObtained())), false, null)
    ];

    public Collectible(T excelRow)
    {
        ExcelRow = excelRow;
        Id = GetId();
        CollectibleKey = CollectibleKeyFactory.Get(this);
        Name = GetName();
        Description = GetDescription();
        PrimaryHint = GetPrimaryHint();
        SecondaryHint = GetSecondaryHint();
        IconHandler = new IconHandler(GetIconId());

        if (CollectibleKey is null)
        {
            Dev.Log($"Missing collectible key: {Name} ({Id})");
        }
    }

    public virtual void OpenGamerEscape()
    {
        WikiOpener.OpenGamerEscape(GetDisplayName());
    }

    protected bool isObtained = false;
    public bool GetIsObtained()
    {
        return isObtained;
    }

    public bool IsFavorite()
    {
        var key = GetCollectionName();
        if (!Services.Configuration.Favorites.ContainsKey(key))
        {
            return false;
        }
        return Services.Configuration.Favorites[key].Contains(Id);
    }

    public void SetFavorite(bool favorite)
    {
        var key = GetCollectionName();
        if (!Services.Configuration.Favorites.ContainsKey(key))
        {
            Services.Configuration.Favorites[key] = new();
        }

        if (favorite)
        {
            Dev.Log($"Adding {Id} to Favorites");
            Services.Configuration.Favorites[key].Add(Id);
        }
        else
        {
            Dev.Log($"Removing {Id} from Favorites");
            Services.Configuration.Favorites[key].Remove(Id);
        }
        Services.Configuration.Save();
    }

    public bool IsWishlist()
    {
        var key = GetCollectionName();
        if (!Services.Configuration.Wishlist.ContainsKey(key))
        {
            return false;
        }
        return Services.Configuration.Wishlist[key].Contains(Id);
    }

    public void SetWishlist(bool wishlist)
    {
        var key = GetCollectionName();
        if (!Services.Configuration.Wishlist.ContainsKey(key))
        {
            Services.Configuration.Wishlist[key] = new();
        }

        if (wishlist)
        {
            Dev.Log($"Adding {Id} to Wishlist");
            Services.Configuration.Wishlist[key].Add(Id);
        }
        else
        {
            Dev.Log($"Removing {Id} from Wishlist");
            Services.Configuration.Wishlist[key].Remove(Id);
        }
        Services.Configuration.Save();
    }

    public ISharedImmediateTexture GetIconLazy()
    {
        return IconHandler.GetIconLazy();
    }

    public virtual string GetDisplayName()
    {
        return Name
                .UpperCaseAfterSpaces()
                .LowerCaseWords(new List<string>() { "Of", "Up" });
    }

    public virtual List<CollectibleSortOption> GetSortOptions()
    {
        return SortOptions;
    }
}
