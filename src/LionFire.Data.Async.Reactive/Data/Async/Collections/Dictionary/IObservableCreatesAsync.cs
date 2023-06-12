using LionFire.Structures;

namespace LionFire.Collections.Async;

// Suggested companion interface: IAwareOfSupportedTypes
public interface IObservableCreatesAsync<TKey, TValue>: ICreatesAsync<TKey, TValue>
{
    IObservable<(TKey key, Type type, object[] parameters, Task result)> CreatesForKey { get; }
}
