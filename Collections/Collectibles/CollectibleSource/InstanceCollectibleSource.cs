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

    private List<CollectibleSourceCategory> sourceType;
    public override List<CollectibleSourceCategory> GetSourceCategories()
    {
        if (sourceType != null)
        {
            return sourceType;
        }

        sourceType = new List<CollectibleSourceCategory>();
        var contentType = ContentFinderCondition.ContentType;
        switch (contentType.Value.Name.ToString())
        {
            case "PvP":
                sourceType.Add(CollectibleSourceCategory.PvP);
                break;
            case "Treasure Hunt":
                sourceType.Add(CollectibleSourceCategory.TreasureMaps);
                break;
            case "Tribal Quests":
                sourceType.Add(CollectibleSourceCategory.BeastTribe);
                break;
            case "Deep Dungeons":
                sourceType.Add(CollectibleSourceCategory.DeepDungeon);
                break;
            default:
                sourceType.Add(CollectibleSourceCategory.Instance);
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
