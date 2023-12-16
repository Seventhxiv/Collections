namespace Collections;

public class EventDataGenerator : BaseDataGenerator<string>
{
    private static readonly string FileName = "ItemIdToEvent.csv";
    protected override void InitializeData()
    {
        var resourceData = CSVHandler.Load<ItemIdToSource>(FileName);
        foreach(var entry in resourceData)
        {
            if (entry.SourceDescription != "")
                AddEntry(entry.ItemId, entry.SourceDescription);
        }
    }
}
