
using DynamicData;
using LionFire.Collections;
using LionFire.DependencyInjection;
using LionFire.Resolves;
using LionFire.Structures;
using LionFire.Threading;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Streams;
using Orleans.Utilities;
using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using static LionFire.Reflection.GetMethodEx;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LionFire.Orleans_.Collections;

#region  FUTURE: Refactor the IPolymorphicListGrainItem<> wrapping in another class:

//public interface IPolymorphicListGrain<TNotificationItem>
//{
//}

//public abstract class PolymorphicListGrain<TNotificationItem> : ListG<IPolymorphicListGrainItem<TNotificationItem>>, IPolymorphicListGrain<TNotificationItem>
//    where TNotificationItem: IGrain
//{
//    protected PolymorphicListGrain(IPersistentState<List<IPolymorphicListGrainItem<TNotificationItem>>> items, IPersistentState<SortedDictionary<DateTime, string>> deletedItemsState) : base(items, deletedItemsState)
//    {
//    }
//}

#endregion

// OPTIMIZE: Replace List with HashSet
//[GenerateSerializer]
public class KeyedListG<TKey, TItem> : Grain
    , IKeyedListG<TKey, TItem>
    //, IAsyncCreating<TNotificationItem> 
    //, ICreatingAsyncDictionary<string, TNotificationItem>
    , IDeleteTrackingListGrain<TItem>
{

    #region Dependencies

    public IClusterClient ClusterClient => ServiceProvider.GetRequiredService<IClusterClient>();

    #endregion

    #region State (persisted)

    [Id(0)]
    protected IPersistentState<List<TItem>> ItemsState { get; }

    [Id(1)]
    protected IPersistentState<SortedDictionary<DateTime, TItem>>? DeletedItemsState { get; private set; }

    #endregion

    #region Configuration

    public bool TrackDeletedItems
    {
        get => DeletedItemsState != null;
        set
        {
            if (value)
            {
                if (DeletedItemsState == null) throw new InvalidOperationException();
            }
            else
            {
                DeletedItemsState = null;
            }
        }
    }

    #endregion

    #region Lifecycle

    // TODO: Replace List<TNotificationItem> with Dictionary<string, TNotificationItem>
    public ListG(/* [PersistentState("Matchmakers", MetaverseStoreNames.Metaverse)] */ IPersistentState<List<TItem>> items, IPersistentState<SortedDictionary<DateTime, TItem>> deletedItemsState)
    {
        ItemsState = items;
        DeletedItemsState = deletedItemsState;
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken = default)
    {
        ItemsState.State ??= new();
        return base.OnActivateAsync(cancellationToken);
    }

    #endregion

    #region Observers

    private readonly ObserverManager<IChangeSetObserverO<TItem>> observers;

    public Task SubscribeObserver(IChangeSetObserverO<TItem> observer)
    {
        observers.Subscribe(observer, observer);
        return Task.CompletedTask;
    }
    public Task UnsubscribeObserver(IChangeSetObserverO<TItem> observer)
    {
        observers.Unsubscribe(observer);
        return Task.CompletedTask;
    }
    public Task NotifyObservers(ChangeSet<TItem> message)
    {
        observers.Notify(s => s.OnNextAsync(message));
        return Task.CompletedTask;
    }

    #endregion

    #region Key

    // NOTE: Keys are needed only when TrackDeletedItems == true

    // for Dictionary
    //public virtual string GetKey(TNotificationItem value)
    //{
    //    if (value is IKeyed<string> k)
    //    {
    //        return k.Key;
    //    }
    //    else
    //    {
    //        throw new NotSupportedException($"Since {nameof(TNotificationItem)} does not implement IKeyed<string> or PolymorphicGrainListItem<>, GetKey must be overridden with a valid implementation.");
    //    }
    //}

    #endregion

    #region Deleted Items: Pruning

    [Id(2)]
    public TimeSpan DeletedItemExpiry { get; set; } = TimeSpan.FromDays(90);

    public async Task PruneDeletedItems()
    {
        KeyValuePair<DateTime, TItem> first;
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

    #region Instantiation

    protected virtual void OnInstantiated(TItem value) { }

    protected virtual TItem Instantiate(Type type)
    {
        if (ActivatorUtilitiesEx.TryCreateInstance(ServiceProvider, out TItem instance, type)) { return instance; }
        return (TItem)ActivatorUtilities.CreateInstance(ServiceProvider, type, Guid.NewGuid().ToString(), type);
    }

    protected virtual TItem InstantiateUnique(Type type)
    {
        TItem result;

        do
        {
            result = Instantiate(type);
        } while (ItemsState.State.Where(m => EqualityComparer<TItem>.Default.Equals(m, result)).Any() || (DeletedItemsState != null && DeletedItemsState.State.Where(deletedKey => EqualityComparer<TItem>.Default.Equals(deletedKey.Value, result)).Any()));

        return result;
    }
    //protected virtual TNotificationItem InstantiateUnique_MOVE_to_Dictionary(Type type)
    //{
    //    TNotificationItem result;
    //    string key;

    //    do
    //    {
    //        result = Instantiate(type);
    //        key = GetKey(result);
    //    } while (ItemsState.State.Where(m => GetKey(m) == key).Any() || (DeletedItemsState != null && DeletedItemsState.State.Where(deletedKey => deletedKey.Value == key).Any()));

    //    return result;
    //}

    #endregion

    #region Modify the list

    #region Create

    public async virtual Task<TItem> Create(Type type, Action<TItem>? init = null)
    {
        var item = InstantiateUnique(type);

        OnInstantiated(item);
        init?.Invoke(item);

        await Add(item);

        return item;
    }

    #endregion

    public Task Insert(int index, TItem item)
    {
        ItemsState.State.Insert(index, item);
        return ItemsState.WriteStateAsync();
    }

    #region Add

    public async Task Add(TItem item)
    {
        TItem saveValue = item;
        //if(item is IAddressable a) { 
        //    saveValue = a.AsReference<TNotificationItem>(); 
        //}

        ItemsState.State.Add(saveValue);
        await ItemsState.WriteStateAsync();

        //var publishTask = PublishCollectionChanged(new NotifyCollectionChangedEventArgs<TNotificationItem>(System.Collections.Specialized.NotifyCollectionChangedAction.Add, item));
        var publishTask = PublishCollectionChanged(new ChangeSet<TItem>(new Change<TItem>[] { new Change<TItem>(ListChangeReason.Add, item) }));

        if (AwaitPublishingNotificationEvents)
        {
            await publishTask;
        }
    }

    #endregion

    #region Remove

    public async Task<bool> Remove(TItem id)
    {
        var existing = ItemsState.State.Where(m => EqualityComparer<TItem>.Default.Equals(m, id)).FirstOrDefault();
        if (existing != null)
        {
            // TOTRANSACTION

            Task? deleteTask = null;

            //var key = GetKey(existing);

            if (DeletedItemsState != null) // TrackDeletedItems
            {
                DeletedItemsState.State.Add(DateTime.UtcNow, existing);
                deleteTask = DeletedItemsState.WriteStateAsync();
            }

            ItemsState.State.Remove(existing);
            await ItemsState.WriteStateAsync();
            await PublishCollectionChanged(new ChangeSet<TItem>(new List<Change<TItem>> { new Change<TItem>(ListChangeReason.Remove, existing) }));
            //PublishCollectionChanged(new NotifyCollectionChangedEventArgs<TNotificationItem>(System.Collections.Specialized.NotifyCollectionChangedAction.Remove, existing));

            if (deleteTask != null) { await deleteTask; }
            return true;
        }
        else
        {
            return false;
        }
    }

    public Task RemoveAt(int index)
    {
        ItemsState.State.RemoveAt(index);
        return ItemsState.WriteStateAsync();
    }

    public Task Clear()
    {
        ItemsState.State.Clear();
        return ItemsState.WriteStateAsync();
    }

    #endregion

    #endregion

    public static bool DefaultAwaitPublishingNotificationEvents { get; set; } = false;
    public bool AwaitPublishingNotificationEvents { get; set; } = DefaultAwaitPublishingNotificationEvents;

    #region Event: CollectionChanged

    private IAsyncStream<ChangeSet<TItem>> CollectionChangedStream => this.GetStreamProvider("ChangeNotifications").GetStream<ChangeSet<TItem>>(
               this.GetPrimaryKeyString(), "CollectionChanged");

    public Task<int> Count => throw new NotImplementedException();

    public Task<bool> IsReadOnly => throw new NotImplementedException();

    //public Task PublishCollectionChanged(NotifyCollectionChangedEventArgs<TNotificationItem> args)
    public async Task PublishCollectionChanged(ChangeSet<TItem> args)
    {
        //var s = ClusterClient.GetStreamProvider("ChangeNotifications").GetStream<NotifyCollectionChangedEventArgs<TNotificationItem>>(Guid.Parse(Source.GetPrimaryKeyString()), "CollectionChanged");

        //IStreamProvider streamProvider = GetStreamProvider("ChangeNotifications");

        //var deviceStream = streamProvider.GetStream<CollectionChangeEventArgs>(
        //       Guid.Parse(this.GetPrimaryKeyString()), "CollectionChanged");
        await NotifyObservers(args);
        await CollectionChangedStream.OnNextAsync(args);
    }

    #endregion

    // REVIEW - shouldn't be necessary, but might help if things get out of sync.
    // UNTESTED - Not sure it works as intended
    public Task Reset()
        => PublishCollectionChanged(new ChangeSet<TItem>(ItemsState.State.Select(s => new Change<TItem>(ListChangeReason.Refresh, s))));

    public Task<IEnumerable<TItem>> Items() => Task.FromResult<IEnumerable<TItem>>(ItemsState.State);

    public Task<IEnumerable<KeyValuePair<DateTime, TItem>>> DeletedKeys() => Task.FromResult(
        (DeletedItemsState?.State as IEnumerable<KeyValuePair<DateTime, TItem>> ?? Enumerable.Empty<KeyValuePair<DateTime, TItem>>())
        );

    public Task<IEnumerable<TItem>> GetEnumerableAsync() => Task.FromResult<IEnumerable<TItem>>(ItemsState.State);

    public virtual Task<IEnumerable<Type>> SupportedTypes() => Task.FromResult(Enumerable.Empty<Type>());

    public Task<TItem> ElementAt(int index) => Task.FromResult(ItemsState.State[index]);

    public Task ElementAt(int index, TItem value)
    {
        ItemsState.State[index] = value;
        return ItemsState.WriteStateAsync();
    }

    public Task<int> IndexOf(TItem item) => Task.FromResult(ItemsState.State.IndexOf(item));



    public Task<bool> Contains(TItem item) => Task.FromResult(ItemsState.State.Contains(item));

    // NOTE: Not going to work for Orleans
    public Task CopyTo(TItem[] array, int arrayIndex)
    {
        ItemsState.State.CopyTo(array, arrayIndex);
        return Task.CompletedTask;
    }

    public Task<int> GetCount() => Task.FromResult(ItemsState.State.Count);

    public Task<bool> GetIsReadOnly()
    {
        throw new NotImplementedException();
    }

    public Task<IResolveResult<IEnumerable<TItem>>> Resolve()
        => Task.FromResult<IResolveResult<IEnumerable<TItem>>>( new ResolveResultSuccess<IEnumerable<TItem>>(ItemsState.State));
}

//public static class IListGrainExtensionsData<TNotificationItem>
//{
//    static ConcurrentWeakDictionaryCache<IListG<TNotificationItem>, ListGrainCollectionChangedSubject<TNotificationItem>> ListGrainCollectionChangedSubjects { get; } = new(listGrain => new ListGrainCollectionChangedSubject<TNotificationItem>(listGrain));
//}

//public static class IListGrainExtensions
//{
//    public static IDisposable Subscribe(this IListG observer)
//    {
//        IListGrainExtensionsData<TNotificationItem>.ListGrainCollectionChangedSubjects[observer].Subscribe(observer);

//        //IObserver<ListGrainCollectionChangedEvent<TNotificationItem>>

//    }
//    //Subject<> CollectionChangedSubject { get; } = new();
//    //void X()
//    //{
//    //    CollectionChangedSubject.
//    //}

//}


