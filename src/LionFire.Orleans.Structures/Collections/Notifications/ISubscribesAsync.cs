namespace LionFire.Orleans_;

public interface ISubscribesAsync
{
    IEnumerable<IAsyncDisposable> Subscriptions { get; }
    void OnSubscribing(IAsyncDisposable subscription);
    ValueTask Unsubscribe();
}
