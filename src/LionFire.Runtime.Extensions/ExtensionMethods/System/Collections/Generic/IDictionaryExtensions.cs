using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods
{
    // For support of no class constraints, consider http://jonskeet.uk/csharp/miscutil/usage/genericoperators.html

    public static class IDictionaryExtensions
    {
        /// <summary>
        /// Returns default(TValue) if key is null
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            if (key == null) return default(TValue);
            if (!dict.ContainsKey(key)) return defaultValue;
            return dict[key];
        }
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> factory)
            where TValue : class
        {
            var result = dict.TryGetValue(key);
            if (result == null)
            {
                result = factory(key);
                dict.Add(key, result);
            }
            return result;
        }
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
            where TValue : class
        {
            var result = dict.TryGetValue(key);
            if (result == null)
            {
                dict.Add(key, value);
            }
            else
            {
                dict[key] = value;
            }
        }
    }
}
