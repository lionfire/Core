using System.Reactive.Subjects;

namespace LionFire.Reactive;

public static class IConnectableObservableX
{
    public static IObservable<T> RefCountWithEvents<T>(
           this IConnectableObservable<T> source,
           Action? onFirstSubscribe = null,
           Action? onLastDispose = null)
    {
        var sourceWithRefCount = source.RefCount();
        return sourceWithRefCount.OnAttachEvents(onFirstSubscribe, onLastDispose);
    }
}
