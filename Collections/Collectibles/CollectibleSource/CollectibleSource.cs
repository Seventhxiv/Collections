namespace Collections;

public interface ICollectibleSource
{
    public abstract string GetName();
    public abstract List<CollectibleSourceCategory> GetSourceCategories();
    public abstract bool GetIslocatable();
    public abstract LocationEntry GetLocationEntry();
}

public abstract class CollectibleSource : ICollectibleSource
{
    public abstract string GetName();
    public abstract List<CollectibleSourceCategory> GetSourceCategories();
    public abstract bool GetIslocatable();
    public abstract LocationEntry GetLocationEntry();

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
