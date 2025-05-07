namespace Collections;

// Used to represent collectibles obtained from PvP Series - should be populated before shop items.
public class PvPSeriesSource : CollectibleSource
{
    private PvPSeries PvPSeries { get; init; }
    private int PvPSeriesLevel {get; init; }
    public PvPSeriesSource(PvPSeries series, int level)
    {
        PvPSeries = series;
        PvPSeriesLevel = level;
    }

    public override string GetName()
    {
        return $"PvP Series {PvPSeries.RowId} - Level {PvPSeriesLevel}";
    }

    private readonly List<SourceCategory> sourceType = new() { SourceCategory.PvP };
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

    public static int iconId = 061806; //alternative is 009058;
    protected override int GetIconId()
    {
        return iconId;
    }

    public override PvPSeriesSource Clone()
    {
        return new PvPSeriesSource(PvPSeries, PvPSeriesLevel);
    }
}
