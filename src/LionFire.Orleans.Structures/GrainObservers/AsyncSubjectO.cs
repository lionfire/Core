using LionFire.Orleans_.Reactive_;
using System.Diagnostics;
using System.Reactive.Disposables;

namespace LionFire.Orleans_.ObserverGrains;

public class AsyncSubjectO<T>(IGrainObservableAsyncObservableG<T> observableGrain)
    : GrainObserverSubscriber<T>(observableGrain)
    , IAsyncSubjectO<T>
{
    #region Relationship

    public IGrainObservableAsyncObservableG<T> ObservableCollection { get; } = observableGrain;

    #endregion

    IAsyncSubjectO<T> orleansObjectReference { get; }
    private Lazy<ConcurrentSimpleAsyncSubject<T>> subject;

    #region Parameters

    #region (Static)

    public static bool UseStats { get; set; } = false;

    #endregion

    #endregion

    #region State

    RefCountAsyncDisposable? SubscriptionRefCount;
    private AsyncGate isConnectedGate = new();
    public bool IsConnecting { get; set; }
    private Task? renewTask = null;

    public GrainObserverStats? Stats { get; } = new();

    #region Derived

    public bool IsConnected => SubscriptionRefCount != null;

    #endregion

    #endregion

    public async ValueTask<bool> ForceRenewGrainObserverSubscription()
    {
        using (await isConnectedGate.LockAsync().ConfigureAwait(false))
        {
            if (IsConnected)
            {
                // TODO: Client Code
                await ObservableCollection.Subscribe(this).ConfigureAwait(false);
                return true;
            }
            {
                return false;
            }
        }
    }

    public async ValueTask<IAsyncDisposable> SubscribeAsync(IAsyncObserver<T> observer)
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
                                    // TODO: 
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

    #region System.IAsyncObserver<T> pass-thru to this.subject

    public ValueTask OnNextAsync(T value)
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
}
