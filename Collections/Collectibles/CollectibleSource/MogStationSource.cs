namespace Collections;

public class MogStationSource : CollectibleSource
{
    public MogStationSource()
    {
    }

    public override string GetName()
    {
        return "Mog Station Item";
    }

    private readonly List<SourceCategory> sourceType = new() { SourceCategory.MogStation };
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
    }

    public static int iconId = 61831;
    protected override int GetIconId()
    {
        return iconId;
    }

    public override MogStationSource Clone()
    {
        return new MogStationSource();
    }
}
