using LuminaSupplemental.Excel.Model;

namespace Collections;

public class ContainersDataGenerator : BaseDataGenerator<uint>
{
    protected override void InitializeData()
    {
        var ItemSupplementList = CsvLoader.LoadResource<ItemSupplement>(CsvLoader.ItemSupplementResourceName, out var failedLines);
        foreach (var entry in ItemSupplementList)
        {
            AddEntry(entry.ItemId, entry.SourceItemId);
        }
    }
}
