namespace Collections;

public class GlamourSetChangeEventArgs : EventArgs
{
    public GlamourSet GlamourSet { get; init; }
    public bool Preview { get; init; }
    public GlamourSetChangeEventArgs(GlamourSet glamourSet, bool preview)
    {
        GlamourSet = glamourSet;
        Preview = preview;
    }
}

public class GlamourSetChangeEvent : Event<GlamourSetChangeEventArgs>
{
}


