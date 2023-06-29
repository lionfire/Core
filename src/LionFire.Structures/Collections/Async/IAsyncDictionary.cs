using LionFire.Structures;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace LionFire.Data.Collections;

public interface IAsyncDictionary<TKey, TValue> : IAsyncCollection<KeyValuePair<TKey, TValue>>, IEnumerableAsync<KeyValuePair<TKey, TValue>>
{
    Task<TValue> ElementAt(TKey key);
    Task ElementAt(TKey key, TValue value);

    Task<ICollection<TKey>> Keys
    { get; }
    Task<ICollection<TValue>> Values { get; }

    Task Add(TKey key, TValue value);

    Task<bool> ContainsKey(TKey key);

    Task<bool> Remove(TKey key);

    Task<bool> TryGetValue(TKey key, /*[MaybeNullWhen(false)]*/ out TValue value);
}
