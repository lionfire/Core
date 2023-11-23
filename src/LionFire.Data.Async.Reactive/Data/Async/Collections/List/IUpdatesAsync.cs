namespace LionFire.Data.Collections;

public interface IUpdatesAsync<TItem>
{
    IObservable<(TItem, Task)> Updates { get; }
    Task Update(TItem item);
}

