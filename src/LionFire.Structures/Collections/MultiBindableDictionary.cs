using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using LionFire.Structures;

namespace LionFire.Collections
{
    //INotifyingDictionary<TKey, TValue> // Messes up JsonEx serialization with two ICollections!?

    /// <summary>
    /// A dictionary with notification support.  Events are raised that automatically dispatches events held by DispatcherObjects as needed.
    /// </summary>
    /// <remarks>
    /// ICollection&lt;TValue&gt; write methods require TValue inherit from IKeyed&lt;TKey&gt;
    /// </remarks>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class MultiBindableDictionary<TKey, TValue> : ICollection<TValue>, IEnumerable<TValue>,
        INotifyingDictionary<TKey, TValue>,
        //INotifyingReadOnlyDictionary<TKey, TValue>,
        IReadOnlyDictionary<TKey, TValue>,
        IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, // redundant?
        INotifyingCollection<TValue>,
        INotifyingReadOnlyCollection<TValue>
    , INotifyCollectionChanged
        , IDictionary
    //INotifyCollectionChanged // TODO NEXT - Replace with INotifyCollectionChanged<K,V> and update Raise method
    {
        private Dictionary<TKey, TValue> dictionary;


        public Func<TValue, TKey> GetKey;

        public Func<TKey, TValue> AutoCreate;

#if AOT
        public static TKey GetStringKeyFromKeyed(TValue val)
        {
            IROStringKeyed keyed = val as IROStringKeyed;
            if (keyed != null)
            {
                return (TKey)(object)keyed.Key;
            }
            return default(TKey);
        }
#else
        public static TKey GetKeyFromKeyed(TValue val)
        {
            var keyed = val as IKeyed<TKey>;
            if (keyed != null)
            {
                return keyed.Key;
            }
            return default(TKey);
        }
#endif
        #region Constructors

        public MultiBindableDictionary()
        {
#if !AOT
            if (typeof(IKeyed<TKey>).IsAssignableFrom(typeof(TValue)))
            {
                GetKey = GetKeyFromKeyed;
            }
#else
            if (typeof(IROStringKeyed).IsAssignableFrom(typeof(TValue)))
            {
                GetKey = GetStringKeyFromKeyed;
            }
#endif
#if AOT
            if (typeof(TKey) == typeof(int))
            {
                dictionary = new Dictionary<TKey, TValue>((IEqualityComparer<TKey>)LionFire.Singleton<LionFire.IntEqualityComparer>.Instance);
            }
            else if (typeof(TKey) == typeof(uint))
            {
                dictionary = new Dictionary<TKey, TValue>((IEqualityComparer<TKey>)LionFire.Singleton<LionFire.UIntEqualityComparer>.Instance);
            }
            else if (typeof(TKey) == typeof(short))
            {
                dictionary = new Dictionary<TKey, TValue>((IEqualityComparer<TKey>)LionFire.Singleton<LionFire.ShortEqualityComparer>.Instance);
            }
            else if (typeof(TKey).IsValueType)
            {
                throw new NotSupportedException("Only int and short supported in AOT for TKey.  Implement IEqualityComparer for your value type and add it to this class.");
            }
            else
#endif
            {
                dictionary = new Dictionary<TKey, TValue>();
            }
        }

        public MultiBindableDictionary(List<TValue> keyedValues)
            : this()
        {
            foreach (var value in keyedValues)
            {
                this.Add(value);
            }
        }

        #endregion

        //event NotifyCollectionChangedHandler<TValue> INotifyingCollection<TValue>.CollectionChanged;
#if AOT
        NotifyCollectionChangedHandler<TValue> collectionChanged; // EVENT FIXME
        public event NotifyCollectionChangedHandler<TValue> CollectionChanged
        {
            add { collectionChanged += value; }
            remove { collectionChanged -= value; }
        }
#else
		public event NotifyCollectionChangedHandler<TValue> CollectionChanged;
#endif

        #region INotifyCollectionChanged

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add
            {  //REVIEW - why did this blow up in unity?
                AddCollectionChanged(value);
                //				this.l += value; 
            }
            remove
            {
                RemoveCollectionChanged(value);
                //				this.collectionChangedNG -= value; 
            }
        }
        private void AddCollectionChanged(NotifyCollectionChangedEventHandler value)
        {
            //try
            //{
                //				if(false) 
                // l.Trace("ZZZ123 local event fail");
                CollectionChangedNG += value;
            //}
            //catch (Exception ex)
            //{
            //    Log.Warn(ex.ToString());
            //}
        }
        private void RemoveCollectionChanged(NotifyCollectionChangedEventHandler value)
        {
            CollectionChangedNG -= value;
        }
        //#if !UNITY
        //        private
        //#else // Weird, not accessible???
        //		public
        //#endif
#if AOT
        public NotifyCollectionChangedEventHandler CollectionChangedNG; // FIXME - EVENT
#else
		public event NotifyCollectionChangedEventHandler CollectionChangedNG; 
#endif

        #endregion

        //public event NotifyCollectionChangedEventHandler CollectionChanged;
        //public event NotifyCollectionChangedEventHandler ValuesChanged;

        public static bool AutoDetachThrowingHandlers = true;
        public static bool TraceDetachThrowingHandlers = true;
        // private static readonly ILogger l = Log.Get();

        public static event Action<object, Exception> CollectionChangedException;

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            try
            {
#if AOT
                var ev = this.collectionChanged;
                var argsG = new NotifyCollectionChangedEventArgs<TValue>(args);

                if (ev != null) ev(argsG);
#else
                MultiBindableEvents.RaiseCollectionChanged<TValue>(CollectionChanged, args);
#endif

                if (args.Action == NotifyCollectionChangedAction.Remove)
                {
                    MultiBindableEvents.RaiseCollectionChangedEventNonGeneric(this.CollectionChangedNG, this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
                else
                {
                    MultiBindableEvents.RaiseCollectionChangedEventNonGeneric(this.CollectionChangedNG, this, args);
                }
            }
            catch (Exception ex)
            {
                // l.Error("Ignoring exception in RaiseCollectionChanged: " + ex.ToString());
                CollectionChangedException?.Invoke(this, ex);
            }

            //// COPY See also: corresponding method in MultiBindableDictionary

            //var ev = CollectionChanged;
            //if (ev != null)
            //{
            //    var genericArgs = new NotifyCollectionChangedEventArgs<TValue>(args);

            //    foreach (NotifyCollectionChangedHandler<TValue> del in ev.GetInvocationList())
            //    {
            //        try
            //        {
            //            if (del.Target as DispatcherObject != null)
            //            {
            //                DispatcherObject dispatcherObject = (DispatcherObject)del.Target;

            //                // REVIEW - Changed to BeginInvoke.  No point in waiting for the invocation to finish! (Unless perhaps we wanted to do error handling.)
            //                if (dispatcherObject.Dispatcher != null && !dispatcherObject.Dispatcher.CheckAccess())
            //                {
            //                    dispatcherObject.Dispatcher.BeginInvoke(del, genericArgs);
            //                }
            //                else
            //                {
            //                    del.Invoke(genericArgs); // oops, this was missing
            //                }
            //            }
            //            else
            //            {
            //                del.Invoke(genericArgs);
            //            }
            //        }
            //        //catch (Exception ex)
            //        //{
            //        //    if (AutoDetachThrowingHandlers)
            //        //    {
            //        //        ev -= del;
            //        //        if (TraceDetachThrowingHandlers)
            //        //        {
            //        //            System.Diagnostics.Trace.WriteLine("MultiBindableDictionary event handler threw exception.  Detaching.  Exception: " + ex.ToString());
            //        //        }
            //        //    }
            //        //}
            //    }
            //}
        }

        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);

            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value)); // OPTIMIZE?
            //RaiseCollectionChanged(NotifyCollectionChangedAction.Add, value);
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

#if !AOT
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get { return dictionary.Keys; }
        }
#endif
        public ICollection<TKey> Keys
        {
            get { return dictionary.Keys; }
        }

        public bool Remove(TKey key)
        {
            if (dictionary.ContainsKey(key))
            {
                TValue val = dictionary[key];
                dictionary.Remove(key);
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, val));
                return true;
            }
            return false;
        }

        public TValue TryGetValue(TKey key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            return default(TValue);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                value = dictionary[key];
                return true;
            }
            value = default(TValue);
            return false;
        }
#if !AOT
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get { return dictionary.Values; }
        }
#endif
        public ICollection<TValue> Values
        {
            get { return dictionary.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (!dictionary.ContainsKey(key) && AutoCreate != null)
                {
                    dictionary.Add(key, AutoCreate(key));
                }
                return dictionary[key];
            }
            set
            {
                TValue oldVal;
                if (TryGetValue(key, out oldVal))
                {
                    if (oldVal.Equals(value))
                    {
                        return;
                    }
                    else
                    {
                        dictionary[key] = value;
                        RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldVal));
                    }
                }
                else
                {
                    dictionary[key] = value; // will throw
                    throw new UnreachableCodeException("Key not found in Dictionary.");
                }
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            if (Count > 0)
            {
                List<TValue> deletedItems = new List<TValue>(Values);
                dictionary.Clear();
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, deletedItems));
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return dictionary.ContainsKey(item.Key) && dictionary[item.Key].Equals(item.Value); // REVIEW
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
#if AOT
            throw new NotSupportedException("AOT");
#endif
            ((IDictionary<TKey, TValue>)dictionary).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get
            {
#if AOT
                throw new NotSupportedException("AOT");
#endif
                return ((IDictionary<TKey, TValue>)dictionary).IsReadOnly;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
#if AOT
            throw new NotSupportedException("AOT");
#endif
            return ((IDictionary<TKey, TValue>)dictionary).Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)dictionary).GetEnumerator();
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return dictionary.Values.GetEnumerator();
        }

        public void Add(TValue item)
        {
            if (GetKey == null) throw new Exception("GetKey function must be set");
            TKey key = GetKey(item);
            if (key == null) throw new Exception("GetKey returned null");

            if (this.ContainsKey(key)) throw new DuplicateNotAllowedException("Key derived from item is already in dictionary: " + key.ToStringSafe());

            this.Add(key, item);
        }

        public bool Remove(TValue item)
        {
            if (GetKey == null) throw new Exception("GetKey function must be set");
            TKey key = GetKey(item);
            if (key == null) throw new Exception("GetKey returned null");
            return this.Remove(key);
        }

        bool ICollection<TValue>.Contains(TValue item)
        {
            return dictionary.Values.Contains(item);
        }
        //bool IReadOnlyCollection<TValue>.Contains(TValue item)
        //{
        //    return dictionary.Values.Contains(item);
        //}

        void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex)
        {
            dictionary.Values.CopyTo(array, arrayIndex);
        }
        //void IReadOnlyCollection<TValue>.CopyTo(TValue[] array, int arrayIndex)
        //{
        //    dictionary.Values.CopyTo(array, arrayIndex);
        //}

        TValue[] INotifyingCollection<TValue>.ToArray()
        {
            return Values.ToArray();
        }
        //TValue[] IReadOnlyCollection<TValue>.ToArray()
        //{
        //    return Values.ToArray();
        //}

#if !AOT
               
        public INotifyingDictionary<BaseKey, BaseValue> Filter<BaseKey, BaseValue>()
        {
            if (!typeof(BaseKey).IsAssignableFrom(typeof(TKey)))
            {
                throw new ArgumentException("BaseKey must be assignable from TKey.");
            }

            if (!typeof(BaseValue).IsAssignableFrom(typeof(TValue)))
            {
                throw new ArgumentException("DerivedValue must be assignable from TValue.");
            }

            Type type = typeof(MultiBindableDictionaryFilter<,,,>).MakeGenericType(typeof(BaseKey), typeof(BaseValue), typeof(TKey), typeof(TValue));
            //Type type = typeof(MultiBindableCollectionFilter<,>).MakeGenericType(typeof(BaseKey), typeof(BaseValue)); // NO

            return (INotifyingDictionary<BaseKey, BaseValue>)Activator.CreateInstance(type, this);
        }
#endif
        //event NotifyCollectionChangedHandler<KeyValuePair<TKey, TValue>> INotifyCollectionChanged<KeyValuePair<TKey, TValue>>.CollectionChanged
        //{
        //    add { throw new NotImplementedException(); }
        //    remove { throw new NotImplementedException(); }
        //}

        void IDictionary.Add(object key, object value)
        {
            this.Add((TKey)key, (TValue)value);
        }

        void IDictionary.Clear()
        {
            this.Clear();
        }

        bool IDictionary.Contains(object key)
        {
            return this.ContainsKey((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return this.IsReadOnly; }
        }

        ICollection IDictionary.Keys
        {
            get { return (ICollection) this.Keys; }
        }

        void IDictionary.Remove(object key)
        {
            this.Remove((TKey)key);
        }

        ICollection IDictionary.Values
        {
            get { return (ICollection)this.Values; }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return this[(TKey)key];
            }
            set
            {
                this[(TKey)key] = (TValue)value;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            foreach (var val in this.Values)
            {
                array.SetValue(val, index);
                index++;
            }
        }

        int ICollection.Count
        {
            get { return this.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return this.syncRoot; }
        }
        private object syncRoot = new object();
    }

#if !AOT
    /// <summary>
    /// TODO: More Support for upcast failures without throwing InvalidCastExceptions
    /// TODO: Use Implicit interface implementations to avoid repetition.
    /// </summary>
    /// <typeparam name="BaseT"></typeparam>
    /// <typeparam name="DerivedT"></typeparam>
    public class MultiBindableDictionaryFilter<BaseKey, BaseValue, DerivedKey, DerivedValue> : INotifyingDictionary<BaseKey, BaseValue>, INotifyingCollection<BaseValue>
        , INotifyCollectionChanged
        where DerivedKey : BaseKey
        where DerivedValue : class, BaseValue
    {
        private INotifyingDictionary<DerivedKey, DerivedValue> target;


        public MultiBindableDictionaryFilter(INotifyingDictionary<DerivedKey, DerivedValue> target)
        {
            if (!typeof(BaseKey).IsAssignableFrom(typeof(DerivedKey)))
            {
                throw new ArgumentException("BaseKey must be assignable from DerivedKey.");
            }

            if (!typeof(BaseValue).IsAssignableFrom(typeof(DerivedValue)))
            {
                throw new ArgumentException("DerivedValue must be assignable from BaseValue.");
            }

            this.target = target;

        }

    #region Filter
#if !AOT
        INotifyingDictionary<Key, Value> INotifyingDictionary<BaseKey, BaseValue>.Filter<Key, Value>()
        {
            return target.Filter<Key, Value>();
        }
#endif
    #endregion


        public void Add(BaseKey key, BaseValue value)
        {
            target.Add((DerivedKey)key, (DerivedValue)value);
        }

        public bool ContainsKey(BaseKey key)
        {
            return ((IDictionary<BaseKey, BaseValue>)target).ContainsKey((DerivedKey)key);
        }

        public ICollection<BaseKey> Keys
        {
            get { throw new NotImplementedException(); }
        }
#if !AOT
        IEnumerable<BaseKey> IReadOnlyDictionary<BaseKey, BaseValue>.Keys
        {
            get { throw new NotImplementedException(); }
        }
#endif

        public bool Remove(BaseKey key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(BaseKey key, out BaseValue value)
        {
            throw new NotImplementedException();
        }

        public ICollection<BaseValue> Values
        {
            get { throw new NotImplementedException(); }
        }
#if !AOT
		IEnumerable<BaseValue> IReadOnlyDictionary<BaseKey, BaseValue>.Values
        {
            get { throw new NotImplementedException(); }
        }
#endif

        public BaseValue this[BaseKey key]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add(KeyValuePair<BaseKey, BaseValue> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            ((ICollection<KeyValuePair<DerivedKey, DerivedValue>>)target).Clear();
        }

        public bool Contains(KeyValuePair<BaseKey, BaseValue> item)
        {
            return target.Contains(new KeyValuePair<DerivedKey, DerivedValue>((DerivedKey)item.Key, (DerivedValue)item.Value));
        }

        public void CopyTo(KeyValuePair<BaseKey, BaseValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return ((ICollection<KeyValuePair<DerivedKey, DerivedValue>>)target).Count; }
        }

        public bool IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<DerivedKey, DerivedValue>>)target).IsReadOnly; }
        }

        public bool Remove(KeyValuePair<BaseKey, BaseValue> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<BaseKey, BaseValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public BaseValue[] ToArray()
        {
            return target.ToArray().Cast<BaseValue>().ToArray();
        }

        public void Add(BaseValue item)
        {
            throw new NotImplementedException();
            //target.Add((DerivedValue)item);
        }

        public bool Contains(BaseValue item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(BaseValue[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(BaseValue item)
        {
            return target.Remove((DerivedValue)item);
        }

        IEnumerator<BaseValue> IEnumerable<BaseValue>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public event NotifyCollectionChangedHandler<BaseValue> CollectionChanged
        {
            add { }
            remove { }// UNUSED
        }

        //#region INotifyCollectionChanged

        //event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        //{
        //    add { ((INotifyCollectionChanged)target).CollectionChanged += value; }
        //    remove { ((INotifyCollectionChanged)target).CollectionChanged -= value; }
        //}

        //#endregion

    #region INotifyCollectionChanged<BaseValue>

#if __MonoCS__ || UNITY
#warning FIXME - MONO bug?
#else

        event NotifyCollectionChangedHandler<BaseValue> INotifyCollectionChanged<BaseValue>.CollectionChanged
        {
            add
            {
                if (derivedCollectionChanged == null)
                {
                    ((INotifyCollectionChanged<DerivedValue>)target).CollectionChanged += new NotifyCollectionChangedHandler<DerivedValue>(target_CollectionChanged);
                }
                collectionChanged += value;
            }
            remove
            {
                collectionChanged -= value;
                if (derivedCollectionChanged == null)
                {
                    ((INotifyCollectionChanged<DerivedValue>)target).CollectionChanged -= new NotifyCollectionChangedHandler<DerivedValue>(target_CollectionChanged);
                }
            }
        } 
#endif

        event NotifyCollectionChangedHandler<BaseValue> collectionChanged;

        void target_CollectionChanged(INotifyCollectionChangedEventArgs<DerivedValue> e)
        {
            var ev = collectionChanged;
            if (ev == null) { return; }

            var newE = new NotifyCollectionChangedEventArgs<BaseValue>
            {
                Action = e.Action
            };

            if (e.OldItems != null)
            {
                newE.OldItems = e.OldItems.Cast<BaseValue>().ToArray();
            }
            if (e.NewItems != null)
            {
                newE.NewItems = e.NewItems.Cast<BaseValue>().ToArray();
            }

            ev(newE);
        }

        event NotifyCollectionChangedHandler<DerivedValue> derivedCollectionChanged;

    #endregion

    #region INotifyCollectionChanged

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                throw new NotImplementedException("INotifyCollectionChanged");
                //l.Warn("Not implemented: INotifyCollectionChanged");
                //if (derivedCollectionChangedNG == null)
                //{
                //    target.CollectionChanged += new NotifyCollectionChangedEventHandler(target_CollectionChanged);
                //}
                //collectionChangedNG += value;
            }
            remove
            {
                throw new NotImplementedException("INotifyCollectionChanged");
                //l.Warn("Not implemented: INotifyCollectionChanged");
                //collectionChangedNG -= value;
                //if (derivedCollectionChanged == null)
                //{
                //    target.CollectionChanged -= new NotifyCollectionChangedEventHandler(target_CollectionChanged);
                //}
            }
        }
        //event NotifyCollectionChangedEventHandler collectionChangedNG;


        ////void target_CollectionChanged(NotifyCollectionChangedEventArgs<DerivedValue> e)
        ////{
        ////    var ev = collectionChangedNG;
        ////    if (ev == null) { return; }

        ////    var newE = new NotifyCollectionChangedEventArgs<BaseValue>();
        ////    newE.Action = e.Action;

        ////    if (e.OldItems != null)
        ////    {
        ////        newE.OldItems = e.OldItems.Cast<BaseValue>().ToArray();
        ////    }
        ////    if (e.NewItems != null)
        ////    {
        ////        newE.NewItems = e.NewItems.Cast<BaseValue>().ToArray();
        ////    }

        ////    ev(newE);
        ////}

        ////event NotifyCollectionChangedEventHandler derivedCollectionChangedNG;

    #endregion

    }
#endif

}
