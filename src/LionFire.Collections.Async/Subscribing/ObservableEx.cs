using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace LionFire.Subscribing;

public static class ObservableEx
{
    public static IObservable<T> Return<T>(T value) => Observable.Create<T>(o =>
    {
        o.OnNext(value);
        o.OnCompleted();
        return Disposable.Empty;
    });
}
