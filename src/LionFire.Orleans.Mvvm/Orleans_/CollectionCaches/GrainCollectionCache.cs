//#if UNUSED // Maybe useful someday for non-polymorphic lists
using LionFire.Orleans_.Collections;
//using ObservableCollections;
using Orleans;
using static LionFire.Reflection.GetMethodEx;
using System.Collections.ObjectModel;
using LionFire.Structures;
using System.ComponentModel;
using Orleans.Streams;
using System.Collections.Specialized;
using LionFire.Collections;
using LionFire.Reflection;
using System.Reflection.Metadata.Ecma335;
using LionFire.Orleans_.Mvvm;
using LionFire.Reactive;
using System.Runtime.CompilerServices;
using LionFire.Orleans_.Streams;
using Newtonsoft.Json.Linq;
using LionFire.Data.Collections;
using DynamicData;
using DynamicData.Cache;
using System;
using Orleans.Runtime;
using LionFire.ExtensionMethods.Collections;
using Orleans.Metadata;
using LionFire.Mvvm;
using LionFire.Data.Async.Gets;
using MorseCode.ITask;
using LionFire.Data;
using System.Reactive.Subjects;
using ReactiveUI;
using System.Reactive;
using System.Linq;
using LionFire.Orleans_.Reactive_;
using System.Reactive.Disposables;
using System.Diagnostics.CodeAnalysis;
using LionFire.Threading;
using Microsoft.Extensions.Options;
using LionFire.ExtensionMethods.Poco.Getters;
using LionFire.Ontology;
using LionFire.Data.Async.Sets;
using System.Diagnostics;
using LionFire.ExtensionMethods;
using LionFire.Orleans_.ObserverGrains;

namespace LionFire.Orleans_.Mvvm;

//public class NotifyingGrainListCache<TInfo, TCollection> : GrainListCache<TInfo, TCollection>
//    , System.IAsyncObservable<NotifyCollectionChangedEventArgs<ChangeSet<GrainId>>>
//    , LionFire.Reactive.IAsyncObservableForSyncObservers<ChangeSet<GrainId>>
//    where TCollection : IAsyncList<TInfo>, IAsyncCreates<TInfo>, IGrain, IEnumerableAsync<KeyValuePair<string, TInfo>>
//    where TInfo : class, IGrain
//{
//    public IClusterClient ClusterClient { get; }

//    #region Lifecycle

//    public NotifyingGrainListCache(TCollection? source = default, IClusterClient clusterClient) : base(source)
//    {
//        ClusterClient = clusterClient;
//    }
//    #endregion

//}

[Flags]
public enum SubscriptionMethods
{
    None = 0,
    GrainObserver = 1 << 0,
    Stream = 1 << 1,
}

public class SubscriptionOptions<T>
{
    public SubscriptionMethods AutoSubscribe { get; set; } = SubscriptionMethods.GrainObserver;
}

/// <summary>
/// Write-through Async ObservableCache for an IKeyedListG
/// Underlying collection is a list of GrainIds.  This Mvvm collection is a dictionary with GrainId keys and TItemGrain grain values.
/// TODO: change this class to ListGrainCache?  Is there a benefit to knowing TNotificationItem is a grain?
/// </summary>
/// <typeparam name="T"></typeparam>
public class GrainCollectionCache<TValue> // RENAME: GrainCollectionCache
    : AsyncKeyedCollection<string, TValue>
    , IObservableCreatesAsync<string, TValue>
    //, IObservableCreatesAsync<TInfo>
    , IDisposable
    , IAsyncDisposable
    , ISubscribesAsync
    , IHas<IGrainObservableAsyncObservableG<ChangeSet<TValue, string>>>
    , ICreatesAsync<string, TValue>
    , ICreatesAsync<TValue>
    where TValue : class, IGrainWithStringKey
{
    #region Dependencies

    public IClusterClient ClusterClient { get; }

    #endregion

    #region Relationships

    public IKeyedCollectionG<string, TValue> CollectionGrain { get; set; }
    IGrainObservableAsyncObservableG<ChangeSet<TValue, string>>  IHas<IGrainObservableAsyncObservableG<ChangeSet<TValue, string>>>.Object => CollectionGrain;

    #endregion

    #region Parameters

    public AsyncObservableCollectionOptions Options { get => options ?? DefaultOptions; set => options = value; }
    private AsyncObservableCollectionOptions? options;
    public static AsyncObservableCollectionOptions DefaultOptions = new();

    #endregion

    #region Lifecycle

    #region Static

    static GrainCollectionCache()
    {
        InitGetOrCreateForKeyImpl();
    }

    #endregion

    public GrainCollectionCache(/* Parameter */ IKeyedCollectionG<string, TValue> collectionGrain,
        IClusterClient clusterClient
        //, IOptionsMonitor<SubscriptionOptions<GrainListCache<TItemGrain>>> optionsMonitor // TEMP Commented
        )
    //: base(new(g => g.GetGrainId()))
    {
        ClusterClient = clusterClient;
        CollectionGrain = collectionGrain;

        //TryAutoSubscribe(optionsMonitor.CurrentValue.AutoSubscribe); // TEMP Commented
        TryAutoSubscribe(SubscriptionMethods.GrainObserver);
        //GetOptions = options!; // might be null // TEMP Commented

        // CreateForKeyCommand = ReactiveCommand.Create<(string key, Type grainType), TItemGrain>(async ((string key, Type grainType) x) =>
        // {
        //     return await GetOrCreateImpl(CollectionGrain, x).ConfigureAwait(false);
        // }
        ////, canExecute
        ////, scheduler
        //);

        GetOrCreateForKey = ReactiveCommand.Create<(string key, Type grainType), Task<TValue>>(((string, Type) parameters) => GetOrCreateImpl(CollectionGrain, parameters));
    }

    void TryAutoSubscribe(SubscriptionMethods subscriptionMethods)
    {
        if (subscriptionMethods.HasFlag(SubscriptionMethods.GrainObserver))
        {
            this.SubscribeViaGrainObserver(ClusterClient).FireAndForget(); // REVIEW:  save task somewhere?
        }
        //if (subscriptionMethods.HasFlag(SubscriptionMethods.Stream))
        //{
        //    this.SubscribeViaStream().FireAndForget(); // REVIEW:  save task somewhere?
        //}
    }


    public void Dispose()
    {
        throw new NotImplementedException();
    }
    public async ValueTask DisposeAsync()
    {
        await (Interlocked.Exchange(ref keyedListObserverO, null)?.DisposeAsync() ?? ValueTask.CompletedTask);
    }

    #endregion

    #region State

    #region Components

    KeyedListObserverO<string, TValue> keyedListObserverO;

    #endregion

    #endregion

    #region Get

    protected override ITask<IGetResult<IEnumerable<TValue>>> GetImpl(CancellationToken cancellationToken = default)
    {
        return GrainListCacheCommon<TValue>.ResolvesRetrieveFunc(CollectionGrain);
    }

    public override void OnNext(IGetResult<IEnumerable<TValue>> result)
    {
        Debug.WriteLine($"{this.GetType().ToHumanReadableName()} OnNext GetResult: {result}");
    }

    #endregion

    #region Create

    #region (static)

    static void InitGetOrCreateForKeyImpl()
    {

        if (typeof(TValue).IsAssignableTo(typeof(IObservableCreatesAsync<string, TValue>)))
        {
            GetOrCreateImpl = async (IGrain grain, (string key, Type grainType) parameters) =>
            {
                var creates = (IObservableCreatesAsync<string, TValue>)grain;

                var newItemGrain = await creates.CreateForKey(parameters.key, parameters.grainType).ConfigureAwait(false);
                return newItemGrain;
            };
        }
        else
        {
            GetOrCreateImpl = (_, _) => throw new NotImplementedException($"Either implement {typeof(IObservableCreatesAsync<string, TValue>).FullName} on {nameof(CollectionGrain)} or set {nameof(GetOrCreateImpl)}");
        }
    }

    public static Func<IGrain, (string, Type), Task<TValue>> GetOrCreateImpl;

    #endregion

    #region ICreatesAsync<TValue>

    public Task<TValue> Create(Type type, params object[]? constructorParameters)
        => CollectionGrain.Create(type, constructorParameters);

    #endregion

    public ReactiveCommand<(string key, Type grainType), Task<TValue>> GetOrCreateForKey { get; }

    //public Task<TItemGrain> GetOrCreateForKey(string key, Type type, params object[] constructorParameters)
    //{
    //    GrainType
    //    GrainId.Create(,)
    //    if (SourceCache.Keys.Contains(GrainId
    //    throw new NotImplementedException();
    //}

    //public virtual bool CanCreate => true;

    public IObservable<(string key, Type type, object[] parameters, Task result)> CreatesForKey => throw new NotImplementedException();

    //public async Task<KeyValuePair<string, TItemGrain>> Create<TNewItem>(params object[]? constructorParameters)
    //    where TNewItem : TItemGrain
    //{
    //    //if (!CanCreate) { throw new NotSupportedException($"{nameof(CanCreate)}"); }
    //    if (!typeof(TNewItem).IsAssignableTo(typeof(TItemGrain))) { throw new ArgumentException($"TNewItem must be assignable to {typeof(TItemGrain).FullName}"); }
    //    if (constructorParameters != null && constructorParameters.Length != 0) { throw new NotImplementedException($"{nameof(constructorParameters)} not implemented"); }
    //    var result = await CreateImpl<TNewItem>();
    //    return new KeyValuePair<string, TItemGrain>(((IGrainWithStringKey)result).GetPrimaryKeyString(), result);
    //}

    #region ICreatesAsync<TKey, TValue>

    public Task<TValue> CreateForKey(string key, Type type, params object[] constructorParameters)
    {
        throw new NotImplementedException();
    }

    Task<TValue> ICreatesAsync<string, TValue>.GetOrCreateForKey(string key, Type type, params object[] constructorParameters)
    {
        throw new NotImplementedException();
    }

    #endregion

    #endregion

    #region Add

    //public override Task Add(KeyValuePair<string, TItemGrain> item)
    //{
    //    return base.Add(item);
    //}

    #endregion

    // ENH: Tracking operations in progress
    //public SourceList<ChangeSet<KeyValuePair<GrainId, TItemGrain?>>> OperationsInProgress;

    #region Remove

    public override async Task<bool> Remove(string key)
    {
        if (CollectionGrain == null) { throw new InvalidOperationException($"Cannot invoke while {nameof(CollectionGrain)} is null"); }

        //if (GetOptions.TrackOperationsInProgress) {
        //    var changeSet
        //    (OperationsInProgress ??= new()).Add(new KeyValuePair<>()));
        //}

        bool result = await CollectionGrain.Remove(key).ConfigureAwait(false);

        //if (CollectionGrain is IAsyncList<TItemGrain> dict)
        //{
        //    result = await dict.Remove(item).ConfigureAwait(false);
        //}
        //else throw new NotSupportedException();

        //if (GetOptions.TrackOperationsInProgress) { OperationsInProgress.Remove(item); if (RemovalsInProgrss.Count == 0) RemovalsInProgrss = null; }

        return result;
    }

    public override Task<bool> Remove(TValue item) => Remove(item.GetPrimaryKeyString());

    //public Task<bool> Remove(KeyValuePair<string, TItemGrain> item)
    //    => Remove(item.Key); // Alternative: ask the collection to remove the KeyValuePair

    #endregion

    #region Observing Changes

    #region ISubscribes

    public IEnumerable<IAsyncDisposable> Subscriptions => subscriptions.Value;

    public IObservable<(Type, object[]?, Task<TValue>)> Creates => throw new NotImplementedException();

    Lazy<List<IAsyncDisposable>> subscriptions = new();

    public void OnSubscribing(IAsyncDisposable asyncDisposable) => subscriptions.Value.Add(asyncDisposable);
    public ValueTask Unsubscribe() 
        => ValueTaskEx.WhenAll(Interlocked.Exchange(ref subscriptions, new()).Value.Select(v => v.DisposeAsync()));

    #endregion

    ///// <summary>
    ///// Input: IAsyncObserver, Output: IObservable
    ///// </summary>
    //public IGrainObserverO<TItemGrain>? ChangeSetGrainObserver => changeSetGrainObserver;
    //private ChangeSetGrainObserver<TItemGrain>? changeSetGrainObserver;

    //IAsyncDisposable grainObserverSubscription;
    //AsyncGate grainObserverSubscriptionGate = new();
    //Task? subscribeTask = null;

    //public async Task<bool> SubscribeViaGrainObserverX()
    //{
    //    using (await grainObserverSubscriptionGate.LockAsync().ConfigureAwait(false))
    //    {
    //        if (grainObserverSubscription != null) return false;
    //        grainObserverSubscription = await KeyedListObserverOX.SubscribeViaGrainObserver(this).ConfigureAwait(false);
    //        return true;
    //    }
    //}

    //bool syncViaObserver;

    //public async Task UnsubscribeViaGrainObserver()
    //{
    //    await grainObserverSubscription.DisposeAsync().ConfigureAwait(false);
        
    //    var reSyncViaObserver = ReSyncViaObserver;
    //    ReSyncViaObserver = null;
    //    reSyncViaObserver?.Dispose();
    //}

    // TODO?
    //public bool SyncViaStream
    //{
    //    set
    //    {
    //        if (syncViaStream == value) return;

    //        syncViaStream = value;
    //        if (value)
    //        {
    //            // Create ChangeSetGrainStream
    //            //if (changeSetGrainStream == null)
    //            //{
    //            //    changeSetGrainStream = new ChangeSetGrainStream<TItemGrain>();
    //            //    changeSetGrainStream.Subscribe(this,)
    //            //}
    //            CollectionGrain.SubscribeStream(this);
    //        }
    //        else
    //        {
    //            ReSyncViaStream.Dispose();
    //            CollectionGrain.UnsubscribeStream(this);
    //        }
    //    }
    //}
    //bool syncViaStream;

    #endregion
   

    #region System.IAsyncObservable

    public ValueTask<IAsyncDisposable> SubscribeAsync(System.IAsyncObserver<ChangeSet<TValue, string>> observer)
    {
        throw new NotImplementedException();
    }

    #endregion

    //public override IEnumerator<TItemGrain> GetEnumerator()
    //{
    //    throw new NotImplementedException();
    //}

}

#if GrainDictionaryCache
public class GrainDictionaryCache<TItemGrain>
 : AsyncDictionaryCache<GrainId, TItemGrain>
{
    #region IAsyncObserver

    public ValueTask OnNextAsync(ChangeSet<TItemGrain> value)
    {
        SourceCache.Edit(u => u.Clone(value.Select(c => new Change<TItemGrain, GrainId>())));
        return ValueTask.CompletedTask;
    }

    #endregion
}
//public static class ChangeSetX
//{
//    public static ChangeSet<TItemGrain, GrainId> Clone<TItemGrain>(this ChangeSet<TItemGrain> changeSet)
//        where TItemGrain : IGrainWithGuidKey
//    {
//        //ListChangeReason l;
//        //ChangeReason c;
//        return new ChangeSet<TItemGrain, GrainId>(changeSet.Select(c => new Change<TItemGrain, GrainId>(c.Reason, c.Current, c.Previous)));
//    }
//}
#endif

//#endif