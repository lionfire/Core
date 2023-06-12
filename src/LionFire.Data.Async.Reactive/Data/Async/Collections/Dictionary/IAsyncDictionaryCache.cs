using DynamicData;

namespace LionFire.Collections.Async;


// Features included:
//  - Remove
//  - Get
//
// Also consider these:
//  - IAsyncAdds<TKey, TItem>
//  - IAsyncCreates<TItem>
//  - IAsyncCreatesForKey<TItem>
public interface IAsyncDictionaryCache<TKey, TItem> : IAsyncCollectionCache<KeyValuePair<TKey, TItem>>
    where TKey : notnull
{
    IObservableCache<TItem, TKey> ObservableCache { get; }

    Task<bool> Remove(TKey key);
}


//internal interface IAsyncDictionaryCacheInternal<TKey, TItem> : IAsyncDictionaryCache<TKey, TItem>
//    where TKey : notnull
//{
//    SourceCache<TItem, TKey> SourceCache { get; }
//}
