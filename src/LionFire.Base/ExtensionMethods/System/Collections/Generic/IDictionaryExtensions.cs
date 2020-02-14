
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.ExtensionMethods
{
    // For support of no class constraints, consider http://jonskeet.uk/csharp/miscutil/usage/genericoperators.html

    public static class IDictionaryExtensions
    {
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue val)
        {
            if (dict.ContainsKey(key)) return false;
            dict.Add(key, val);
            return true;
        }

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
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
            where TValue : class, new()
        {
            var result = dict.TryGetValue(key);
            if (result == null)
            {
                result = new TValue();
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

        /// <summary>
        /// Sets me to match other, by checking keys
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="me"></param>
        /// <param name="other"></param>
        public static void SetToMatch<TKey, TValue>(this IDictionary<TKey, TValue> me, IDictionary<TKey, TValue> other)
        {
            var meArr = me.Keys.ToList();

            foreach (var key in other.Keys)
            {
                if (!me.ContainsKey(key))
                {
                    me.Add(key, other[key]);
                }
                meArr.Remove(key);
            }
            foreach (var item in meArr)
            {
                me.Remove(item);
            }
        }

        /// <summary>
        /// Sets me to match other, by checking keys
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="me"></param>
        /// <param name="other"></param>
        public static void SetToMatch<TKey, TValue>(this IDictionary<TKey, TValue> me, IEnumerable<TKey> other, Func<TKey, TValue> valueProvider, Action<TKey, TValue> onRemove = null)
        {
            var meArr = me.Keys.ToList();

            foreach (var key in other)
            {
                if (!me.ContainsKey(key))
                {
                    me.Add(key, valueProvider(key));
                }
                meArr.Remove(key);
            }
            foreach (var item in meArr)
            {
                onRemove?.Invoke(item, me[item]);
                me.Remove(item);
            }
        }
    }
}
