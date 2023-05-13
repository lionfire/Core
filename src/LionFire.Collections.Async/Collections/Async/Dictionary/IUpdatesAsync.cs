namespace LionFire.Collections.Async;


public interface IUpdatesAsync<TKey, TItem>
{
    IObservable<(TKey key, TItem item, Task result)> Updates { get; }
    Task Update(TKey key, TItem item);
}

