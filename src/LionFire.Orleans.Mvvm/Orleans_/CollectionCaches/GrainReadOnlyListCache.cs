using DynamicData;
using LionFire.Data.Collections;
using LionFire.Orleans_.Streams;
using LionFire.Data.Gets;
using Orleans.Runtime;
using Orleans.Streams;
using System.Collections;
using System.Reactive.Subjects;

namespace LionFire.Orleans_.Mvvm;

public static class GrainListCacheCommon<TItemGrain/*, TCollection*/>
    //where TCollection : ReactiveObject, IAsyncDictionaryCache<GrainId, TItemGrain>
{

    public static async ITask<IGetResult<IEnumerable<TItemGrain>>> ResolvesRetrieveFunc(IGrain collectionGrain)
    {
        if (collectionGrain is IStatelessGetsG<IEnumerable<TItemGrain>> resolves)
        {
            return await resolves.Get().ConfigureAwait(false);
        }
        else throw new NotSupportedException($"{nameof(collectionGrain)} does not implement {nameof(IStatelessGetsG<IEnumerable<TItemGrain>>)}");
    }
    //. Either implement it or provide either {nameof(ResolveFunc)} or {nameof(RetrieveFunc)}

    //    public static Func<IGrain, ITask<IGetResult<IEnumerable<TItemGrain>>>> RetrieveToResolveFunc(Func<IGrain, ITask<IEnumerable<TItemGrain>>> retrieveFunc)
    //        => async grain =>
    //        {
    //            var values = await value(grain).ConfigureAwait(false);
    //            return new ResolveResultSuccess<IEnumerable<TItemGrain>>(values);
    //        };
}


// TCollection:
//  - AsyncReadOnlyDictionaryCache
//  - AsyncDictionaryCache
// Inheritors:
//  - GrainListCache
//  - GrainReadOnlyListCache
//public class GrainListCacheBase<TItemGrain>

#if GrainReadOnlyListCache
/// <summary>
/// Lightweight (read-only) alternative to GrainListCache
/// </summary>
/// <typeparam name="TItemGrain"></typeparam>
public class GrainReadOnlyListCache<TItemGrain>
     : AsyncReadOnlyDictionaryCache<GrainId, TItemGrain>
    , ICollectionGrainObserver
    //, IConnectableCache<TItemGrain, GrainId> // TODO: How to subscribe? AsyncConnectable?
    //: AsyncObservableDictionaryCache<string, TItemGrain>
    //: System.IAsyncObservable<ChangeSet<GrainId>>
    //, LionFire.Reactive.IAsyncObservableForSyncObservers<ChangeSet<GrainId>>
    //, class,  IObservableCollection<KeyValuePair<TItem, TValue>>, new()
    where TItemGrain : class, IGrainWithStringKey
    //where TCollectionGrain : IGrain, IGrainWithStringKey
    // Note: Base class is a list.  Would be cleaner if it was AsyncEnumerableListCache, but probably unneeded
{
    #region Dependencies

    public IClusterClient ClusterClient { get; }

    #endregion

    #region Relationships

    public IGrain CollectionGrain { get; }

    #endregion

    #region Parameters

    public AsyncObservableCollectionOptions Options { get => options ?? DefaultOptions; set => options = value; }
    private AsyncObservableCollectionOptions? options;
    public static AsyncObservableCollectionOptions DefaultOptions = new();

    #endregion

    #region Lifecycle

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grainKey"></param>
    /// <param name="clusterClient"></param>
    /// <param name="existingItems">Pre-populates cache with these items</param>
    /// <param name="options"></param>
    public GrainReadOnlyListCache(IGrain collectionGrain, IClusterClient clusterClient, IEnumerable<TItemGrain>? existingItems = default
        , AsyncObservableCollectionOptions? options = null)
        : base(new(g => g.GetGrainId()))
    {
        ClusterClient = clusterClient;
        CollectionGrain = collectionGrain;
        Options = options!; // might be null
        RetrievedToCacheSubscription = SourceCache.PopulateFrom(Retrieved);

        if (existingItems != null)
        {
            Cache.AddOrUpdate(existingItems);
        }
    }

    public void Dispose()
    {
        RetrievedToCacheSubscription?.Dispose();
        RetrievedToCacheSubscription = null;
    }

    #endregion

    #region State

    public SourceCache<TItemGrain, GrainId> Cache => state.SourceCache;

    #region IConnectableCache

    //public IObservable<int> CountChanged => Cache.CountChanged;

    //public IObservable<IChangeSet<TItemGrain, GrainId>> Connect(Func<TItemGrain, bool>? predicate = null, bool suppressEmptyChangeSets = true)
    //{
    //    return Cache.Connect(predicate, suppressEmptyChangeSets);
    //}

    //public IObservable<IChangeSet<TItemGrain, GrainId>> Preview(Func<TItemGrain, bool>? predicate = null)
    //{
    //    return Cache.Preview(predicate);
    //}

    //public IObservable<Change<TItemGrain, GrainId>> Watch(GrainId key)
    //{
    //    return Cache.Watch(key);
    //}

    #endregion

    #region Convenience

    public IEnumerable<GrainId> Keys => Cache.Keys;
    public IEnumerable<TItemGrain> Values => Cache.Items;

    #endregion

    #endregion

    #region Collection

    public virtual bool IsReadOnly => true;

    #endregion

    #region Notifications

    #region ...from Observer

    public async Task SubscribeAsObserver()
    {
        if (CollectionGrain is not System.IAsyncObservable<ChangeSet<GrainId>> asyncObservable)
        {
            throw new NotSupportedException($"{nameof(CollectionGrain)} of TNewItem {CollectionGrain.GetType().FullName} does not implement {typeof(System.IAsyncObserver<ChangeSet<GrainId>>).FullName}");
        }
        else
        {
            var objectReference = ClusterClient.CreateObjectReference<ICollectionGrainObserver>(this);

            var asyncDisposable = await asyncObservable.SubscribeAsync(this).ConfigureAwait(false);

            if (asyncDisposable is IDependsOn<IGrainFactory> d)
            {
                d.Dependency = ClusterClient;
            }
        }
    }

    #endregion

    #region ...from Stream

    public IAsyncStream<ChangeSet<GrainId>> CollectionChangedStream
    {
        get
        {
            var s = ClusterClient.GetStreamProvider("ChangeNotifications")
                .GetStream<ChangeSet<GrainId>>("CollectionChanged", Guid.Parse(CollectionGrain.GetPrimaryKeyString()));
            return s;
        }
    }

    #region LionFire AsyncObserver

    //OrleansStreamObserverToAsyncObserver<NotifyCollectionChangedEventArgs<string>>? subscriptionAdapter;

    //public ValueTask<IAsyncDisposable> SubscribeAsync(System.IAsyncObserver<NotifyCollectionChangedEventArgs<string>> observer)
    //{
    //    //if (subscriptionAdapter == null)
    //    //{
    //    //    subscriptionAdapter = new OrleansStreamObserverToAsyncObserver<NotifyCollectionChangedEventArgs<string>>(this);
    //    //}
    //    //return await subscriptionAdapter.SubscribeAsync(observer);

    //    return CollectionChangedStream.SubscribeAsync(observer);
    //}

    //public Task<IAsyncDisposable> SubscribeAsync(IObserver<NotifyCollectionChangedEventArgs<string>> observer)
    //{
    //    //if (subscriptionAdapter == null)
    //    //{
    //    //    subscriptionAdapter = new OrleansStreamObserverToAsyncObserver<NotifyCollectionChangedEventArgs<string>>(this);
    //    //}
    //    //return await subscriptionAdapter.SubscribeAsync(observer);

    //    return CollectionChangedStream.SubscribeAsync(observer);
    //}

    #endregion

    #endregion

    #region ...from Broadcast

    #endregion

    #endregion

    //public virtual string GetKey(TValue item)
    //{
    //    if (item is IKeyed<string> k)
    //    {
    //        return k.Key;
    //    }
    //    else
    //    {
    //        throw new NotSupportedException($"Since {nameof(TValue)} does not implement IKeyed<string>, GetKey must be overridden with a valid implementation.");
    //    }
    //}

    #region Resolve

    Subject<IEnumerable<TItemGrain>> Retrieved = new();
    public IDisposable RetrievedToCacheSubscription { get; private set; }

    public async ITask<IGetResult<IEnumerable<TItemGrain>>> Resolve()
    {

    }

    //public virtual bool CanResolve => true;

    //public Task<IEnumerable<KeyValuePair<string, TItemGrain>>> Retrieve(CancellationToken cancellationToken = default)
    //{
    //    return RetrieveImpl(cancellationToken);
    //}
    //protected async Task<IEnumerable<KeyValuePair<string, TItemGrain>>> RetrieveImpl(CancellationToken cancellationToken = default)
    //    => throw new NotImplementedException()// (await Cache.GetEnumerableAsync())
    //                                          //.Select(i => (TValue)GrainFactory.GetGrain(i.Type, i.Id))
    //    ;

    #endregion

    //protected Task<bool> Subscribe()
    //{
    //    throw new NotImplementedException();
    //    //var streamProvider = ClusterClient.GetStreamProvider("ChangeNotifications"); // HARDCODE TODO
    //    //var stream = streamProvider.GetStream<string>(Source.GetPrimaryKeyString(), "DictionaryCache"); // HARDCODE TODO
    //    //var subscriptionHandle = await stream.SubscribeAsync(async (message, token) => Console.WriteLine(message));

    //    //return true; // TODO REVIEW
    //}

    #region IAsyncObserver

    public ValueTask OnNextAsync(ChangeSet<GrainId> value)
    {
        throw new NotImplementedException();
    }

    public ValueTask OnErrorAsync(Exception error)
    {
        throw new NotImplementedException();
    }

    public ValueTask OnCompletedAsync()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region IEnumerable

    public IEnumerator<KeyValuePair<GrainId, TItemGrain>> GetEnumerator()
        => this.Cache.KeyValues.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    //IEnumerator<GrainId> IEnumerable<GrainId>.GetEnumerator() => this.Cache.Keys.GetEnumerator();
    IEnumerator<TItemGrain> IEnumerable<TItemGrain>.GetEnumerator()
        => this.Cache.Items.GetEnumerator();

    #endregion


#if TODO
    /// <summary>
    /// Can call Subscribe() and Unsubscribe().  Do not call these directly; set Sync instead.
    /// </summary>
    public override bool CanSubscribe => true;

    /// <summary>
    /// 
    /// </summary>
    /// <returns>true if actually subscribed to something during this invocation, false if already subscribed (throw on fail)</returns>
    /// <exception cref="NotSupportedException"></exception>
    protected override Task<bool> Subscribe() => throw new NotSupportedException();
    protected override Task Unsubscribe() => throw new NotSupportedException();
#endif

}
#endif
