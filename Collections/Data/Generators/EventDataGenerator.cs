namespace Collections;

public class EventDataGenerator
{
    public Dictionary<uint, List<string>> itemsToEvents = new();

    private static readonly string FileName = "ItemIdToEvent.csv";
    public EventDataGenerator()
    {
        PopulateData();
    }

    private void PopulateData()
    {
        var resourceData = CSVHandler.Load<ItemIdToSource>(FileName);
        itemsToEvents = resourceData
            .AsParallel().Where(entry => entry.SourceDescription != "")
            .GroupBy(entry => entry.ItemId)
            .ToDictionary(kv => kv.Key, kv => kv.Select(e => e.SourceDescription).ToList());
    }
}
