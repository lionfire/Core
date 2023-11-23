using Orleans.Streams;

namespace LionFire.Orleans_.Streams;

public class StreamSubscriptionHandleAsyncWrapper<T> : IAsyncDisposable
{
    StreamSubscriptionHandle<T> inner;

    public StreamSubscriptionHandleAsyncWrapper(StreamSubscriptionHandle<T> inner)
    {
        this.inner = inner;
    }
    public async ValueTask DisposeAsync()
    {
        await inner.UnsubscribeAsync().ConfigureAwait(false);
    }
    
}
