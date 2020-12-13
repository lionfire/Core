#if OLD // Dupe of System.Collections.Generic.IReadOnlyDictionary
using System.Collections.Generic;
#if AOT
using System.Collections;
#endif

namespace LionFire.Collections
{
    public interface IReadOnlyDictionary2<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        TValue this[TKey key] { get; }
        IEnumerable<TKey> Keys { get; }
        IEnumerable<TValue> Values { get; }


        bool ContainsKey(TKey key);

        bool TryGetValue(TKey key, out TValue value);
    }

}
#endif