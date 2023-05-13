
namespace LionFire.Orleans_.Collections;

[GenerateSerializer]
public class GrainObserverSubscription<T> : IAsyncDisposable
{
    [Id(0)]
    public IGrainObserverO<T>? Subscriber { get; set; }
    [Id(1)]
    public IGrainObservableG<T>? Observable { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (Subscriber != null && Observable != null)
        {
            var s = Subscriber;
            Subscriber = null;
            var o = Observable;
            Observable = null;
            await o.Unsubscribe(s).ConfigureAwait(false);
        }
    }
}
