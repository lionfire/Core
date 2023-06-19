using LionFire.Structures;

namespace LionFire.Data.Async.Collections;

// Suggested companion interface: IAwareOfSupportedTypes
public interface IObservableCreatesAsync<TKey, TValue>: ICreatesAsync<TKey, TValue>
{
    IObservable<(TKey key, Type type, object[] parameters, Task result)> CreatesForKey { get; }
}
