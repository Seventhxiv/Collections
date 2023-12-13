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

    private readonly List<CollectibleSourceCategory> sourceType = new() { CollectibleSourceCategory.Event };
    public override List<CollectibleSourceCategory> GetSourceCategories()
    {
        return sourceType;
    }

    public override bool GetIslocatable()
    {
        return true;
    }

    public override void DisplayLocation()
    {
        WikiOpener.OpenGamerEscape(eventName);
    }

    public static int iconId = 61757;
    protected override int GetIconId()
    {
        return iconId;
    }
}
