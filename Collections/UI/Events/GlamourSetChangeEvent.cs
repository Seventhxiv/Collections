using System;

namespace Collections;

public class GlamourSetChangeEventArgs : EventArgs
{
    public GlamourSet GlamourSet { get; init; }
    public GlamourSetChangeEventArgs(GlamourSet glamourSet)
    {
        GlamourSet = glamourSet;
    }
}

public class GlamourSetChangeEvent : Event<GlamourSetChangeEventArgs>
{
}


