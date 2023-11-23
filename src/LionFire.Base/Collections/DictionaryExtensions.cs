﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire
{
    public static class DictionaryExtensions
    {
        public static void SetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
#if !AOT
        public static ValueType? TryGetValue<KeyType, ValueType>(this IDictionary<KeyType, ValueType> dictionary,
            KeyType key)
            where ValueType : class
        {
            if (key == null) return null;
            if (dictionary.ContainsKey(key)) return dictionary[key];
            return null;
        }

#else
        public static ValueType TryGetValue<KeyType, ValueType>(this Dictionary<KeyType, ValueType> dictionary,
            KeyType key)
            where ValueType : class
        {
            if (dictionary.ContainsKey(key)) return dictionary[key];
            return null;
        }
		public static object TryGetValue(this IDictionary dictionary, object key)
		{
			if (dictionary.Contains(key)) return dictionary[key];
			return null;
		}
#endif
    }
}
