using System.Collections.Generic;
using System.Linq;

namespace Collections;

public class CollectibleToEvent
{
    public string SheetName { get; set; }
    public uint id { get; set; }
    public string Name { get; set; }
    public string Events { get; set; }
    public bool isMogstation { get; set; }
}

public class EventDataParser
{
    public Dictionary<uint, List<string>> itemsToEvents = new();

    private static readonly string CollectibleToEventPath = @"D:\FFXIV Plugin Dev\Collections-master\Collections\Data\Resources\collectibleToEvent.csv";
    public EventDataParser()
    {
        Dev.StartStopwatch();

        PopulateData();
        Dev.EndStopwatch();
    }

    private void PopulateData()
    {
        var gamerEscapeItemToCSVList = Helpers.LoadCSV<CollectibleToEvent>(CollectibleToEventPath);
        itemsToEvents = gamerEscapeItemToCSVList
            .AsParallel().Where(entry => entry.Events != "")
            .ToDictionary(value => value.id, value => value.Events.Split(",", 10).ToList());
    }
}
