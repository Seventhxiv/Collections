namespace Collections;

public abstract class Event<T> where T : EventArgs
{
    public delegate void Delegate(T eventArgs);
    public event Delegate OnPublish;

    public void Publish(T eventArgs)
    {
        if (OnPublish != null)
            OnPublish(eventArgs);
    }
}

public interface ISubscriber<T> where T : EventArgs
{
    public void OnPublish(T args);
}