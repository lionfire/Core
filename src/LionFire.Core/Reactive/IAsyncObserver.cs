namespace LionFire.Reactive;

public interface IAsyncObserver<in T>
{
    Task OnNextAsync(T item);
    Task OnCompletedAsync();
    Task OnErrorAsync(Exception ex);
}
