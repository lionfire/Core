using Orleans.Streams;

namespace LionFire.Orleans_;

public class SystemObserverToOrleansStreamsAsyncObserver<T> : Orleans.Streams.IAsyncObserver<T>
{
    private IObserver<T> syncObserver;

    public SystemObserverToOrleansStreamsAsyncObserver(IObserver<T> syncObserver)
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