namespace Collections;

public class FilterChangeEventArgs : EventArgs
{
    public FilterChangeEventArgs()
    {
    }
}

public class FilterChangeEvent : Event<FilterChangeEventArgs>
{
}


