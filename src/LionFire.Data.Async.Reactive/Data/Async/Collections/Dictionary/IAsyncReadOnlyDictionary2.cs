using DynamicData;

namespace LionFire.Data.Collections;

/// <summary>
/// The Async means the collection can be retrieved asynchronously.
/// </summary>
/// <remarks>
/// See also:
/// - If TItem knows its own TKey or a selector can determine TKey from TItem, consider using IAsyncReadOnlyKeyedCollection&lt;TKey,TItem&gt; instead.
/// 
/// Suggested interfaces:
///  - ISubscribes<IEnumerable<KeyValuePair<TKey, TItem>>>
/// </remarks>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public interface IAsyncReadOnlyDictionary2<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    IObservableCache<(TKey key, TValue value), TKey> ObservableCache { get; }


}

