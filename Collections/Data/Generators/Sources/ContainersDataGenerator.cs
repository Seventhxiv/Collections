using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.Excel.Services;

namespace Collections;

public class ContainersDataGenerator : BaseDataGenerator<uint>
{
    protected override void InitializeData()
    {
        var ItemSupplementList = CsvLoader.LoadResource<ItemSupplement>(CsvLoader.ItemSupplementResourceName, out var failedLines, out var exceptions);
        foreach (var entry in ItemSupplementList)
        {
            AddEntry(entry.ItemId, entry.SourceItemId);
        }
    }
}
