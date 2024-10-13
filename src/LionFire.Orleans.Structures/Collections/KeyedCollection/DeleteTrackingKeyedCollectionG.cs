using DynamicData;
using LionFire.Threading;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;

namespace LionFire.Orleans_.Collections;

public class DeleteTrackingKeyedCollectionG<TKey, TValue> : KeyedCollectionG<TKey, TValue>
    , IDeleteTrackingListGrain<TValue>
    where TKey : notnull
    where TValue : notnull
{
    #region State (persisted)

    protected IPersistentState<SortedDictionary<DateTime, TKey>> DeletedItemsState { get; private set; }

    #endregion

    #region Configuration

    public const bool TrackDeletedItems = true; // REFACTOR
    //{
    //    get => DeletedItemsState != null;
    //    set
    //    {
    //        if (value)
    //        {
    //            if (DeletedItemsState == null) throw new InvalidOperationException();
    //        }
    //        else
    //        {
    //            DeletedItemsState = null;
    //        }
    //    }
    //}

    #endregion

    #region Lifecycle

    public DeleteTrackingKeyedCollectionG(IServiceProvider serviceProvider, IPersistentState<Dictionary<TKey,TValue>> items, ILogger<KeyedCollectionG<TKey, TValue>> logger, IPersistentState<SortedDictionary<DateTime, TKey>> deletedItemsState) : base(serviceProvider, items, logger)
    {
        DeletedItemsState = deletedItemsState;
    }

    #endregion

    #region Instantiation

    protected override TValue Instantiate(Type type)
    {
        TValue result;
        TKey key;

        do
        {
            result = base.Instantiate(type);
            key = KeySelector(result);
        } while (ItemsState.State.Where(m =>
            EqualityComparer<TKey>.Default.Equals(m.Key, key)).Any()
            || (DeletedItemsState != null
                && DeletedItemsState.State.Where(deletedKey =>
                    EqualityComparer<TKey>.Default.Equals(deletedKey.Value, key)).Any()));

        return result;
    }

    #endregion

    #region Deleted Items

    public Task<IEnumerable<KeyValuePair<DateTime, TValue>>> DeletedKeys() => Task.FromResult(
        (DeletedItemsState?.State as IEnumerable<KeyValuePair<DateTime, TValue>> ?? Enumerable.Empty<KeyValuePair<DateTime, TValue>>())
        );

    #region Deleted Items: Pruning

    public TimeSpan DeletedItemExpiry { get; set; } = TimeSpan.FromDays(90);

    public async Task PruneDeletedItems()
    {
        KeyValuePair<DateTime, TKey> first;
        var list = DeletedItemsState?.State;
        if (list == null) { return; } // !TrackDeletedItems

        bool didSomething = false;
        for (; list.Count > 0;)
        {
            first = list.First();
            if (DateTime.UtcNow - first.Key > DeletedItemExpiry)
            {
                list.Remove(first.Key);
                didSomething = true;
            }
        }
        if (didSomething)
        {
            await DeletedItemsState!.WriteStateAsync();
        }
    }

    #endregion

    #endregion

    #region List: write

    public async override Task<bool> Remove(TKey key)
    {
        var result = ItemsState.State.Remove(key);
        if (result && TrackDeletedItems)
        {
            DeletedItemsState.State.Add(DateTime.UtcNow, key);
            await Task.WhenAll(DeletedItemsState.WriteStateAsync() ?? Task.CompletedTask, ItemsState.WriteStateAsync());
        }

        //await PublishCollectionChanged(new ChangeSet<TValue, TKey>(new Change<TValue, TKey>[] { new Change<TValue, TKey>(ChangeReason.Remove, key, default) }));

        return result;
    }

    #endregion
}
