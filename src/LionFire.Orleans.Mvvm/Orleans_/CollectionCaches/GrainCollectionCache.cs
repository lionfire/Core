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
using LionFire.Collections.Async;
using DynamicData;
using DynamicData.Cache;
using System;
using Orleans.Runtime;
using LionFire.ExtensionMethods.Collections;
using Orleans.Metadata;
using LionFire.Mvvm;
using LionFire.Resolves;
using MorseCode.ITask;
using System.Reactive.Subjects;
using LionFire.Resolvers;
using ReactiveUI;
using System.Reactive;
using System.Linq;
using LionFire.Orleans_.Reactive_;

namespace LionFire.Orleans_.Mvvm;

//public class NotifyingGrainListCache<TValue, TCollection> : GrainListCache<TValue, TCollection>
//    , System.IAsyncObservable<NotifyCollectionChangedEventArgs<ChangeSet<GrainId>>>
//    , LionFire.Reactive.IAsyncObservableForSyncObservers<ChangeSet<GrainId>>
//    where TCollection : IAsyncList<TValue>, IAsyncCreates<TValue>, IGrain, IEnumerableAsync<KeyValuePair<string, TValue>>
//    where TValue : class, IGrain
//{
//    public IClusterClient ClusterClient { get; }

//    #region Lifecycle

//    public NotifyingGrainListCache(TCollection? source = default, IClusterClient clusterClient) : base(source)
//    {
//        ClusterClient = clusterClient;
//    }
//    #endregion

//}

/// <summary>
/// Write-through Async Cache for an IKeyedListG
/// Underlying collection is a list of GrainIds.  This Mvvm collection is a dictionary with GrainId keys and TItemGrain grain values.
/// TODO: change this class to ListGrainCache?  Is there a benefit to knowing TNotificationItem is a grain?
/// </summary>
/// <typeparam name="T"></typeparam>
public class GrainListCache<TItemGrain> // RENAME: GrainCollectionCache
    : AsyncDictionaryCache<GrainId, TItemGrain>
    , IObservableCreatesAsync<GrainId, TItemGrain>
    , IDisposable
    , IAsyncDisposable
    where TItemGrain : class, IGrainWithStringKey
    //where TCollectionGrain : IGrain, IGrainWithStringKey
{
    #region Dependencies

    public IClusterClient ClusterClient { get; }

    #endregion

    #region Relationships

    public IKeyedListG<GrainId, TItemGrain> CollectionGrain { get; }

    #endregion

    #region Parameters

    public AsyncObservableCollectionOptions Options { get => options ?? DefaultOptions; set => options = value; }
    private AsyncObservableCollectionOptions? options;
    public static AsyncObservableCollectionOptions DefaultOptions = new();

    #endregion

    #region Lifecycle

    #region Static

    static GrainListCache()
    {
        InitGetOrCreateForKeyImpl();
    }

    #endregion

    public GrainListCache(IKeyedListG<GrainId, TItemGrain> collectionGrain, IClusterClient clusterClient)
    //: base(new(g => g.GetGrainId()))
    {
        ClusterClient = clusterClient;
        CollectionGrain = collectionGrain;
        SubscribeViaGrainObserver().FireAndForget(); // REVIEW
        Options = options!; // might be null

        // CreateForKeyCommand = ReactiveCommand.Create<(string key, Type grainType), TItemGrain>(async ((string key, Type grainType) x) =>
        // {
        //     return await GetOrCreateImpl(CollectionGrain, x).ConfigureAwait(false);
        // }
        ////, canExecute
        ////, scheduler
        //);

        GetOrCreateForKey = ReactiveCommand.Create<(string key, Type grainType), Task<TItemGrain>>(((string, Type) parameters) => GetOrCreateImpl(CollectionGrain, parameters));

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

    KeyedListObserverO<GrainId, TItemGrain> keyedListObserverO;

    #endregion

    #endregion

    #region Collection

    public override int Count => Value?.Count() ?? 0;

    #endregion

    #region Resolve

    #region Resolve

    private Func<IGrain, ITask<IResolveResult<IEnumerable<TItemGrain>>>> ResolveFunc { get; } = GrainListCacheCommon<TItemGrain>.ResolvesRetrieveFunc;

    public override ITask<IResolveResult<IEnumerable<TItemGrain>>> Resolve()
    {
        lock (currentResolving)
        {
            var existing = currentResolving.Value;
            if (existing != null && !existing.AsTask().IsCompleted) { return existing; }

            var task = Task.Run<IResolveResult<IEnumerable<TItemGrain>>>(async () =>
            {
                var result = await ResolveFunc(CollectionGrain).ConfigureAwait(false);
                if (result.IsSuccess != false)
                {
                    SourceList.EditDiff(result.Value ?? Enumerable.Empty<TItemGrain>(),
                        EqualityComparer<TItemGrain>.Default
                    //    (left, right) => 
                    ////left.GetGrainId() == right.GetGrainId()
                    );
                    //return new ResolveResultSuccess<IEnumerable<TItemGrain>>(result.Value);
                }
                return result;

            }).AsITask();

            currentResolving.OnNext(task);
            return task;
        }
    }

    #endregion

    #region Resolving

    public IObservable<ITask<IResolveResult<IEnumerable<TItemGrain>>>> Resolving => currentResolving;
    private BehaviorSubject<ITask<IResolveResult<IEnumerable<TItemGrain>>>> currentResolving = new(Task.FromResult<IResolveResult<IEnumerable<TItemGrain>>>(ResolveResultNotResolvedNoop<IEnumerable<TItemGrain>>.Instance).AsITask());
    private object currentResolvingLock = new();

    #endregion


    #endregion

    #region Create

    #region (static)

    static void InitGetOrCreateForKeyImpl()
    {

        if (typeof(TItemGrain).IsAssignableTo(typeof(IObservableCreatesAsync<string, TItemGrain>)))
        {
            GetOrCreateImpl = async (IGrain grain, (string key, Type grainType) parameters) =>
            {
                var creates = (IObservableCreatesAsync<string, TItemGrain>)grain;

                var newItemGrain = await creates.CreateForKey(parameters.key, parameters.grainType).ConfigureAwait(false);
                return newItemGrain;
            };
        }
        else
        {
            GetOrCreateImpl = (_, _) => throw new NotImplementedException($"Either implement {typeof(IObservableCreatesAsync<string, TItemGrain>).FullName} on {nameof(CollectionGrain)} or set {nameof(GetOrCreateImpl)}");
        }
    }

    public static Func<IGrain, (string, Type), Task<TItemGrain>> GetOrCreateImpl;

    #endregion

    public ReactiveCommand<(GrainId key, Type grainType), Task<TItemGrain>> GetOrCreateForKey { get; }

    //public Task<TItemGrain> GetOrCreateForKey(string key, Type type, params object[] constructorParameters)
    //{
    //    GrainType
    //    GrainId.Create(,)
    //    if (SourceCache.Keys.Contains(GrainId
    //    throw new NotImplementedException();
    //}

    //public virtual bool CanCreate => true;

    public IObservable<(GrainId key, Type type, object[] parameters, Task result)> CreatesForKey => throw new NotImplementedException();

    //public async Task<KeyValuePair<string, TItemGrain>> Create<TNewItem>(params object[]? constructorParameters)
    //    where TNewItem : TItemGrain
    //{
    //    //if (!CanCreate) { throw new NotSupportedException($"{nameof(CanCreate)}"); }
    //    if (!typeof(TNewItem).IsAssignableTo(typeof(TItemGrain))) { throw new ArgumentException($"TNewItem must be assignable to {typeof(TItemGrain).FullName}"); }
    //    if (constructorParameters != null && constructorParameters.Length != 0) { throw new NotImplementedException($"{nameof(constructorParameters)} not implemented"); }
    //    var result = await CreateImpl<TNewItem>();
    //    return new KeyValuePair<string, TItemGrain>(((IGrainWithStringKey)result).GetPrimaryKeyString(), result);
    //}

    public Task<TItemGrain> CreateForKey(GrainId key, Type type, params object[] constructorParameters)
    {
        throw new NotImplementedException();
    }

    Task<TItemGrain> ICreatesAsync<GrainId, TItemGrain>.GetOrCreateForKey(GrainId key, Type type, params object[] constructorParameters)
    {
        throw new NotImplementedException();
    }

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

    public async override Task<bool> Remove(TItemGrain item)
    {
        if (CollectionGrain == null) { throw new InvalidOperationException($"Cannot invoke while {nameof(CollectionGrain)} is null"); }

        //if (Options.TrackOperationsInProgress) {
        //    var changeSet
        //    (OperationsInProgress ??= new()).Add(new KeyValuePair<>()));
        //}

        bool result;
        if (CollectionGrain is IAsyncList<TItemGrain> dict)
        {
            result = await dict.Remove(item).ConfigureAwait(false);
        }
        else throw new NotSupportedException();

        //if (Options.TrackOperationsInProgress) { OperationsInProgress.Remove(item); if (RemovalsInProgrss.Count == 0) RemovalsInProgrss = null; }

        return result;
    }

    //public Task<bool> Remove(KeyValuePair<string, TItemGrain> item)
    //    => Remove(item.Key); // Alternative: ask the collection to remove the KeyValuePair

    #endregion

    #region Resolves

    public override IObservable<ITask<IResolveResult<IEnumerable<TItemGrain>>>> Resolves => throw new NotImplementedException();

    #region LazilyResolves

    public override bool HasValue => throw new NotImplementedException();

    public override IEnumerable<TItemGrain>? Value => throw new NotImplementedException();

    public override ITask<ILazyResolveResult<IEnumerable<TItemGrain>>> TryGetValue()
    {
        throw new NotImplementedException();
    }

    public override ILazyResolveResult<IEnumerable<TItemGrain>> QueryValue()
    {
        throw new NotImplementedException();
    }

    public override void DiscardValue()
    {
        throw new NotImplementedException();
    }

    #endregion

    #endregion

    #region Observing Changes

    ///// <summary>
    ///// Input: IAsyncObserver, Output: IObservable
    ///// </summary>
    //public IGrainObserverO<TItemGrain>? ChangeSetGrainObserver => changeSetGrainObserver;
    //private ChangeSetGrainObserver<TItemGrain>? changeSetGrainObserver;

    IAsyncDisposable grainObserverSubscription;
    AsyncGate grainObserverSubscriptionGate = new();

    public Task<bool> SubscribeViaGrainObserver()
    {
        using (await grainObserverSubscriptionGate.LockAsync().ConfigureAwait(false))
        {
#error FIXME: NEXT
            if (syncViaObserver == value) return;

            syncViaObserver = value;
            if (value)
            {
                var grainObserverSubscription = this.SubscribeViaGrainObserver().Result;
            }
            else
            {
                var reSyncViaObserver = ReSyncViaObserver;
                ReSyncViaObserver = null;
                reSyncViaObserver?.Dispose();

                CollectionGrain.UnsubscribeObserver(this);
            }
        }
    }
    bool syncViaObserver;

    public Task<bool> UnsubscribeViaGrainObserver()
    {

    }

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

    #region IEnumerable

    public override IEnumerator<TItemGrain> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region System.IAsyncObservable

    public ValueTask<IAsyncDisposable> SubscribeAsync(System.IAsyncObserver<ChangeSet<TItemGrain, GrainId>> observer)
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