using LionFire.Structures;

namespace LionFire.Collections.Async;

// Suggested companion interface: IAwareOfSupportedTypes
public interface ICreatesAsync<TKey, TValue>: ICreatesG<TKey, TValue>
{
    IObservable<(string key, Type type, object[] parameters, Task result)> CreatesForKey { get; }
}

// Separated from IAsyncCreates for Orleans grains
public interface ICreatesG<TKey, TValue>
{
    public Task<TValue> CreateForKey(string key, Type type, params object[] constructorParameters);

    public Task<TValue> GetOrCreateForKey(string key, Type type, params object[] constructorParameters);
}
