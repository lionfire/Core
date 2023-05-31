namespace LionFire.Orleans_.Collections;

public class KeyedListG<TKey, TItem>
{

    #region State (persisted)

    protected IPersistentState<List<TItem>> ItemsState { get; }

    #endregion

    #region Lifecycle

    public KeyedListG()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region List: read

    public Task<TItem> ElementAt(int index) => Task.FromResult(ItemsState.State[index]);

    public Task ElementAt(int index, TItem value)
    {
        ItemsState.State[index] = value;
        return ItemsState.WriteStateAsync();
    }

    public Task<int> IndexOf(TItem item) => Task.FromResult(ItemsState.State.IndexOf(item));

    #endregion

    #region List: write

    public Task Insert(int index, TItem item)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Remove

    public async virtual Task<bool> Remove(TKey key)
    {
        throw new NotImplementedException();

        //var existing = ItemsState.State.Where(m => EqualityComparer<TItem>.Default.Equals(m, item)).FirstOrDefault();
        //if (existing != null)
        //{
        //    // TOTRANSACTION

        //    Task? deleteTask = null;

        //    //var key = GetKey(existing);

        //    if (DeletedItemsState != null) // TrackDeletedItems
        //    {
        //        DeletedItemsState.State.Add(DateTime.UtcNow, existing);
        //        deleteTask = DeletedItemsState.WriteStateAsync();
        //    }

        //    ItemsState.State.Remove(existing);
        //    await ItemsState.WriteStateAsync();
        //    await PublishCollectionChanged(new ChangeSet<TItem>(new List<Change<TItem>> { new Change<TItem>(ChangeReason.Remove, existing) }));
        //    //PublishCollectionChanged(new NotifyCollectionChangedEventArgs<TNotificationItem>(System.Collections.Specialized.NotifyCollectionChangedAction.Remove, existing));

        //    if (deleteTask != null) { await deleteTask; }
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
    }

    //public Task<bool> Remove(TItem item) => Remove(KeySelector(item));   

    public Task RemoveAt(int index)
    {
        ItemsState.State.RemoveAt(index);
        return ItemsState.WriteStateAsync();
    }

    #endregion
}
