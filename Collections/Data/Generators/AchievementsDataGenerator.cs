namespace Collections;

public class AchievementsDataGenerator
{
    public readonly Dictionary<uint, Achievement> itemToAchievement = new();

    public AchievementsDataGenerator()
    {
        //Dev.Start();
        populateData();
        //Dev.Stop();
    }

    private void populateData()
    {
        var achievementSheet = ExcelCache<Achievement>.GetSheet();
        foreach (var achievement in achievementSheet)
        {
            var itemId = achievement.Item.Row;
            itemToAchievement[itemId] = achievement;
        }
    }
}
