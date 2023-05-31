using DynamicData;

namespace LionFire.Collections.Async;

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
public interface IAsyncReadOnlyKeyedCollectionCache<TKey, TItem>
    : IAsyncReadOnlyCollectionCacheBase<TItem>
    where TKey : notnull
{
    IObservableCache<TItem, TKey> ObservableCache { get; } 
}

