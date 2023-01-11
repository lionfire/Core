using System;
using System.Collections.Concurrent;

namespace LionFire.Collections
{
    public class ConcurrentWeakDictionaryCache<TKey, TValue> : IDictionaryCache<TKey, TValue>
        where TValue : class
    {
        private ConcurrentDictionary<TKey, WeakReference<TValue>> dict = new ConcurrentDictionary<TKey, WeakReference<TValue>>();

        /// <summary>
        /// Instantiate/Create TValue for TKey.
        /// </summary>
        public Func<TKey, TValue> Getter { get; set; } // RENAME Instantiator

        public ConcurrentWeakDictionaryCache(Func<TKey, TValue> getter = null)
        {
            Getter = getter;
        }

        private WeakReference<TValue> Get(TKey key) => Getter == null ? throw new ArgumentException($"{nameof(Getter)} not available") : new WeakReference<TValue>(Getter(key));

        public TValue this[TKey key]
        {
            get
            {
                if (dict.TryGetValue(key, out WeakReference<TValue> cached))
                {
                    if (cached.TryGetTarget(out TValue cachedTarget))
                    {
                        return cachedTarget;
                    }
                    else
                    {
                        dict.TryRemove(key, out _);
                    }
                }

                var newWR = dict.GetOrAdd(key, Get);
                if (newWR.TryGetTarget(out TValue newTarget))
                {
                    return newTarget;
                }
                throw new InvalidOperationException("Failed to get target from weak reference"); // Race condition.  Make this threadsafe, or fix here or make user code threadsafe.
            }
        }

        public TValue GetOrAdd(TKey key, Func<TValue> value)
        {
            if(dict.GetOrAdd(key, k =>
            {
                TValue val = value();
                return new WeakReference<TValue>(val);
            }).TryGetTarget(out var target))
            {
                return target;
            }
            throw new UnreachableCodeException("WeakReference forgot Target");
        }
    }
}
