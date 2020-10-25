using System.Collections.Generic;
using System.Collections;

namespace LionFire // RENAME LionFire.ExtensionMethods
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
