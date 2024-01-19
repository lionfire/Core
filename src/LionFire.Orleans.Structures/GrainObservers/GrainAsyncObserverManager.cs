
using DynamicData;
using LionFire.Orleans_.Reactive_;
using Microsoft.Extensions.Logging;
using Orleans.Utilities;

namespace LionFire.Orleans_.ObserverGrains;

public class AsyncObserverGrainObserverManager<T>
{
    #region Dependencies

    //public ILogger Logger { get; }
    static ILogger Logger = Log.Get<AsyncObserverGrainObserverManager<T>>();
    public IGrainObservableG<T> Grain { get; }

    #endregion

    #region Parameters

    public TimeSpan SubscriptionTimeout { get; }

    #endregion

    #region Lifecycle

    public AsyncObserverGrainObserverManager(IGrainObservableG<T> grain, TimeSpan timeSpan
        //, ILogger logger
        )
    {
        Grain = grain;
        SubscriptionTimeout = timeSpan;
        //Logger = logger;
        observers = new ObserverManager<IAsyncObserverO<T>>(SubscriptionTimeout, Logger);
    }

    #endregion

    #region State

    private readonly ObserverManager<IAsyncObserverO<T>> observers;

    #endregion

    #region Methods

    public void Subscribe(IAsyncObserverO<T> subscriber) => observers.Subscribe(subscriber, subscriber);
    // OLD
    //var result = new GrainObserverSubscription<T>()
    //{
    //    Observable = Grain,
    //    Subscriber = subscriber,
    //};
    //return Task.FromResult(result);

    public void Unsubscribe(IAsyncObserverO<T> subscriber) => observers.Unsubscribe(subscriber);

    public Task NotifyObservers(T message)
    {
        observers.Notify(s => s.OnNextAsync(message));
        return Task.CompletedTask;
    }

    #endregion
}
