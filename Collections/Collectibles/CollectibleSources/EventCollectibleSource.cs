using System.Collections.Generic;

namespace Collections;

public class EventCollectibleSource : CollectibleSource
{
    private string eventName { get; init; }
    public EventCollectibleSource(string eventName)
    {
        this.eventName = eventName;
    }

    public override string GetName()
    {
        return eventName;
    }

    private readonly List<CollectibleSourceType> sourceType = new() { CollectibleSourceType.Event };
    public override List<CollectibleSourceType> GetSourceType()
    {
        return sourceType;
    }

    public override bool GetIslocatable()
    {
        return false;
    }

    public override LocationEntry GetLocationEntry()
    {
        return null;
    }

    public static int iconId = 61757;
    protected override int GetIconId()
    {
        return iconId;
    }
}
