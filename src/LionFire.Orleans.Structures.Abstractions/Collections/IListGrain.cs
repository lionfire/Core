using LionFire.Structures;

namespace LionFire.Orleans_.Collections;

public interface IAsyncDictionary<TKey, TValue>
    : IEnumerableAsync<KeyValuePair<TKey, TValue>>
{
    Task<bool> Remove(TKey id);
}

public interface IAsyncCreating<TValue>
{
    Task<TValue> Create(Type type/*, Action<TValue>? init = null*/);

    Task<IEnumerable<Type>> CreateableTypes();
}
public interface ICreatingAsyncDictionary<TKey, TValue>
    : IAsyncDictionary<string, TValue>
    , IAsyncCreating<TValue>
    //EnumerableAsync<TValue> // Change to KVP<TKey,TValue>?    
{
}

public interface IListAsync<TItem> : IEnumerableAsync<TItem>
{
    Task Add(TItem item);
    Task<bool> Remove(TItem item);

}

public interface IListGrain<TItem> : IListAsync<TItem>, IGrain
{
   
}

public interface IPolymorphicListGrain<TItem> : IGrain
{
}

// Idea with metadata
// public interface IListGrain<TValue, TMetadata>
//    where TValue : IGrain
//{
//    Task<bool> Remove(string id);
//    Task<(string id, TValue newValue)> Create(Type type);

//    Task<IEnumerable<Type>> CreateableTypes();

//    Task<IEnumerable<GrainListItem<TValue, TMetadata>>> Items();

//}
