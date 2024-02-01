using LionFire.Orleans_.Reactive_;

namespace LionFire.Orleans_.ObserverGrains;

public interface IGrainObservableG : IAddressable
{
    /// <summary>
    /// Observer must re-subscribe before this timeout elapses, or the subscription will be terminated.
    /// </summary>
    /// <returns></returns>
    ValueTask<TimeSpan> SubscriptionTimeout(); // REVIEW - DEPRECATE this and return TimeSpace in Subscribe() instead?

}
public interface IGrainObservableG<TObserver> : IGrainObservableG
{
    ValueTask<TimeSpan> Subscribe(TObserver subscriber);
    ValueTask Unsubscribe(TObserver subscriber);

}

public interface IGrainObservableG<TObserver, TSubscribeParameters> : IGrainObservableG
{
    ValueTask<TimeSpan> Subscribe(TObserver subscriber, TSubscribeParameters options);
    ValueTask Unsubscribe(TObserver subscriber);
}

public interface IGrainObservableAsyncObservableG<T> : IGrainObservableG<IAsyncObservableO<T>>
{
}

//[GenerateSerializer]
//public class ObserverGrainOptions
//{
//    [Id(0)]
//    public TimeSpan Timeout { get; set; }
//}

