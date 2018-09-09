using System;
using System.Collections.Concurrent;

namespace LionFire.Collections
{
    public class ConcurrentDictionaryCache<TKey, TValue>
    {
        private ConcurrentDictionary<TKey, TValue> dict = new ConcurrentDictionary<TKey, TValue>();

        public Func<TKey, TValue> Getter;

        public ConcurrentDictionaryCache(Func<TKey, TValue> getter)
        {
            Getter = getter;
        }

        public TValue this[TKey key] => dict.GetOrAdd(key, Getter);
    }
}
