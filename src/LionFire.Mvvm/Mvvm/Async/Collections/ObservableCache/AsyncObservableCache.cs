//#if UNUSED
//#nullable enable

//using DynamicData;
//using DynamicData.Kernel;

//namespace LionFire.Blazor.Components;

//public class AsyncObservableCache_OLD<TKey, TValue> : IObservableCache<TValue, TKey>
//    where TKey : notnull
//{
//    public SourceCache<TValue, TKey> SourceCache { get; protected set; }

//    #region IObservableCache

//    public int Count => SourceCache.Count;

//    public IEnumerable<TValue> Items => SourceCache.Items;

//    public IEnumerable<TKey> Keys => SourceCache.Keys;

//    public IEnumerable<KeyValuePair<TKey, TValue>> KeyValues => SourceCache.KeyValues;

//    public IObservable<int> CountChanged => SourceCache.CountChanged;

//    public IObservable<IChangeSet<TValue, TKey>> Connect(Func<TValue, bool>? predicate = null, bool suppressEmptyChangeSets = true) => SourceCache.Connect(predicate, suppressEmptyChangeSets);

//    public void Dispose() => SourceCache.Dispose();

//    public Optional<TValue> Lookup(TKey key) => SourceCache.Lookup(key);

//    public IObservable<IChangeSet<TValue, TKey>> Preview(Func<TValue, bool>? predicate = null) => SourceCache.Preview(predicate);

//    public IObservable<Change<TValue, TKey>> Watch(TKey key) => SourceCache.Watch(key);

//    #endregion

//    public virtual Task AddOrUpdate(TValue item)
//    {
//        SourceCache.AddOrUpdate(item);
//        return Task.CompletedTask;
//    }
//}

//#endif