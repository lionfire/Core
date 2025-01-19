
namespace LionFire.Data.Collections;

/// Also consider these:
///  - IAsyncAdds<TKey, TItem>
///  - IAsyncCreates<TItem>
///  - IAsyncCreatesForKey<TItem>

/// <summary>
/// Augments an IObservableCache (IAsyncReadOnlyDictionary) with async mutation methods
/// 
/// </summary>
/// <remarks>
/// See also:
/// - If TItem knows its own TKey or a selector can determine TKey from TItem, consider using IAsyncKeyedCollection&lt;TKey,TItem&gt; instead.
/// </remarks>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
public interface IAsyncDictionary<TKey, TItem> 
    : IAsyncReadOnlyDictionary<TKey, TItem>
    where TKey : notnull
    where TItem : notnull 
{
    ValueTask<bool> Remove(TKey key);

    ValueTask<bool> TryAdd(TKey key, TItem item);
    ValueTask Upsert(TKey key, TItem item);
}

//internal interface IAsyncDictionaryCacheInternal<TKey, TItem> : IAsyncDictionaryCache<TKey, TItem>
//    where TKey : notnull
//{
//    SourceCache<TItem, TKey> SourceCache { get; }
//}
