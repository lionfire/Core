using DynamicData;
using LionFire.ExtensionMethods.Cloning;
using LionFire.FlexObjects;
using LionFire.Orleans_;
using LionFire.Orleans_.ObserverGrains;
using LionFire.Orleans_.Reactive_;
using LionFire.Threading;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Nito.Disposables;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LionFire.Orleans_.Collections;

public sealed class KeyedListObserverO<TKey, TItem>
    : IAsyncSubjectO<ChangeSet<TItem, TKey>>
    //, System.IAsyncObservable<ChangeSet<TItem, TKey>>
    , IAsyncDisposable
    where TKey : notnull
    where TItem : notnull
{

    #region Relationships

    public IGrainObservableAsyncObservableG<ChangeSet<TItem, TKey>> ObservableCollection { get; }
    //public IClusterClient ClusterClient { get; }

    IAsyncSubjectO<ChangeSet<TItem, TKey>> orleansObjectReference { get; }
    #endregion

    #region Lifecycle

    public KeyedListObserverO(IGrainObservableAsyncObservableG<ChangeSet<TItem, TKey>> observableCollection, IClusterClient clusterClient)
    {
        ObservableCollection = observableCollection;
        //ClusterClient = clusterClient;
        orleansObjectReference = clusterClient.CreateObjectReference<IAsyncSubjectO<ChangeSet<TItem, TKey>>>(this);
        Init();
        if (UseStats) Stats = new();
    }

    void Init()
    {
        subject = new();
    }

    public async ValueTask DisposeAsync()
    {
        var subscribeTaskLocal = Interlocked.Exchange(ref renewTask, null);
        if (subscribeTaskLocal != null) { await subscribeTaskLocal.ConfigureAwait(false); }

        var subscriptionRefCountCopy = Interlocked.Exchange(ref SubscriptionRefCount, null);
        if (subscriptionRefCountCopy != null) { await subscriptionRefCountCopy.DisposeAsync(); }

        //var subscriptionRefCountCopy = Interlocked.Exchange(ref asyncDisposables, null);
        //return subscriptionRefCountCopy != null ? ValueTaskEx.WhenAll(subscriptionRefCountCopy.Select(c => c.DisposeAsync())) : ValueTask.CompletedTask;
    }

    #endregion

    #region State

    public GrainObserverStatus Status { get; } = new();
    public GrainObserverStats? Stats { get; } = new();
    public static bool UseStats { get; set; } = false;

    private Lazy<ConcurrentSimpleAsyncSubject<ChangeSet<TItem, TKey>>> subject;


    RefCountAsyncDisposable? SubscriptionRefCount;

    public bool IsConnected => SubscriptionRefCount != null;
    private AsyncGate isConnectedGate = new();
    public bool IsConnecting { get; set; }
    private Task? renewTask = null;

    #endregion

    #region IAsyncObservable: SubscribeAsync

    // REFACTOR: I started to extract and generalize this in GrainObserverSubscriber.
    public async ValueTask<IAsyncDisposable> SubscribeAsync(IAsyncObserver<ChangeSet<TItem, TKey>> observer)
    {
        await subject.Value.SubscribeAsync(observer);

        return await (await Connect().ConfigureAwait(false)).GetDisposableAsync().ConfigureAwait(false);

        #region (Local) Connect (includes Disconnect)

        async ValueTask<RefCountAsyncDisposable> Connect(bool forceRenew = false)
        {
            using (await isConnectedGate.LockAsync().ConfigureAwait(false))
            {
                var subscriptionRefCount = SubscriptionRefCount;

                if (subscriptionRefCount != null)
                {
                    if (forceRenew) await ForceRenewGrainObserverSubscription().ConfigureAwait(false);
                }
                else
                {
                    try
                    {
                        CancellationTokenSource cts = new();

                        PeriodicTimer? renewTimer = null;
                        bool RenewalDesired() => renewTimer != null;
                        IsConnecting = true;
                        SubscriptionRefCount = subscriptionRefCount = new RefCountAsyncDisposable(System.Reactive.Disposables.AsyncDisposable.Create(
                            async () =>
                            {
                                cts.Cancel();
                                renewTimer!.Dispose(); // UNTESTED - does captured variable work as intended?
                                renewTimer = null;
                                SubscriptionRefCount = null;

                                using (await isConnectedGate.LockAsync().ConfigureAwait(false))
                                {
                                    await ObservableCollection.Unsubscribe(this).ConfigureAwait(false); // Disconnects
                                }
                            }));

                        var subSW = Stopwatch.StartNew();
                        var subscriptionTask = ObservableCollection.Subscribe(orleansObjectReference); // "Connects"
                        var timeoutTask = ObservableCollection.SubscriptionTimeout();
                        TimeSpan timeout;

                        try
                        {
                            await subscriptionTask.ConfigureAwait(false);
                            if (Stats != null) { Stats.LastSubscribeDuration = subSW.Elapsed; }
                            Status.LastRenewal = DateTimeOffset.Now;
                            Status.Timeout = timeout = await timeoutTask.ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            renewTimer = null;
                            await this.OnErrorAsync(ex).ConfigureAwait(false);
                            throw;
                        }

                        renewTimer = new PeriodicTimer(Status.RenewInterval = GrainObserverConfiguration.GetRenewInterval(timeout));

                        renewTask = Task.Run(async () =>
                        {
                            try
                            {
                                while (await renewTimer.WaitForNextTickAsync(cts.Token))
                                {
                                    await ObservableCollection.Subscribe(this.orleansObjectReference).ConfigureAwait(false);
                                    Status.LastRenewal = DateTimeOffset.Now;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                // Shouldn't throw because it may run in DisposeAsync
                            }
                        }, cts.Token);

                        SubscriptionRefCount = subscriptionRefCount; // IsConnected == true
                    }
                    finally
                    {
                        IsConnecting = false;
                    }
                }
                return subscriptionRefCount;
            }
        }
        #endregion
    }

    #endregion

    #region Subscribe

    public async ValueTask<bool> ForceRenewGrainObserverSubscription()
    {
        using (await isConnectedGate.LockAsync().ConfigureAwait(false))
        {
            if (IsConnected)
            {
                await ObservableCollection.Subscribe(this).ConfigureAwait(false);
                return true;
            }
            {
                return false;
            }
        }
    }

    #endregion

    #region System.IAsyncObserver<ChangeSet<TItem, TKey>> pass-thru to this.subject

    public ValueTask OnNextAsync(ChangeSet<TItem, TKey> value)
    {
        subject.Value.OnNextAsync(value);
        return ValueTask.CompletedTask;
    }

    public ValueTask OnCompletedAsync()
    {
        subject.Value.OnCompletedAsync();
        return ValueTask.CompletedTask;
    }

    public ValueTask OnErrorAsync(Exception error)
    {
        subject.Value.OnErrorAsync(error);
        return ValueTask.CompletedTask;
    }

    #endregion

    // vvv REVIEW vvv

    //public void BindTo(SourceCache<TItem, TKey> observableCache)
    //{
    //    this.subscribingObservable.SubscribeAsync(cs =>
    //    {
    //        observableCache.Edit(u => u.Clone(cs));
    //    });
    //}

    #region Misc

    private static ILogger l = Log.Get();

    #endregion
}

#if ENH // Optimize: only send GrainId over the network.  Missing dependency: Convert ChangeSet<TItem, TKey> to ChangeSet<TItem, TKey>

/// <summary>
/// Listens to ChangeSet<TItem, TKey> changes, and publishes ChangeSet<ChangeSet<TItem, TKey>, TKey> changes
/// 
/// Created for this scenario:
///  - GrainIds sent over the network
///  - TOutputItem is Grain types
///  - TOutputKey is the same GrainId that was sent over the network
/// </summary>
/// <typeparam name="TOutputItem"></typeparam>
/// <typeparam name="ChangeSet<TItem, TKey>"></typeparam>
public class TransformingKeySelectingListObserverO<ChangeSet<TItem, TKey>, TOutputKey, TOutputItem>
    : KeyedListObserverBaseO<TOutputKey, TOutputItem, ChangeSet<ChangeSet<TItem, TKey>>>
    , IChangeSetGrainObserver<ChangeSet<TItem, TKey>>
    , IObservable<ChangeSet<ChangeSet<TItem, TKey>>>
    where TOutputKey : notnull
{
    private readonly Func<ChangeSet<TItem, TKey>, TOutputItem> KeySelector;

    #region Lifecycle

    public TransformingKeySelectingListObserverO(Func<ChangeSet<TItem, TKey>, (TOutputKey key, TOutputItem item)> keySelector, IChangeSetObservableG<ChangeSet<TItem, TKey>> collectionGrain)
    {
        KeySelector = keySelector;

        observable = Observable.FromAsync<ChangeSet<ChangeSet<TItem, TKey>>>(async () =>
        {
            asyncDisposables.Add(await collectionGrain.SubscribeAsync(this));
            //await disposables.AddAsync(await collectionGrain.Subscribe(this));
        })
            .Publish()
            .RefCount()
            ;
    }

    #endregion
}

public abstract class KeyedListObserverBaseO<TKey, TItem, TChangeSet>
    : IObservable<ChangeSet<TItem, TKey>>
    where TKey : notnull
{
    #region (Private) fields


    #endregion

    
}
#endif

///// <summary>
///// Not Implemented
///// </summary>
///// <typeparam name="TItem"></typeparam>
//public class KeyedListObserverO<TItem>
//    : IGrainObserverO<TKey>
//    , IObservable<ChangeSet<TItem, TKey>>
//{
//    public KeyedListObserverO()
//    {
//        throw new NotImplementedException();
//    }

//    #region IObservable

//    IObservable<ChangeSet<TItem, TKey>> subscribingObservable;

//    public IDisposable Connect(IObserver<ChangeSet<TItem, TKey>> observer)
//        => ((IObservable<ChangeSet<TItem, TKey>>)subject).Connect(observer);

//    #endregion
//}
