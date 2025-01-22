namespace Collections;

public abstract class BaseDataGenerator<T>
{
    public readonly Dictionary<uint, HashSet<T>> data = new();
    protected abstract void InitializeData();

    public BaseDataGenerator()
    {
        //Dev.Start();
        InitializeData();
        //Dev.Stop();
    }

    protected void AddEntry(uint id, T source)
    {
        if (!data.ContainsKey(id))
            data[id] = new HashSet<T>();

        data[id].Add(source);
    }

    protected void RemoveEntry(uint id, T source)
    {
        if (data.TryGetValue(id, out var sources))
        {
            sources.Remove(source);
            if (sources.Count == 0)
            {
                data.Remove(id);
            }
        }
    }
}
