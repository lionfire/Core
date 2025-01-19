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
