namespace Collections;

public class QuestCollectibleSource : CollectibleSource
{
    private Quest Quest { get; init; }
    public QuestCollectibleSource(Quest quest)
    {
        Quest = quest;
    }

    public override string GetName()
    {
        return Quest.Name;
    }

    private List<CollectibleSourceCategory> sourceType = new List<CollectibleSourceCategory>() { CollectibleSourceCategory.Quest };
    public override List<CollectibleSourceCategory> GetSourceCategories()
    {
        return sourceType;
    }

    public override bool GetIslocatable()
    {
        return false;
    }

    public override LocationEntry GetLocationEntry()
    {
        return null;
    }

    public static int iconId = 61419;
    protected override int GetIconId()
    {
        return iconId;
    }
}
