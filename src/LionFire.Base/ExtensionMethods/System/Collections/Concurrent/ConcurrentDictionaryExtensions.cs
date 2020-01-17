using System;
using System.Collections.Concurrent;

namespace LionFire.ExtensionMethods
{
    public static class ConcurrentDictionaryExtensions
    {
        public static void AddOrThrow<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            var result = dict.GetOrAdd(key, value);
            if (!ReferenceEquals(key, value)) throw new ArgumentException($"Key has already been added");
        }
    }
}
