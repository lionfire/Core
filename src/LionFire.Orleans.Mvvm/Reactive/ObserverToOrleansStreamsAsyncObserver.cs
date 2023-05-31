using Orleans.Streams;

namespace LionFire.Orleans_;

//public class AsyncSystemObserverToOrleansObserver<TValue> : //? 
//{
//}

public class AsyncSystemObserverToOrleansStreamsAsyncObserver<T> : Orleans.Streams.IAsyncObserver<T>
{
    private System.IAsyncObserver<T> asyncObserver;

    public AsyncSystemObserverToOrleansStreamsAsyncObserver(System.IAsyncObserver<T> syncObserver)
    {
        this.asyncObserver = syncObserver;
    }

    public Task OnCompletedAsync() => asyncObserver.OnCompletedAsync().AsTask();

    public Task OnErrorAsync(Exception ex) => asyncObserver.OnErrorAsync(ex).AsTask();

    public Task OnNextAsync(T item, StreamSequenceToken? token = null) => asyncObserver.OnNextAsync(item).AsTask();
    
}
