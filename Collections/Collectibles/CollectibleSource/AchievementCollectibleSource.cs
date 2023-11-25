
namespace Collections;

public class AchievementCollectibleSource : CollectibleSource
{
    private Achievement Achievement { get; init; }
    public AchievementCollectibleSource(Achievement achievement)
    {
        Achievement = achievement;
    }

    public override string GetName()
    {
        return Achievement.Name + ": " + Achievement.Description;
    }

    private List<CollectibleSourceCategory> sourceType;
    public override List<CollectibleSourceCategory> GetSourceCategories()
    {
        if (sourceType != null)
        {
            return sourceType;
        }

        var achievementKind = Achievement.AchievementCategory.Value.AchievementKind;
        sourceType = new List<CollectibleSourceCategory>() { CollectibleSourceCategory.Achievement };
        switch (achievementKind.Value.Name)
        {
            case "PvP":
                sourceType.Add(CollectibleSourceCategory.PvP);
                break;
        }
        return sourceType;
    }

    public override bool GetIslocatable()
    {
        return false;
    }

    public override void DisplayLocation()
    {
    }

    public static int iconId = 000006; //61501;
    protected override int GetIconId()
    {
        return iconId;
    }
}
