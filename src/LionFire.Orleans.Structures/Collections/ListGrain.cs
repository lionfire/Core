
using LionFire.Collections;
using LionFire.DependencyInjection;
using LionFire.Structures;
using LionFire.Threading;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Streams;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using static LionFire.Reflection.GetMethodEx;

namespace LionFire.Orleans_.Collections;

#region  FUTURE: Refactor the IPolymorphicListGrainItem<> wrapping in another class:

//public interface IPolymorphicListGrain<TItem>
//{
//}

//public abstract class PolymorphicListGrain<TItem> : ListGrain<IPolymorphicListGrainItem<TItem>>, IPolymorphicListGrain<TItem>
//    where TItem: IGrain
//{
//    protected PolymorphicListGrain(IPersistentState<List<IPolymorphicListGrainItem<TItem>>> items, IPersistentState<SortedDictionary<DateTime, string>> deletedItemsState) : base(items, deletedItemsState)
//    {
//    }
//}

#endregion

// OPTIMIZE: Replace List with HashSet
//[GenerateSerializer]
public class ListGrain<TItem> : Grain
    , IListGrain<TItem>
    //, IAsyncCreating<TItem> 
    //, ICreatingAsyncDictionary<string, TItem>
    , IDeleteTrackingListGrain<TItem>
{


    #region Dependencies
    
    public IClusterClient ClusterClient => ServiceProvider.GetRequiredService<IClusterClient>();

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

    // TODO: Replace List<TItem> with Dictionary<string, TItem>
    public ListGrain(/* [PersistentState("Matchmakers", MetaverseStoreNames.Metaverse)] */ IPersistentState<List<TItem>> items, IPersistentState<SortedDictionary<DateTime, TItem>> deletedItemsState)
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

    #region Key

    // NOTE: Keys are needed only when TrackDeletedItems == true

    // for Dictionary
    //public virtual string GetKey(TItem value)
    //{
    //    if (value is IKeyed<string> k)
    //    {
    //        return k.Key;
    //    }
    //    else
    //    {
    //        throw new NotSupportedException($"Since {nameof(TItem)} does not implement IKeyed<string> or PolymorphicGrainListItem<>, GetKey must be overridden with a valid implementation.");
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
    //protected virtual TItem InstantiateUnique_MOVE_to_Dictionary(Type type)
    //{
    //    TItem result;
    //    string key;

    //    do
    //    {
    //        result = Instantiate(type);
    //        key = GetKey(result);
    //    } while (ItemsState.State.Where(m => GetKey(m) == key).Any() || (DeletedItemsState != null && DeletedItemsState.State.Where(deletedKey => deletedKey.Value == key).Any()));

    //    return result;
    //}

    #endregion

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

    #region Add

    public async Task Add(TItem item)
    {
        ItemsState.State.Add(item);
        await ItemsState.WriteStateAsync();

        var publishTask = PublishCollectionChanged(new NotifyCollectionChangedEventArgs<TItem>(System.Collections.Specialized.NotifyCollectionChangedAction.Add, item));

        if (AwaitPublishingNotificationEvents)
        {
            await publishTask;
        }
    }

    #endregion

    public static bool DefaultAwaitPublishingNotificationEvents { get; set; } = false;
    public bool AwaitPublishingNotificationEvents { get; set; } = DefaultAwaitPublishingNotificationEvents;

    #region Event: CollectionChanged

    private IAsyncStream<NotifyCollectionChangedEventArgs<TItem>> CollectionChangedStream => this.GetStreamProvider("ChangeNotifications").GetStream<NotifyCollectionChangedEventArgs<TItem>>( 
               this.GetPrimaryKeyString(), "CollectionChanged");

    public Task PublishCollectionChanged(NotifyCollectionChangedEventArgs<TItem> args)
    {
        //var s = ClusterClient.GetStreamProvider("ChangeNotifications").GetStream<NotifyCollectionChangedEventArgs<TItem>>(Guid.Parse(Source.GetPrimaryKeyString()), "CollectionChanged");

        //IStreamProvider streamProvider = GetStreamProvider("ChangeNotifications");

        //var deviceStream = streamProvider.GetStream<CollectionChangeEventArgs>(
        //       Guid.Parse(this.GetPrimaryKeyString()), "CollectionChanged");
        return CollectionChangedStream.OnNextAsync(args);
    }

    #endregion

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
            PublishCollectionChanged(new NotifyCollectionChangedEventArgs<TItem>(System.Collections.Specialized.NotifyCollectionChangedAction.Remove, existing));

            if (deleteTask != null)
            {
                await deleteTask;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public Task<IEnumerable<TItem>> Items() => Task.FromResult<IEnumerable<TItem>>(ItemsState.State);

    public Task<IEnumerable<KeyValuePair<DateTime, TItem>>> DeletedKeys() => Task.FromResult(
        (DeletedItemsState?.State as IEnumerable<KeyValuePair<DateTime, TItem>> ?? Enumerable.Empty<KeyValuePair<DateTime, TItem>>())
        );

    public Task<IEnumerable<TItem>> GetEnumerableAsync() => Task.FromResult<IEnumerable<TItem>>(ItemsState.State);

    public virtual Task<IEnumerable<Type>> CreateableTypes() => Task.FromResult(Enumerable.Empty<Type>());

}

public class ListGrainCollectionChangedEvent<TItem>
{

    public CollectionChangeEventArgs Args { get; set; }

}

//public static class IListGrainExtensionsData<TItem>
//{
//    static ConcurrentWeakDictionaryCache<IListGrain<TItem>, ListGrainCollectionChangedSubject<TItem>> ListGrainCollectionChangedSubjects { get; } = new(listGrain => new ListGrainCollectionChangedSubject<TItem>(listGrain));
//}

//public static class IListGrainExtensions
//{
//    public static IDisposable Subscribe(this IListGrain observer)
//    {
//        IListGrainExtensionsData<TItem>.ListGrainCollectionChangedSubjects[observer].Subscribe(observer);

//        //IObserver<ListGrainCollectionChangedEvent<TItem>>

//    }
//    //Subject<> CollectionChangedSubject { get; } = new();
//    //void X()
//    //{
//    //    CollectionChangedSubject.
//    //}

//}


