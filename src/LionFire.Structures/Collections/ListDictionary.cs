using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using LionFire.Structures;

namespace LionFire.Instantiating
{
    //public class ListDictionarySerializationConverter : IJsonTypeConverter
    //{
    //    public object Context
    //    {
    //        set { throw new NotImplementedException(); }
    //    }

    //    public object ConvertFrom(object item, JsonExSerializer.SerializationContext serializationContext)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public object ConvertTo(object item, Type sourceType, JsonExSerializer.SerializationContext serializationContext)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Type GetSerializedType(Type sourceType)
    //    {
    //        return typeof(ArrayList);
    //    }

    //    public bool SupportsReferences(Type sourceType, JsonExSerializer.SerializationContext serializationContext)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    // FUTURE: INotiry
    //[JsonExSerializer.JsonConvert(typeof(ListDictionarySerializationConverter))]
    public class ListDictionary<TKey, TValue> : IEnumerable<TValue>
    #region FUTURE: Notification support?
        //, INotifyCollectionChanged<TValue> 
        //, INotifyingDictionary<TKey, TValue>
    #endregion
        where TValue :
#if AOT
 IROStringKeyed
#else
			IKeyed<TKey>
#endif
        where TKey : class
    {

        /// <summary>
        /// For serialization purposes
        /// </summary>
        [SerializeDefaultValue(false)]
        public List<TValue> AllItems
        {
            get
            {
                IEnumerable<TValue> e = this;
                if (!e.Any()) return null;
                return e.ToList();
            }
            set
            {
                Clear();
                if (value != null)
                {
                    foreach (TValue item in value)
                    {
                        Add(item);
                    }
                }
            }
        }

        public void Clear()
        {
            this.UnkeyedItems.Clear();
            this.KeyedItems.Clear();
        }

        #region List

        public List<TValue> UnkeyedItems
        {
            get { if (list == null) { list = new List<TValue>(); } return list; }
        } private List<TValue> list;

        #endregion

        #region Dictionary

        public Dictionary<TKey, TValue> KeyedItems
        {
            get { if (dictionary == null) { dictionary = new Dictionary<TKey, TValue>(); } return dictionary; }
        } private Dictionary<TKey, TValue> dictionary;

        #endregion

        #region IEnumerable

        public IEnumerator<TValue> GetEnumerator()
        {
            if (dictionary != null)
            {
                foreach (var item in dictionary)
                {
                    yield return item.Value;
                }
            }
            if (list != null)
            {
                foreach (var listItem in list)
                {
                    yield return listItem;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            IEnumerable<TValue> e = this;
            foreach (TValue item in e)
            {
                yield return item;
            }
        }

        #endregion

        #region (Public) Add/Remove Methods

        public void Add(TValue keyed)
        {
#if AOT
            TKey key = (TKey)(object)keyed.Key;
#else
			TKey key = keyed.Key;
#endif

            if (key == default(TKey)) key = TryGenerateKey(keyed);

            if (key == default(TKey))
            {
                UnkeyedItems.Add(keyed);
            }
            else
            {
#if AOT
                KeyedItems.Add((TKey)(object)keyed.Key, keyed); // Throws on key collision
#else
				KeyedItems.Add(keyed.Key, keyed); // Throws on key collision
#endif
            }
            OnAdded(keyed);
        }

        protected virtual void OnAdded(TValue keyed)
        {
        }
        protected virtual void OnRemoved(TValue keyed)
        {
        }


        public bool Remove(TValue keyed)
        {
#if AOT
            TKey key = (TKey)(object)keyed.Key;
#else
			TKey key = keyed.Key;
#endif

            if (key == default(TKey)) key = TryGenerateKey(keyed);

            if (key == default(TKey))
            {
                TValue val = UnkeyedItems.Find(x => x.Equals(keyed));

                bool result = UnkeyedItems.Remove(keyed);

                if (val != null) OnRemoved(val);

                return result;
            }
            else
            {
                return Remove(
#if AOT
(TKey)(object)
#endif

keyed.Key);
            }
        }

        public bool Remove(TKey key)
        {
            TValue val;
            bool gotVal;
            gotVal = KeyedItems.TryGetValue(key, out val);

            bool result = KeyedItems.Remove(key);

            if (gotVal) { OnRemoved(val); }

            return result;
        }

        #endregion

        #region Collection Support

        public int Count
        {
            get
            {
                int count = 0;
                if (list != null) count += list.Count;
                if (dictionary != null) count += dictionary.Count;
                return count;
            }
        }

            // Remove this?  (if so, first rename to more easily refactor)
        public IEnumerable<TValue> Values => this;

        public bool ContainsKey(TKey key) { return dictionary != null && dictionary.ContainsKey(key); }

        public void AddRange(IEnumerable<TValue> items)
        {
            foreach (TValue item in (IEnumerable)items)
            {
                Add(item);
            }
        }
        public void AddRange(params TValue[] items)
        {
            foreach (TValue item in (IEnumerable)items)
            {
                Add(item);
            }
        }

        #endregion

        #region Key Generation

        protected virtual TKey TryGenerateKey(TValue val)
        {
            return default(TKey);
        }

        #endregion

    }
}
