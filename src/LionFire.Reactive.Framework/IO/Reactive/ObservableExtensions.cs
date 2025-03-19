using System.Reactive.Disposables;

namespace LionFire.IO.Reactive;

// Custom extension method to track subscriber count
public static class ObservableExtensions
{
    public static IObservable<int> SubscribeCount<T>(this IObservable<T> source)
    {
        var count = 0;
        return Observable.Create<int>(observer =>
        {
            var subscription = source.Subscribe(
                _ => { }, // Ignore OnNext events
                observer.OnError,
                observer.OnCompleted);

            count++;
            observer.OnNext(count);

            return Disposable.Create(() =>
            {
                count--;
                observer.OnNext(count);
                subscription.Dispose();
            });
        });
    }
}