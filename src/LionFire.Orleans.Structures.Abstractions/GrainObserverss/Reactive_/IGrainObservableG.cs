using LionFire.Orleans_.Reactive_;

namespace LionFire.Orleans_.Collections;

public interface IGrainObservableG<T> : IAddressable
{
    ValueTask Subscribe(IAsyncObserverO<T> subscriber);
    ValueTask Unsubscribe(IAsyncObserverO<T> subscriber);

    /// <summary>
    /// Observer must re-subscribe before this timeout elapses, or the subscription will be terminated.
    /// </summary>
    /// <returns></returns>
    ValueTask<TimeSpan> SubscriptionTimeout();
}

//[GenerateSerializer]
//public class ObserverGrainOptions
//{
//    [Id(0)]
//    public TimeSpan Timeout { get; set; }
//}

