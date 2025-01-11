using ReactiveUI;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Linq.Async;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace LionFire.Data.Collections;

/// <summary>
/// An IObservableCache&lt;TValue, TKey&gt; that is retrieved asynchronously.
/// </summary>
public abstract class AsyncReadOnlyDictionary2<TKey, TValue>
    : IObservableCache<(TKey key, TValue value), TKey>
    //IAsyncReadOnlyDictionary2<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    public IObservableCache<(TKey key, TValue value), TKey> ObservableCache => sourceCache.AsObservableCache();
    private SourceCache<(TKey key, TValue value), TKey> sourceCache = new(kv => kv.key);


    IObservable<IChangeSet<(TKey key, TValue value), TKey>> observable;

    public void AsyncReadOnlyDictionary2()
    {
         observable = Observable.Create<IChangeSet<(TKey key, TValue value), TKey>>(observer =>
        {
            StartFeedingData();

            var subscription = sourceCache.Connect().Subscribe(observer);

            return Disposable.Create(() =>
            {
                // TODO: Ref counting
                StopFeedingData();
                subscription.Dispose();
            });
        });
    }

    void StartFeedingData()
    {

    }
    void StopFeedingData()
    {

    }

}
