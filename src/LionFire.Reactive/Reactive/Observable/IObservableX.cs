using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Reactive;

public static class IObservableX
{
    public static IObservable<IChangeSet<TObject, TKey>> CreateConnectOnDemand<TObject, TKey>(Func<TObject, TKey> keySelector,
          Func<SourceCache<TObject, TKey>, IDisposable> resourceFactory,
          Func<TObject, bool>? predicate = null,
          bool suppressEmptyChangeSets = true)
          where TKey : notnull
          where TObject : notnull
    {
        var sourceCache = new SourceCache<TObject, TKey>(keySelector);

        return Observable.Using(() => resourceFactory(sourceCache),
                    _ => sourceCache.Connect(predicate, suppressEmptyChangeSets)
                ).RefCount();
    }

    public static IObservable<T> PublishRefCountWithEvents<T>(
        this IObservable<T> source,
        Action? onFirstSubscribe = null,
        Action? onLastDispose = null)
    {
        var sourceWithRefCount = source
            .Publish()
            .RefCount();
        return sourceWithRefCount.OnAttachEvents(onFirstSubscribe, onLastDispose);
    }

    public static IObservable<T> OnAttachEvents<T>(
       this IObservable<T> source,
       Action? onFirstSubscribe = null,
       Action? onLastDispose = null)
    {
        // Static field to track subscription count for this specific observable
        int subscriptionCount = 0;

        return Observable.Create<T>(observer =>
        {
            // Increment count and check if first subscriber
            if (Interlocked.Increment(ref subscriptionCount) == 1)
            {
                onFirstSubscribe?.Invoke();
            }

            var subscription = source.Subscribe(observer);

            return Disposable.Create(() =>
            {
                subscription.Dispose();
                // Decrement count and check if last subscriber
                if (Interlocked.Decrement(ref subscriptionCount) == 0)
                {
                    onLastDispose?.Invoke();
                }
            });
        });
    }
}
