
namespace Collections;

public abstract class CollectibleSource : ICollectibleSource
{
    public abstract string GetName();
    public abstract List<SourceCategory> GetSourceCategories();
    public abstract bool GetIslocatable();
    public abstract void DisplayLocation();

    protected abstract int GetIconId();

    protected IconHandler IconHandler { get; set; }

    public CollectibleSource()
    {
    }

    public ISharedImmediateTexture GetIconLazy()
    {
        if (IconHandler == null)
        {
            IconHandler = new IconHandler(GetIconId());
        }
        return IconHandler.GetIconLazy();
    }

    public abstract CollectibleSource Clone();

    ICollectibleSource ICollectibleSource.Clone()
    {
        return Clone();
    }
}
