namespace Collections;

public class GlamourItemChangeEventArgs : EventArgs
{
    public GlamourCollectible Collectible { get; init; }
    public GlamourItemChangeEventArgs(GlamourCollectible collectible)
    {
        Collectible = collectible;
    }
}

public class GlamourItemChangeEvent : Event<GlamourItemChangeEventArgs>
{
}


