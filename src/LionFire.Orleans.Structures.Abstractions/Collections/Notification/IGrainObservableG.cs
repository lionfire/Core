namespace LionFire.Orleans_.Collections;

public interface IGrainObservableG<T> : IAddressable
{
    /// <summary>
    /// Observer must resubscribe before this timeout elapses, or the subscription will be terminated.
    /// </summary>
    /// <returns></returns>
    Task<TimeSpan> SubscriptionTimeout();

    Task<GrainObserverSubscription<T>> Subscribe(IGrainObserverO<T> observer);
    Task Unsubscribe(IGrainObserverO<T> observer);
}


