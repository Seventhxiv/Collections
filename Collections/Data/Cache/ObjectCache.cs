using System.Collections.Concurrent;

namespace Collections;

public interface ICreateable<TOut, TIn>
{
    abstract static TOut Create(TIn excelRow);
}

public abstract class ObjectCache<TObjectCache, TObj, TInput, TKey>
    where TObj : ICreateable<TObj, TInput>
    where TObjectCache : new()
{
    private static TObjectCache? InternalInstance;
    public static TObjectCache Instance => InternalInstance ??= new TObjectCache();
    private readonly ConcurrentDictionary<TKey, TObj> cache = new();

    protected abstract TKey GetKey(TInput input);
    protected abstract TInput GetInput(TKey key);

    public TObj GetObject(TInput input)
    {
        var key = GetKey(input);
        if (cache.TryGetValue(key, out var value))
        {
            return value;
        }
        return cache[key] = TObj.Create(input);
    }

    public TObj GetObject(TKey key)
    {
        if (cache.TryGetValue(key, out var value))
        {
            return value;
        }
        var input = GetInput(key);
        return cache[key] = TObj.Create(input);
    }
}
