#if UNUSED // conflicts, REVIEW: alongside LionFire.Data.Async IAsyncReadOnlyDictionaryCache
using LionFire.Structures;
using System.Diagnostics.CodeAnalysis;

namespace LionFire.Structures;

public interface IAsyncReadOnlyDictionary<TKey, TValue> : IEnumerableAsync<KeyValuePair<TKey, TValue>>
    , IAsyncReadOnlyCollection<KeyValuePair<TKey, TValue>>
{

    Task<TValue> this[TKey key] { get; }

    Task<IEnumerable<TKey>> Keys { get; }

    Task<IEnumerable<TValue>> Values { get; }

    Task<bool> ContainsKey(TKey key);

    Task<bool> TryGetValue(TKey key, /*[MaybeNullWhen(false)]*/ out TValue value);
    

}
#endif