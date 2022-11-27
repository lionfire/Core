using Orleans.Streams;

namespace LionFire.Orleans_;

public class AsyncObserverToOrleansStreamsAsyncObserver<T> : Orleans.Streams.IAsyncObserver<T>
{
    private Reactive.IAsyncObserver<T> asyncObserver;

    public AsyncObserverToOrleansStreamsAsyncObserver(LionFire.Reactive.IAsyncObserver<T> syncObserver)
    {
        this.asyncObserver = syncObserver;
    }

    public Task OnCompletedAsync() => asyncObserver.OnCompletedAsync();

    public Task OnErrorAsync(Exception ex) => asyncObserver.OnErrorAsync(ex);

    public Task OnNextAsync(T item, StreamSequenceToken? token = null) => asyncObserver.OnNextAsync(item);
    
}

public class ObserverToOrleansStreamsAsyncObserver<T> : Orleans.Streams.IAsyncObserver<T>
{
    private IObserver<T> syncObserver;

    public ObserverToOrleansStreamsAsyncObserver(IObserver<T> syncObserver)
    {
        this.syncObserver = syncObserver;
    }

    public Task OnCompletedAsync()
    {
        syncObserver.OnCompleted();
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
        syncObserver.OnError(ex);
        return Task.CompletedTask;
    }

    public Task OnNextAsync(T item, StreamSequenceToken? token = null)
    {
        syncObserver.OnNext(item);
        return Task.CompletedTask;
    }
}