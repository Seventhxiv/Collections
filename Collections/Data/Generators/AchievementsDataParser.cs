using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace Collections;

public class AchievementsDataParser
{
    public readonly Dictionary<uint, Achievement> itemToAchievement = new();

    public AchievementsDataParser()
    {
        Dev.StartStopwatch();
        populateData();
        Dev.EndStopwatch();
    }

    private void populateData()
    {
        var achievementSheet = Excel.GetExcelSheet<Achievement>();
        foreach (var achievement in achievementSheet)
        {
            var itemId = achievement.Item.Row;
            itemToAchievement[itemId] = achievement;
        }
    }
}
