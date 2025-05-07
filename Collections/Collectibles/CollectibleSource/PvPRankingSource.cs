namespace Collections;

// Mainly for old Feast rewards and (as of 7.2) Crystal Conflict Ranking rewards.
public class PvPRankingSource : CollectibleSource
{
    private string description{ get; init; }
    public PvPRankingSource(string description)
    {
        this.description = description;
    }

    public override string GetName()
    {
        return description; 
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

    public static int iconId = 009058;
    protected override int GetIconId()
    {
        return iconId;
    }

    public override PvPRankingSource Clone()
    {
        return new PvPRankingSource(description);
    }
}
