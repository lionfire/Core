using System;
using System.Collections.Concurrent;

namespace LionFire.Collections
{

    public class ConcurrentDictionaryCache<TKey, TValue> : IDictionaryCache<TKey, TValue>
        where TValue : class
    {
        private ConcurrentDictionary<TKey, TValue> dict = new ConcurrentDictionary<TKey, TValue>();

        public Func<TKey, TValue> Getter;

        public ConcurrentDictionaryCache(Func<TKey, TValue> getter)
        {
            Getter = getter;
        }

        public TValue this[TKey key]
        {
            get
            {
                if (dict.TryGetValue(key, out TValue value))
                {
                    return value;
                }
                var retrieved = Getter(key);
                if (retrieved != default(TValue))
                {
                    dict.GetOrAdd(key, _ => retrieved);
                }
                return retrieved;
            }
        }
    }
}
