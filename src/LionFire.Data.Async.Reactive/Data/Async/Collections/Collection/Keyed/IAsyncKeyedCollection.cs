using DynamicData;
using LionFire.Data.Async;

namespace LionFire.Data.Collections;

public interface IAsyncKeyedCollection<TKey, TItem> : IAsyncReadOnlyKeyedCollection<TKey, TItem>
    , IAsyncCollection<TItem>
    where TKey : notnull
    where TItem : notnull
{
    ValueTask<bool> Remove(TKey key);

    ValueTask Add(TItem item);
    ValueTask Upsert(TItem item);

}

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
    where TKey : notnull
    where TItem : notnull
{
    IObservableCache<TItem, TKey> ObservableCache { get; }
}


#if ENH 

/// <summary>
/// NOTIMPLEMENTED
/// Use this to watch for changes to members of a collection you are only partially subscribe to.
/// You can then opt in to items of interest.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
public interface IAsyncKeyedCollectionWithObservableKeys<TKey, TItem> : IAsyncKeyedCollection<TKey, TItem>
    where TKey : notnull
    where TItem : notnull
{
    IObservableList<TKey> ObservableList { get; }
}



/// <summary>
/// Ability to subscribe to an IObservableCache containing only selected keys
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
public interface ISubscriptionToKeys<TKey, TItem>
    where TKey : notnull
    where TItem : notnull
{
    void Subscribe(TKey key);
    void Unsubscribe(TKey key);

    IObservableCache<TItem, TKey> ObservableCache { get; }

}


#endif
