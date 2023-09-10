using LionFire.Data.Async.Sets;
using LionFire.Structures;

namespace LionFire.Data.Collections;

// Suggested companion interface: IAwareOfSupportedTypes
public interface IObservableCreatesAsync<TKey, TValue>: ICreatesAsync<TKey, TValue>
{
    IObservable<(TKey key, Type type, object[] parameters, Task result)> CreatesForKey { get; }
}

public interface IObservableCreatesAsync<TValue> : ICreatesAsync<TValue>
{
    IObservable<(Type type, object[] parameters, Task result)> Creates { get; }
}
