using DynamicData;

namespace LionFire.Data.Async.Collections;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Suggested interfaces:
///  - ISubscribes&lt;IEnumerable&lt;KeyValuePair&lt;TKey, TItem&gt;&gt;&gt;
/// </remarks>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
public interface IAsyncReadOnlyDictionaryCache<TKey, TItem>
    : IAsyncReadOnlyCollectionCacheBase<KeyValuePair<TKey, TItem>>
    where TKey : notnull
{

    IObservableCache<TItem, TKey> ObservableCache { get; } 

}

