using LionFire.Orleans_.ObserverGrains;
using LionFire.Threading;

namespace LionFire.Orleans_;

public static class IGrainObservableGX
{

    public static async Task<IAsyncDisposable> SubscribeWithRenew<TObserver, TSubscribeParameters>(this IGrainObservableG<TObserver, TSubscribeParameters> grain, TObserver subscriber, TSubscribeParameters options)
    {
        var sub = new _ObservableSubscription<TObserver, TSubscribeParameters>(grain);
        await sub.Subscribe(subscriber, options);
        return sub;
    }

    private class _ObservableSubscription<TObserver, TSubscribeParameters>
        : IAsyncDisposable
    {

        PeriodicTimer? renewTimer;
        CancellationTokenSource cts = new();

        private readonly IGrainObservableG<TObserver, TSubscribeParameters> observableGrain;
        private TObserver? subscriber;

        public _ObservableSubscription(IGrainObservableG<TObserver, TSubscribeParameters> Grain)
        {
            observableGrain = Grain;
        }

        public async ValueTask Subscribe(TObserver subscriber, TSubscribeParameters options)
        {
            var timeSpan = await observableGrain.Subscribe(subscriber, options);
            if (cts.IsCancellationRequested) return;

            var renewSeconds = timeSpan.TotalSeconds - 35;
            renewSeconds = Math.Min(Math.Max(6, renewSeconds), timeSpan.TotalSeconds * 0.9);
            var renewTimeSpan = TimeSpan.FromSeconds(renewSeconds);

            if (renewTimer == null || renewTimer.Period != renewTimeSpan)
            {
                renewTimer = new PeriodicTimer(renewTimeSpan);
            }
            Task.Run(async () =>
            {
                if (await renewTimer.WaitForNextTickAsync(cts.Token))
                {
                    await Subscribe(subscriber, options);
                }
            }).FireAndForget();
        }

        public bool IsDisposed => subscriber == null;
        public async ValueTask DisposeAsync()
        {
            renewTimer?.Dispose();
            cts.Cancel();
            var subCopy = subscriber;
            if (subCopy != null)
            {
                subscriber = default;
                await observableGrain.Unsubscribe(subCopy);
            }
        }
    }
}
