namespace Collections;

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

    // TODO: replace with PvPSeries icon
    public static int iconId = 000006; //61501;
    protected override int GetIconId()
    {
        return iconId;
    }
}
