namespace LionFire.Data.Async.Collections;


public interface IAddsAsync<TItem>
{
    IObservable<(TItem, Task)> Adds { get; }
    //IEnumerable<TItem> Adding { get; } // TODO: change return type to IObservable<IEnumerable<TValue>>
    Task Add(TItem item);
    Task AddRange(IEnumerable<TItem> item);
}

