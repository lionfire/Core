namespace LionFire.Data.Collections;

// REFACTOR:
//  - IAsyncReadOnlyDictionaryCache and IAsyncReadOnlyKeyedCollectionCache inherit from this
//public interface IAsyncReadOnlyKeyedCollectionCacheBase<TKey, TItem>
//{
//    IObservableCache<TItem, TKey> ObservableCache { get; }
//}

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Suggested interfaces:
///  - ISubscribes&lt;IEnumerable&lt;KeyValuePair&lt;TKey, TItem&gt;&gt;&gt;
/// </remarks>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
public interface IObservableCacheKeyableGetter<TKey, TItem>
    : IEnumerableGetter<TItem>
    , IHasObservableCache<TItem, TKey>
    where TKey : notnull
{
    Func<TItem, TKey> KeySelector { get; }
}

public interface IObservableCacheKeyValueGetter<TKey, TItem>
    : IEnumerableGetter<KeyValuePair<TKey, TItem>>
    , IHasObservableCache<TItem, TKey>
    where TKey : notnull
{
}

