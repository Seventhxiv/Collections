namespace Collections;

public class MogStationCollectibleSource : CollectibleSource
{
    public MogStationCollectibleSource()
    {
    }

    public override string GetName()
    {
        return "Mog Station Item";
    }

    private readonly List<CollectibleSourceCategory> sourceType = new() { CollectibleSourceCategory.MogStation };
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
    }

    public static int iconId = 61831;
    protected override int GetIconId()
    {
        return iconId;
    }
}
