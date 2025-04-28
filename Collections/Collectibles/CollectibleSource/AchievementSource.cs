namespace Collections;

public class AchievementSource : CollectibleSource
{
    private Achievement Achievement { get; init; }
    public AchievementSource(Achievement achievement)
    {
        Achievement = achievement;
    }

    public override string GetName()
    {
        return Achievement.Name.ToString() + ": " + Achievement.Description.ToString();
    }

    private List<SourceCategory> sourceType;
    public override List<SourceCategory> GetSourceCategories()
    {
        if (sourceType != null)
        {
            return sourceType;
        }

        var achievementKind = Achievement.AchievementCategory.Value.AchievementKind;
        sourceType = new List<SourceCategory>() { SourceCategory.Achievement };
        switch (achievementKind.Value.RowId)
        {
            case 2: // PvP
                sourceType.Add(SourceCategory.PvP);
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
