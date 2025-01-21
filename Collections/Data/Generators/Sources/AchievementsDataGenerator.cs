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
            AddEntry(achievement.Item.RowId, achievement);
        }

        // Based on resource data
        var resourceData = CSVHandler.Load<ItemIdToSource>(FileName);
        foreach (var entry in resourceData)
        {
            if (entry.SourceId == 0)
                continue;

            var achievement = achievementSheet.GetRow(entry.SourceId);
            if (achievement != null)
            {
                AddEntry(entry.ItemId, (Achievement)achievement);
            }
        }
    }
}
