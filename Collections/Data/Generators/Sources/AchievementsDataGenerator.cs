namespace Collections;

public class AchievementsDataGenerator : BaseDataGenerator<Achievement>
{
    private static readonly string FileName = "ItemIdToAchievement.csv";
    protected override void InitializeData()
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
}
