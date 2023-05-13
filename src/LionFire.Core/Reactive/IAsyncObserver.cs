#if false // Use instead: System.Reactive.Async (currently prerelease)
namespace LionFire.Reactive;

public interface IAsyncObserver<in T>
{
    // REVIEW: Use ValueTask instead
    Task OnNextAsync(T item);
    Task OnCompletedAsync();
    Task OnErrorAsync(Exception ex);
}

#endif