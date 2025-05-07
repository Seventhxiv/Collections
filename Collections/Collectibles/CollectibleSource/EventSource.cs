namespace Collections;

public class EventSource : CollectibleSource
{
    private string eventName { get; init; }
    public EventSource(string eventName)
    {
        this.eventName = eventName;
    }

    public override string GetName()
    {
        return eventName;
    }

    private readonly List<SourceCategory> sourceType = new() { SourceCategory.Event };
    public override List<SourceCategory> GetSourceCategories()
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

    public override EventSource Clone()
    {
        return new EventSource(eventName);
    }
}
