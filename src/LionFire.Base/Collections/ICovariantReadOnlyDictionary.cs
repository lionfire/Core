using System.Collections.Generic;
using System.Collections;
using LionFire.Structures;

namespace LionFire.Collections
{
    public interface ICovariantReadOnlyDictionary<TKey, out TValue> : IReadOnlyCollection<IKeyValuePair<TKey, TValue>>, IEnumerable<IKeyValuePair<TKey, TValue>>, System.Collections.IEnumerable, ICollection, IDictionary
	{
        new IEnumerable<TKey> Keys { get; }
        new IEnumerable<TValue> Values { get; }

        TValue this[TKey key] { get; }

        bool ContainsKey(TKey key);

        TValue TryGetValue(TKey key);
    }
}
