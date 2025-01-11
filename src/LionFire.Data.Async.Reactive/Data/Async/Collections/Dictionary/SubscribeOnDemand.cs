using DynamicData;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using DynamicData.Kernel;
using System.Reactive.Subjects;

namespace LionFire.Data.Collections;

public abstract class SubscribeOnDemand<TKey, TValue> : IObservableCache<(TKey key, TValue value), TKey>
    where TKey : notnull
    where TValue : notnull
{
    #region Lifecycle

    private readonly CompositeDisposable disposables = new();

    public SubscribeOnDemand()
    {
        subscriberCount
            .DistinctUntilChanged()
            .Subscribe(count =>
            {
                lock (syncLock)
                {
                    if (count > 0 && !isRetrieving)
                    {
                        isRetrieving = true;
                        StartRetrievingData();
                    }
                    else if (count == 0 && isRetrieving)
                    {
                        isRetrieving = false;
                        StopRetrievingData();
                    }
                }
            })
            .DisposeWith(disposables);
    }

    public void Dispose()
    {
        disposables.Dispose();
        sourceCache.Dispose();
        subscriberCount.Dispose();
    }

    #endregion

    #region State

    public IObservableCache<(TKey key, TValue value), TKey> ObservableCache => sourceCache;
    private readonly SourceCache<(TKey key, TValue value), TKey> sourceCache = new(kv => kv.key);

    private readonly BehaviorSubject<int> subscriberCount = new(0);
    private readonly object syncLock = new();
    private bool isRetrieving;

    #endregion

    #region Abstract

    protected abstract void StartRetrievingData();
    protected abstract void StopRetrievingData();

    #endregion

    public IObservable<IChangeSet<(TKey key, TValue value), TKey>> Connect(Func<(TKey key, TValue value), bool>? predicate = null, bool suppressEmptyChangeSets = true)
    {
        return Observable.Using(
            () =>
            {
                subscriberCount.OnNext(subscriberCount.Value + 1);
                return Disposable.Create(() =>
                {
                    subscriberCount.OnNext(subscriberCount.Value - 1);
                });
            },
            _ => sourceCache.Connect(predicate, suppressEmptyChangeSets)
        );
    }

    #region Pass-thru

    public IReadOnlyList<(TKey key, TValue value)> Items => sourceCache.Items;
    //IReadOnlyList<(TKey key, TValue value)> IObservableCache<(TKey key, TValue value), TKey>.Items => sourceCache.Items;

    public IReadOnlyList<TKey> Keys => sourceCache.Keys;
    //IReadOnlyList<TKey> IObservableCache<(TKey key, TValue value), TKey>.Keys => sourceCache.Keys;
    public IReadOnlyDictionary<TKey, (TKey key, TValue value)> KeyValues => sourceCache.KeyValues;

    public int Count => sourceCache.Count;
    public IObservable<int> CountChanged => sourceCache.CountChanged;

    public Func<(TKey key, TValue value), TKey> KeySelector => sourceCache.KeySelector;

    public Optional<(TKey key, TValue value)> Lookup(TKey key) => sourceCache.Lookup(key);
    public IObservable<Change<(TKey key, TValue value), TKey>> Watch(TKey key) => sourceCache.Watch(key);

    /// <summary>
    /// Preview does not initiate retrieval of data
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IObservable<IChangeSet<(TKey key, TValue value), TKey>> Preview(Func<(TKey key, TValue value), bool>? predicate = null) => sourceCache.Preview(predicate);

    #endregion
}
