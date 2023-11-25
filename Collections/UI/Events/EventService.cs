using System.Runtime.CompilerServices;

namespace Collections;

public class EventService
{
    private Dictionary<Type, Func<Object>> events = new();

    private T GetEvent<T, TArgs>()
        where T : Event<TArgs>, new()
        where TArgs : EventArgs
    {
        if (events.ContainsKey(typeof(T)))
        {
            return events[typeof(T)]() as T;
        }

        var presEvent = new T();
        events.Add(typeof(T), () => presEvent);

        return presEvent;
    }

    public void Subscribe<T, TArgs>(Action<TArgs> action, [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
        where T : Event<TArgs>, new()
        where TArgs : EventArgs
    {
        var presEvent = GetEvent<T, TArgs>();
        Dev.Log($"subscribed to {typeof(T)}", 2);
        presEvent.OnPublish += new Event<TArgs>.Delegate(action);
    }

    public void Publish<T, TArgs>(TArgs eventArgs, [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
        where T : Event<TArgs>, new()
        where TArgs : EventArgs
    {
        var presEvent = GetEvent<T, TArgs>();
        Dev.Log($"published {typeof(T)}");
        presEvent.Publish(eventArgs);
    }
}
