using DynamicData;

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
public interface IObservableCacheGetter<TKey, TItem>
    : IEnumerableGetter<KeyValuePair<TKey, TItem>>
    where TKey : notnull
{
    IObservableCache<TItem, TKey> ObservableCache { get; } 
}

