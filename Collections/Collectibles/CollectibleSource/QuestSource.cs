namespace Collections;

public class QuestSource : CollectibleSource
{
    private Quest Quest { get; init; }
    public QuestSource(Quest quest)
    {
        Quest = quest;
    }

    public override string GetName()
    {
        return Quest.Name.ToString();
    }

    private List<SourceCategory> sourceType = new List<SourceCategory>() { SourceCategory.Quest };
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

    public static int iconId = 61419;
    protected override int GetIconId()
    {
        return iconId;
    }
}
