using Collections;

public class SubmarineSource : CollectibleSource
{
    private SubmarineExploration subMap { get; init; }
    public SubmarineSource(SubmarineExploration subMap)
    {
        this.subMap = subMap;
    }

    public override string GetName()
    {
        var name = subMap.Destination.ToString();
        return "Subaquatic Voyages - " + name;
    }

    private List<SourceCategory> sourceType = new List<SourceCategory>() {SourceCategory.Voyages};
    public override List<SourceCategory> GetSourceCategories()
    {
        return sourceType;
    }

    public static int defaultIconId = 65035; //60850
    protected override int GetIconId()
    {
        return defaultIconId;
    }

    public override bool GetIslocatable()
    {
        return false;
    }

    public override void DisplayLocation()
    {
    }
}
