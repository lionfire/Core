using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Structures;

namespace LionFire.ExtensionMethods
{
    public static class IKeyedDictionaryExtensions
    {
        public static IDictionary<TKey, TValue> ToDictionary<TKey,TValue>(this IEnumerable<TValue> values) 
            where TValue : IKeyed<TKey>
        {
            var dict = new Dictionary<TKey, TValue>();
            foreach (var val in values)
            {
                dict.Add(val.Key, val);
            }
            return dict;
        }
    }
}
