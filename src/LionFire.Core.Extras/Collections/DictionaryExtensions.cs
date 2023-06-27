using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Collections;
using LionFire.Structures;
#if AOT
using System.Collections;
#endif
using System.Collections.Concurrent;

    // MOVE to LionFire.Base Collections/DictiionaryExtensions.cs
namespace LionFire 
{

    public static class DictionaryExtensions
    {

        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue val)
        {
            if (dict.ContainsKey(key)) return false;
            dict.Add(key, val);
            return true;
        }

        //public static void Add(this IDictionary<int, Protocol> dictionary, Protocol protocol)
        //{
        //    dictionary.Add(protocol.Id, protocol);
        //}

#if !AOT
#if !NET45
        /// <summary>
        /// Returns a read-only interface.  Does not prevent explicit casting back to a read-write dictionary.
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            // TODO: Duck typing
            return (IReadOnlyDictionary<TKey, TValue>)dictionary;
        }
#endif
#endif

        //public static IReadOnlyDictionary<TKey, TValue> ToReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        //{
        //    // TODO: Wrap dictionary in read-only wrapper
        //}


#if !AOT
        public static ValueType TryGetValueRO<KeyType, ValueType>(this IReadOnlyDictionary<KeyType, ValueType> dictionary,
            KeyType key)
            where ValueType : class
        {
            if (dictionary.ContainsKey(key)) return dictionary[key];
            return null;
        }
#endif

        public static bool TryGetValue<KeyType, ValueType>(this IDictionary<KeyType, ValueType> dictionary,
            KeyType key, out ValueType value)
        // where ValueType : class
        {
            if (dictionary.ContainsKey(key))
            {
                value = dictionary[key];
                return true;
            }
            else
            {
                value = default(ValueType);
                return false;
            }

        }

#if !AOT
        public static ValueType GetOrAddNew<KeyType, ValueType>(this IDictionary<KeyType, ValueType> dictionary,
            KeyType key)
            where ValueType : class, new()
        {
            if (dictionary.ContainsKey(key)) return dictionary[key];
            ValueType val = new ValueType();

            // TODO - eliminate this, have it only in Keyed version
            if (val is IKeyable<KeyType> keyed) { keyed.Key = key; }

            dictionary.Add(key, val);
            return val;
        }

        /// <summary>
        /// Injects key to new objects of type IKeyed&lt;KeyType&gt;
        /// </summary>
        /// <typeparam name="KeyType"></typeparam>
        /// <typeparam name="ValueType"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ValueType GetOrAddNewKeyed<KeyType, ValueType>(this IDictionary<KeyType, ValueType> dictionary,
            KeyType key)
            where ValueType : class, new()
        {
            if (dictionary.ContainsKey(key)) return dictionary[key];
            ValueType val = new ValueType();

            if (val is IKeyable<KeyType> keyed) { keyed.Key = key; }

            dictionary.Add(key, val);
            return val;
        }
#endif

#if !AOT
        [Obsolete("See LionFire.Base's GetOrAdd")]
        public static ValueType GetOrAddDefault<KeyType, ValueType>(this IDictionary<KeyType, ValueType> dictionary,
            KeyType key, Func<ValueType> defaultValue)
        {
            if (dictionary.ContainsKey(key)) return dictionary[key];
            ValueType val = defaultValue.Invoke();

            if (val is IKeyable<KeyType> keyed) { keyed.Key = key; }

            dictionary.Add(key, val);
            return val;
        }
#else
        public static object GetOrAddDefault(this IDictionary dictionary,
            object key, Func<object> defaultValue)
        {
            if (dictionary.Contains(key)) return dictionary[key];
            object val = defaultValue.Invoke();

            //var keyed = val as IKeyed<KeyType>;
            //if (keyed != null) { keyed.Key = key; }

            dictionary.Add(key, val);
            return val;
        }
#endif
#if NET35
		public static ValueType GetOrAddDefault<KeyType, ValueType>(this ConcurrentDictionary<KeyType, ValueType> dictionary,
		                                                            KeyType key, Func< ValueType> defaultValue)
            // OLD: Func<KeyType, ValueType>
		{
			return dictionary.GetOrAdd(key, defaultValue);
		}
#endif

#if !AOT
        public static void Set<KeyType, ValueType>(this IDictionary<KeyType, ValueType> dictionary,
            KeyType key, ValueType value)
        {
            if (dictionary.ContainsKey(key)) dictionary[key] = value;
            else dictionary.Add(key, value);
        }
#else
		public static void Set(this System.Collections.IDictionary dictionary,
		                                           object key, object value)
		{
			if (dictionary.Contains(key)) dictionary[key] = value;
			else dictionary.Add(key, value);
		}
        // FUTURE: Consider a remove if value null option for Set, or a separate SetOrRemove method
#endif

#if !AOT
        public static void AddRange<KeyType, ValueType>(this IDictionary<KeyType, ValueType> dict, IEnumerable<ValueType> items)
            where ValueType :
#if AOT
				IROStringKeyed
#else
 IKeyed<KeyType>
#endif
        {
            foreach (var item in items)
            {
                dict.Add(item.Key, item);
            }
        }
#endif

#if !AOT
        public static void Add<KeyType, ValueType>(this IDictionary<KeyType, ValueType> dict, ValueType val)
            where ValueType : IKeyed<KeyType>
        {
            dict.Add(val.Key, val);
        }
#else
		public static void Add(this IDictionary dict, object val)
		{
			IROKeyed keyed = val as IROKeyed;
			if(keyed == null) throw new ArgumentException("val must implement IROKeyed");

			dict.Add(keyed.Key, val);
		}
#endif
    }

#if UNITY
#if !AOT
	public static class DictEx2<KeyType,ValueType>
		where ValueType : class, new()
	{
		public static ValueType GetOrAddNew(IDictionary<KeyType, ValueType> dictionary,
		                                    KeyType key)
		{
			if (dictionary.ContainsKey(key)) return dictionary[key];
			ValueType val = new ValueType();
			
			var keyed = val as IKeyed<KeyType>;
			if (keyed != null) { keyed.Key = key; }
			
			dictionary.Add(key, val);
			return val;
		}
	}
	public static class DictEx<KeyType,ValueType>
			where ValueType : class //, new()
	{

		public static ValueType GetOrAddDefault( IDictionary<KeyType, ValueType> dictionary,
		                                                            KeyType key, Func<ValueType> defaultValue)
		{
			if (dictionary.ContainsKey(key)) return dictionary[key];
			ValueType val = defaultValue.Invoke();
			
			var keyed = defaultValue as IKeyed<KeyType>;
			if (keyed != null) { keyed.Key = key; }
			
			dictionary.Add(key, val);
			return val;
		}
		
#if NET35
		public static ValueType GetOrAddDefault( ConcurrentDictionary<KeyType, ValueType> dictionary,
		                                                            KeyType key, Func<ValueType> defaultValue)
		{
			return dictionary.GetOrAdd(key, defaultValue); 
		}
#endif
	}
#endif
#endif

}
