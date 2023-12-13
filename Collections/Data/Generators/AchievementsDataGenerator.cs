namespace Collections;

public class AchievementsDataGenerator
{
    public readonly Dictionary<uint, HashSet<Achievement>> itemToAchievement = new();

    public AchievementsDataGenerator()
    {
        populateData();
    }

    private static readonly string FileName = "ItemIdToAchievement.csv";
    private void populateData()
    {
        // Based on sheet
        var achievementSheet = ExcelCache<Achievement>.GetSheet();
        foreach (var achievement in achievementSheet)
        {
            AddEntry(achievement.Item.Row, achievement);
        }

        // Based on resource data
        var resourceData = CSVHandler.Load<ItemIdToSource>(FileName);
        foreach (var entry in resourceData)
        {
            if (entry.SourceId == 0)
                continue;

            AddEntry(entry.ItemId, achievementSheet.GetRow(entry.SourceId));
        }
    }

    private void AddEntry(uint itemId, Achievement achievement)
    {
        if (!itemToAchievement.ContainsKey(itemId))
        {
            itemToAchievement[itemId] = new HashSet<Achievement>();
        }
        itemToAchievement[itemId].Add(achievement);
    }
}
