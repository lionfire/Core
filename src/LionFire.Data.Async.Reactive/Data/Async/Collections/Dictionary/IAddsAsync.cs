namespace LionFire.Data.Async.Collections;

public interface IAddsAsync<TKey, TItem>
{
    bool CanAdd { get; }
    IObservable<(KeyValuePair<TKey, TItem> parameter, Task result)> Adds { get; }
    Task Add(TKey key, TItem item);
    Task AddRange(IEnumerable<KeyValuePair<TKey, TItem>> item);
}

