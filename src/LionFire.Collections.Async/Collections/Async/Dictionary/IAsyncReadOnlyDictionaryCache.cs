using DynamicData;

namespace LionFire.Collections.Async;

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
    : IAsyncReadOnlyCollectionCache<KeyValuePair<TKey, TItem>>
    where TKey : notnull
{

    IObservableCache<TItem, TKey> Cache { get; } 

}

