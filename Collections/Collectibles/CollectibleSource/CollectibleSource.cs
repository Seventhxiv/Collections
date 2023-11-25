namespace Collections;

public interface ICollectibleSource
{
    public string GetName();
    public List<CollectibleSourceCategory> GetSourceCategories();
    public bool GetIslocatable();
    public void DisplayLocation();
}

public abstract class CollectibleSource : ICollectibleSource
{
    public abstract string GetName();
    public abstract List<CollectibleSourceCategory> GetSourceCategories();
    public abstract bool GetIslocatable();
    public abstract void DisplayLocation();

    protected abstract int GetIconId();
    protected IconHandler IconHandler { get; set; }

    public CollectibleSource()
    {
    }

    public IDalamudTextureWrap GetIconLazy()
    {
        if (IconHandler == null)
        {
            IconHandler = new IconHandler(GetIconId());
        }
        return IconHandler.GetIconLazy();
    }
}
