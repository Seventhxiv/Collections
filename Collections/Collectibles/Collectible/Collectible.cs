using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.Excel.Services;

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
    // keep as decimal; Version sucks for how FF14 handles versioning.
    public decimal PatchAdded {get; init; }

    protected abstract int GetIconId();
    protected abstract uint GetId();
    protected abstract string GetName();
    protected abstract string GetDescription();
    protected abstract HintModule GetSecondaryHint();
    protected abstract string GetCollectionName();

    public ICollectibleKey CollectibleKey { get; init; }
    public T ExcelRow { get; set; }
    protected IconHandler IconHandler { get; init; }
    protected List<CollectibleSortOption> SortOptions = [
        new CollectibleSortOption(
            "Patch", 
            Comparer<ICollectible>.Create((c1, c2) => c2.PatchAdded.CompareTo(c1.PatchAdded)),
            false,
            (FontAwesomeIcon.SortNumericDown, FontAwesomeIcon.SortNumericUp)
        ),
        new CollectibleSortOption(
            "Name",
            Comparer<ICollectible>.Create((c1, c2) => c1.Name.CompareTo(c2.Name)),
            false,
            (FontAwesomeIcon.SortAlphaUp, FontAwesomeIcon.SortAlphaDown)
        ),
        // comparing c2 to c1 to modify default sort behavior
        new CollectibleSortOption(
            "Obtained",
            Comparer<ICollectible>.Create((c1, c2) => c2.GetIsObtained().CompareTo(c1.GetIsObtained())),
            false,
            null
        )
    ];

    public Collectible(T excelRow)
    {
        ExcelRow = excelRow;
        Id = GetId();
        CollectibleKey = CollectibleKeyFactory.Get(this);
        Name = GetName();
        Description = GetDescription();
        if (CollectibleKey is null)
        {
            Dev.Log($"Missing collectible key: {Name} ({Id})");
        }
        PatchAdded = GetPatchAdded();
        PrimaryHint = GetPrimaryHint();
        SecondaryHint = GetSecondaryHint();
        IconHandler = new IconHandler(GetIconId());
    }

    public virtual void OpenGamerEscape()
    {
        WikiOpener.OpenGamerEscape(GetDisplayName());
    }

    protected virtual HintModule GetPrimaryHint()
    {
        return new HintModule($"Patch {GetDisplayPatch()}", FontAwesomeIcon.Hashtag);
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
                .LowerCaseWords(new List<string>() { "Of", "Up", "The"});
    }

    public virtual List<CollectibleSortOption> GetSortOptions()
    {
        return SortOptions;
    }

    protected virtual decimal GetPatchAdded()
    {
        decimal temp = (decimal)0.0;
        if(CollectibleKey != null)
        {
            // find patch added to the game
            foreach(var patch in CsvLoader.LoadResource<ItemPatch>(CsvLoader.ItemPatchResourceName, true, out var failedLines, out var exceptions))
            {
                if( CollectibleKey.Id >= patch.StartItemId && CollectibleKey.Id <= patch.EndItemId )
                {
                    temp = patch.PatchNo;
                }
            }
        }
        return temp;
    }

    public virtual string GetDisplayPatch()
    {
        return PatchAdded <= 0 ? "Unknown" : PatchAdded.ToString();
    }
}