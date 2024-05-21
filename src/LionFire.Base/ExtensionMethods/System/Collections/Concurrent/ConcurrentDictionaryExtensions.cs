using System;
using System.Collections.Concurrent;

namespace LionFire.ExtensionMethods;

public static class ConcurrentDictionaryExtensions
{
    public static void AddOrThrow<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        var result = dict.GetOrAdd(key, value);
        if (!ReferenceEquals(result, value)) throw new ArgumentException($"Key has already been added");
    }
    public static bool TryAdd<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        var result = dict.GetOrAdd(key, value);
        return ReferenceEquals(result, value);
    }

    public static TKey AddUnique<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, Func<TKey> keyGenerator, TValue value)
    {
        // The possibility of GUIDs colliding is low, but never zero.

        while (true) // INFINITELOOP
        {
            var key = keyGenerator();
            if (dict.TryAdd(key, value)) return key;
        }
    }

    public static (TKey key, TValue value) AddUnique<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, Func<TKey> keyGenerator, Func<TKey, TValue> valueGenerator)
    {
        // The possibility of GUIDs colliding is low, but never zero.

        while (true) // INFINITELOOP
        {
            var key = keyGenerator();
            var value = valueGenerator(key);
            if (dict.TryAdd(key, value)) return (key, value);
        }
    }
}
