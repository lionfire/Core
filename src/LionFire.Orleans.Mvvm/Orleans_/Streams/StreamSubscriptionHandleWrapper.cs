//using ObservableCollections;
using Orleans.Streams;
using LionFire.Threading;

namespace LionFire.Orleans_.Streams;

public class StreamSubscriptionHandleWrapper<T> : IDisposable
{
    StreamSubscriptionHandle<T> inner;

    public StreamSubscriptionHandleWrapper(StreamSubscriptionHandle<T> inner)
    {
        this.inner = inner;
    }
    public void Dispose()
    {
        inner.UnsubscribeAsync().FireAndForget();
    }
}
