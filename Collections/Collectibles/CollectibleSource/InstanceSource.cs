using Collections.Executors;

namespace Collections;

public class InstanceSource : CollectibleSource
{
    private ContentFinderCondition ContentFinderCondition { get; init; }
    public InstanceSource(ContentFinderCondition contentFinderCondition)
    {
        ContentFinderCondition = contentFinderCondition;
    }

    public override string GetName()
    {
        return ContentFinderCondition.Name.ToString();
    }

    private List<SourceCategory> sourceType;
    public override List<SourceCategory> GetSourceCategories()
    {
        if (sourceType != null)
        {
            return sourceType;
        }

        sourceType = new List<SourceCategory>();
        var contentType = ContentFinderCondition.ContentType;
        switch (contentType.Value.RowId)
        {
            case 6:
                sourceType.Add(SourceCategory.PvP);
                break;
            case 9:
                sourceType.Add(SourceCategory.TreasureHunts);
                break;
            case 13:
                sourceType.Add(SourceCategory.BeastTribes);
                break;
            case 21:
                sourceType.Add(SourceCategory.DeepDungeon);
                break;
            default:
                sourceType.Add(SourceCategory.Duty);
                break;
        }
        return sourceType;
    }

    public override bool GetIslocatable()
    {
        return true;
    }

    public override void DisplayLocation()
    {
        DutyFinderOpener.OpenRegularDuty(ContentFinderCondition.RowId);
    }

    public static int defaultIconId = 060414;
    protected override int GetIconId()
    {
        var contentType = ContentFinderCondition.ContentType.Value;
        var contentIconId = contentType.Icon;
        if (contentIconId == 0)
        {
            return defaultIconId;
        }
        else
        {
            return (int)contentIconId;
        }
    }
}
