using System;

namespace Collections;

public class GlamourItemChangeEventArgs : EventArgs
{
    public GlamourItem GlamourItem { get; init; }
    public GlamourItemChangeEventArgs(GlamourItem glamourItem)
    {
        GlamourItem = glamourItem;
    }
}

public class GlamourItemChangeEvent : Event<GlamourItemChangeEventArgs>
{
}


