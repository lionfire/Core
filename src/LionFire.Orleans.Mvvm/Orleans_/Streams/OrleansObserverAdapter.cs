//using ObservableCollections;
using Orleans.Streams;

namespace LionFire.Orleans_.Streams;

public class OrleansObserverAdapter<T> : IObservable<T>
{
    StreamSubscriptionHandle<T> handle;

    public static async Task<OrleansObserverAdapter<T>> Create(Reactive.IAsyncObserver<T> observer1, Orleans.Streams.IAsyncObservable<T> parent)
    {
        var orleansAsyncObserver = new AsyncObserverToOrleansStreamsAsyncObserver<T>(observer1);

        var result = new OrleansObserverAdapter<T>()
        {
            handle = await parent.SubscribeAsync(orleansAsyncObserver)
        };
        return result;
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
        return new StreamSubscriptionHandleWrapper<T>(handle);

    }
}