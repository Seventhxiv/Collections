using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace Collections;

public class InstanceCollectibleSource : CollectibleSource
{
    private ContentFinderCondition ContentFinderCondition { get; init; }
    public InstanceCollectibleSource(ContentFinderCondition contentFinderCondition)
    {
        ContentFinderCondition = contentFinderCondition;
    }

    public override string GetName()
    {
        return ContentFinderCondition.Name;
    }

    private List<CollectibleSourceType> sourceType;
    public override List<CollectibleSourceType> GetSourceType()
    {
        if (sourceType != null)
        {
            return sourceType;
        }

        sourceType = new List<CollectibleSourceType>();
        var contentType = ContentFinderCondition.ContentType;
        switch (contentType.Value.Name.ToString())
        {
            case "PvP":
                sourceType.Add(CollectibleSourceType.PvP);
                break;
            case "Treasure Hunt":
                sourceType.Add(CollectibleSourceType.TreasureMaps);
                break;
            case "Tribal Quests":
                sourceType.Add(CollectibleSourceType.BeastTribe);
                break;
            case "Deep Dungeons":
                sourceType.Add(CollectibleSourceType.DeepDungeon);
                break;
            default:
                sourceType.Add(CollectibleSourceType.Instance);
                break;
        }
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

    public static int defaultIconId = 60550;
    private int? iconId = null;
    protected override int GetIconId()
    {
        if (iconId != null)
        {
            return (int)iconId;
        }

        var contentType = ContentFinderCondition.ContentType.Value;
        var contentIconId = contentType.Icon;
        if (contentIconId == 0)
        {
            iconId = defaultIconId; // Default
        }
        else
        {
            iconId = (int)contentIconId;
        }
        return (int)iconId;
    }
}
