namespace LionFire.Collections.Async;

public interface IUpdatesAsync<TItem>
{
    IObservable<(TItem, Task)> Updates { get; }
    Task Update(TItem item);
}

