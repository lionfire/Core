using DynamicData;

namespace LionFire.Data.Collections;

// Difference from IAsyncReadOnlyDictionary: Instead of implementing IGetter, inject an IObservableCache.
// FUTURE: Obsolete IAsyncReadOnlyDictionary and replace with IAsyncReadOnlyDictionary2.

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

public interface IAsyncDictionary2<TKey, TItem>
    : IAsyncReadOnlyDictionary2<TKey, TItem>
    where TKey : notnull
    where TItem : notnull
{
    ValueTask<bool> Remove(TKey key);

    ValueTask<bool> TryAdd(TKey key, TItem item);
    ValueTask Upsert(TKey key, TItem item);
}
