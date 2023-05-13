#if false // Use instead: System.Reactive.Async
namespace LionFire.Reactive;

public interface IAsyncObservable<T>
{
    Task<IAsyncDisposable> SubscribeAsync(IAsyncObserver<T> observer);
}
#endif