using ObservableCollections;
using System.Reflection.Metadata.Ecma335;

namespace LionFire.Mvvm;

public interface IAsyncObservableCollectionVM
{
    // ENH Idea: cancel button for the whole VM to cancel all operations in progress: retrieve, add, remove, etc.
    //Task Cancel();
}

public class DelegateCollectionCache<T> : AsyncObservableCollectionCacheBase<T, ObservableList<T>>, IAsyncCollectionCache<T>
{

    public DelegateCollectionCache()
    {
    }

    #region Add

    public Func<T, Task<bool>>? AddAction { get; set; }
    public Func<IEnumerable<T>, Task<bool>>? AddRangeAction { get; set; }
    public Task Add(T item)
    {
        return (AddAction ?? throw new NotSupportedException($"{nameof(AddAction)} must be set")).Invoke(item);
    }
    public Task AddRange(IEnumerable<T> items)
    {
        // TODO: AddRange
        throw new NotImplementedException();
    }
    public virtual bool CanAdd => AddAction != null;

    public IEnumerable<T> Adding => throw new NotImplementedException();

    #endregion

    #region Remove

    public Func<T, Task<bool>>? RemoveAction { get; set; }
    public Task<bool> Remove(T item)
    {
        return (RemoveAction ?? throw new NotSupportedException($"{nameof(RemoveAction)} must be set")).Invoke(item);
    }
    public virtual bool CanRemove => RemoveAction != null;

    public IEnumerable<T> Removing => throw new NotImplementedException();

    #endregion

    #region Retrieve

    public Func<CancellationToken, Task<IEnumerable<T>>>? RetrieveAction { get; set; }

    private Task<IEnumerable<T>>? retrievingTask;
    private object SyncRoot = new();

    public Task<IEnumerable<T>> Retrieve(CancellationToken cancellationToken = default)
    {
        if (RetrieveAction == null) throw new NotSupportedException($"{nameof(RetrieveAction)} must be set in order to invoke Retrieve");

        lock (SyncRoot)
        {
            if (IsRetrieving && /* redundant: */ retrievingTask != null) { return retrievingTask; }
            else
            {
                IsRetrieving = true;
                retrievingTask = Task.Run(async () =>
                {
                    try
                    {
                        var retrievedItems = await RetrieveAction.Invoke(cancellationToken);
                        HasRetrieved = true;

                        if (collection == null)
                        {
                            collection = new ObservableList<T>(retrievedItems);
                            // TODO: Raise reset event
                        }
                        else
                        {
                            var additions = new List<T>();
                            var removals = new List<T>();
                            foreach (var item in retrievedItems)
                            {
                                if (!collection.Contains(item)) additions.Add(item);
                            }
                            foreach (var item in collection)
                            {
                                if (!retrievedItems.Contains(item))
                                {
                                    removals.Add(item);
                                }
                            }
                            foreach (var item in removals) { collection.Remove(item); }
                            collection.AddRange(additions);

                            //if (additions.Count > 0 || removals.Count > 0)
                            //{
                            //    GlobalItemsChanged(this, EffectiveKey);
                            //}
                        }
                        return retrievedItems;
                    }
                    finally
                    {
                        lock (SyncRoot)
                        {
                            IsRetrieving = false;
                            retrievingTask = null;
                        }
                    }
                });
                return retrievingTask;
            }
        }
    }

    #endregion

    public void Clear() // Make public?
    {
        HasRetrieved = false;
        collection?.Clear();
    }

}
