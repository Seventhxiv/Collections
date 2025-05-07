using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.Excel.Services;

namespace Collections;

public class SubmarineDataGenerator : BaseDataGenerator<SubmarineExploration>
{
    protected override void InitializeData()
    {
        // Based on sheet
        var subDropsList = CsvLoader.LoadResource<SubmarineDrop>(CsvLoader.SubmarineDropResourceName, true, out var failedLines, out var exceptions);
        foreach (var entry in subDropsList)
        {
            var subMap = ExcelCache<SubmarineExploration>.GetSheet().GetRow(entry.SubmarineExplorationId);
            if(subMap is not null)
            {
                AddEntry(entry.ItemId, (SubmarineExploration)subMap);
            }
        }
    }
}