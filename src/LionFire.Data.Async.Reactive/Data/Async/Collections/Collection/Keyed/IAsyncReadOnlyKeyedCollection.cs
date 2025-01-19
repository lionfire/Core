using DynamicData;

namespace LionFire.Data.Collections;

/// <summary>
/// A Keyed Collection is for TItems that know their own TKey (or a selector func can be used to determine it.)
/// </summary>
/// <remarks>
/// See also:
/// - If TItem does not know its own TKey, but you still want a Dictionary style collection, use IAsyncDictionary&lt;TKey,TItem&gt; instead.
/// </remarks>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
public interface IAsyncReadOnlyKeyedCollection<TKey, TItem> 
    : IAsyncReadOnlyCollection<TItem>
    , IHasObservableCache<TItem, TKey>
    where TKey : notnull
    where TItem : notnull
{
}

