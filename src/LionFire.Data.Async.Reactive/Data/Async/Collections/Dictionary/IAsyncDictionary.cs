using DynamicData;

namespace LionFire.Data.Collections;


// Features included:
//  - Remove
//  - Get
//
// Also consider these:
//  - IAsyncAdds<TKey, TItem>
//  - IAsyncCreates<TItem>
//  - IAsyncCreatesForKey<TItem>
public interface IAsyncDictionary<TKey, TItem> 
    : IAsyncCollection<KeyValuePair<TKey, TItem>>
    , IAsyncReadOnlyDictionary<TKey, TItem>
    where TKey : notnull
{ 
    Task<bool> Remove(TKey key);
}


//internal interface IAsyncDictionaryCacheInternal<TKey, TItem> : IAsyncDictionaryCache<TKey, TItem>
//    where TKey : notnull
//{
//    SourceCache<TItem, TKey> SourceCache { get; }
//}
