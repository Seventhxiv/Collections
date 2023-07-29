using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

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

    private List<CollectibleSourceType> sourceType = new List<CollectibleSourceType>() { CollectibleSourceType.Quest };
    public override List<CollectibleSourceType> GetSourceType()
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
