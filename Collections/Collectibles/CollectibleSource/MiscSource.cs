namespace Collections;

public class MiscSource : CollectibleSource
{
    private string misc { get; init; }
    public MiscSource(string misc)
    {
        this.misc = misc;
    }

    public override string GetName()
    {
        return misc;
    }

    private readonly List<SourceCategory> sourceType = new();
    public override List<SourceCategory> GetSourceCategories()
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
