namespace Collections;

public class MiscCollectibleSource : CollectibleSource
{
    private string misc { get; init; }
    public MiscCollectibleSource(string misc)
    {
        this.misc = misc;
    }

    public override string GetName()
    {
        return misc;
    }

    private readonly List<CollectibleSourceCategory> sourceType = new();
    public override List<CollectibleSourceCategory> GetSourceCategories()
    {
        return sourceType;
    }

    public override bool GetIslocatable()
    {
        return false;
    }

    public override void DisplayLocation()
    {
        throw new NotImplementedException();
    }

    protected override int GetIconId()
    {
        return 60404;
    }
}
