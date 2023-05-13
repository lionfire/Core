using LionFire.Orleans_.Reactive_;

namespace LionFire.Orleans_.Collections;

public interface IAsyncObservableG<TValue>
{
    ValueTask<GrainObserverSubscription<TValue>> SubscribeAsync(IAsyncObserverO<TValue> observer);
}
