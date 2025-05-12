namespace Collections;

public class PvPSeriesDataGenerator : BaseDataGenerator<(PvPSeries, int)>
{
    protected override void InitializeData()
    {
        // Based on sheet
        var pvpSeries = ExcelCache<PvPSeries>.GetSheet();
        foreach (var season in pvpSeries)
        {
            // so we can add the correct level
            for(int level = 0; level < season.LevelRewards.Count; level++)
            {
                var reward = season.LevelRewards[level];
                foreach(var indv_reward in reward.LevelRewardItem)
                {
                    // trophies are reward 0;
                    if(indv_reward.RowId != 0)
                    {
                        AddEntry(indv_reward.RowId, (season, level));
                    }
                }
            }
        }

        // // Based on resource data
        // var resourceData = CSVHandler.Load<ItemIdToSource>(FileName);
        // foreach (var entry in resourceData)
        // {
        //     if (entry.SourceId == 0)
        //         continue;

        //     var achievement = achievementSheet.GetRow(entry.SourceId);
        //     if (achievement != null)
        //     {
        //         AddEntry(entry.ItemId, (Achievement)achievement);
        //     }
        // }
    }
}