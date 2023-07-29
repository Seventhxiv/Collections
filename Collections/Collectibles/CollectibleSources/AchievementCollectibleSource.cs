
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace Collections;

public class AchievementCollectibleSource : CollectibleSource
{
    private Achievement Achievement { get; init; }
    public AchievementCollectibleSource(Achievement achievement)
    {
        this.Achievement = achievement;
    }

    public override string GetName()
    {
        return Achievement.Name + ": " + Achievement.Description;
    }

    private List<CollectibleSourceType> sourceType;
    public override List<CollectibleSourceType> GetSourceType()
    {
        if (sourceType != null)
        {
            return sourceType;
        }

        var achievementKind = Achievement.AchievementCategory.Value.AchievementKind;
        sourceType = new List<CollectibleSourceType>() { CollectibleSourceType.Achievement };
        switch (achievementKind.Value.Name)
        {
            case "PvP":
                sourceType.Add(CollectibleSourceType.PvP);
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

    public static int iconId = 61501;
    protected override int GetIconId()
    {
        return iconId;
    }
}
