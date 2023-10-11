namespace LionFire.Data.Collections;

// TODO: Rename AsyncDynamicDataCollection to AsyncDynamicDataCollectionRxO, and have Lightweight version of AsyncDynamicDataCollection? It would possibly be without
//  - ReactiveObject base
//  - Value property

public abstract partial class AsyncDynamicDataCollection<TValue>
    : DynamicDataCollection<TValue>
{
    protected object getInProgressLock = new();

    // REVIEW: Move more async only things here?

    public override async ITask<IGetResult<IEnumerable<TValue>>> Get(CancellationToken cancellationToken = default)
    {
        ITask<IGetResult<IEnumerable<TValue>>> task;

        lock (getInProgressLock)
        {
            var existing = getOperations.Value;
            if (existing != null && !existing.AsTask().IsCompleted) { task = existing; }
            else
            {
                task = GetImpl(cancellationToken);
                //task.AsTask().ContinueWith(t => { 
                //    base.OnNext(t.Result); 
                //});
                getOperations.OnNext(task);
            }
        }

        try
        {
            return await task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return GetResult<IEnumerable<TValue>>.Exception(ex);
        }
    }
    protected abstract ITask<IGetResult<IEnumerable<TValue>>> GetImpl(CancellationToken cancellationToken = default);
}
