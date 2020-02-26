using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using LionFire.Structures;
//using SystemIReadOnlyDictionary = System.Collections.Generic.IReadOnlyDictionary<, >;

namespace LionFire.Collections
{

    //INotifyingDictionary<TKey, TValue> // Messes up JsonEx serialization with two ICollections!?

    /// <summary>
    /// A ConcurrentDictionary with notification support.  Events are raised that automatically dispatches events held by DispatcherObjects as needed.
    /// </summary>
    /// <remarks>
    /// ICollection&lt;TValue&gt; write methods require TValue inherit from IKeyed&lt;TKey&gt;
    /// </remarks>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class MultiBindableConcurrentDictionary<TKey, TValue> : ICollection<TValue>, IEnumerable<TValue>,
        INotifyingDictionary<TKey, TValue>,
        IReadOnlyDictionary<TKey, TValue>,
        IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, // redundant?
        INotifyingCollection<TValue>,
        INotifyingReadOnlyCollection<TValue>,
        INotifyCollectionChanged
    //INotifyCollectionChanged // TODO NEXT - Replace with INotifyCollectionChanged<K,V> and update Raise method
    {
        private ConcurrentDictionary<TKey, TValue> dictionary;

        public Func<TValue, TKey> GetKey;

        public Func<TKey, TValue> AutoCreate;

#if !AOT
        public static TKey GetKeyFromKeyed(TValue val) => val is IKeyed<TKey> keyed ? keyed.Key : default;
#else
        
        public static TKey GetKeyFromStringKeyed(TValue val)
        {
            IROStringKeyed keyed = val as IROStringKeyed;
            if (keyed != null)
            {
                return (TKey)(object)keyed.Key ; // DOUBLECAST
            }
            return default(TKey);
        }
#endif
        #region Constructors

        private void ctor(IEqualityComparer<TKey> equalityComparer = null)
        {
            if (equalityComparer == null)
            {
                dictionary = new ConcurrentDictionary<TKey, TValue>();
            }
            else
            {
                dictionary = new ConcurrentDictionary<TKey, TValue>(equalityComparer);
            }

#if !AOT
            if (typeof(IKeyed<TKey>).IsAssignableFrom(typeof(TValue)))
            {
                GetKey = GetKeyFromKeyed;
            }
#else
            if(typeof(IROStringKeyed).IsAssignableFrom(typeof(TValue)) && typeof(TKey) == typeof(string))
            {
                GetKey = GetKeyFromStringKeyed;
            }
#endif
        }

        public MultiBindableConcurrentDictionary()
        {
            ctor();
        }
        public MultiBindableConcurrentDictionary(IEqualityComparer<TKey> equalityComparer)
        {
            ctor(equalityComparer);
        }

        public MultiBindableConcurrentDictionary(List<TValue> keyedValues)
        {
            ctor();
            foreach (var value in keyedValues)
            {
                this.Add(value);
            }
        }

        #endregion

        //event NotifyCollectionChangedHandler<TValue> INotifyingCollection<TValue>.CollectionChanged;
        public event NotifyCollectionChangedHandler<TValue> CollectionChanged;
        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { collectionChangedNonGeneric += value; }
            remove { collectionChangedNonGeneric -= value; }
        }

        private event NotifyCollectionChangedEventHandler collectionChangedNonGeneric;

        //event NotifyCollectionChangedHandler INotifyCollectionChanged.CollectionChanged;
        //event NotifyCollectionChangedHandler<KeyValuePair<TKey, TValue>> INotifyCollectionChanged<KeyValuePair<TKey, TValue>>.CollectionChanged
        //{
        //    add { notifyCollectionChanged += value; }
        //    remove { notifyCollectionChanged -= value; }
        //}
        //private event NotifyCollectionChangedHandler<KeyValuePair<TKey, TValue>> notifyCollectionChanged;

        //public event NotifyCollectionChangedEventHandler CollectionChanged;
        //public event NotifyCollectionChangedEventHandler ValuesChanged;

        public static bool AutoDetachThrowingHandlers = true;
        public static bool TraceDetachThrowingHandlers = true;
        private static ILogger l = Log.Get();

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            try
            {
                //				l.Warn("BWJI - Pre MultiBindableEvents.RaiseCollectionChanged<TValue>");
                MultiBindableEvents.RaiseCollectionChanged<TValue>(this.CollectionChanged, args); // GENERICMETHOD - works in AOT
                                                                                                  //				l.Warn("BWJI - Post MultiBindableEvents.RaiseCollectionChanged<TValue>");
                MultiBindableEvents.RaiseCollectionChangedEventNonGeneric(this.collectionChangedNonGeneric, this, args);
            }
            catch (Exception ex)
            {
                l.Error("Ignoring exception in RaiseCollectionChanged: " + ex.ToString());
            }
            //// COPY See also: corresponding method in MultiBindableConcurrentDictionary

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
            //        //            System.Diagnostics.Trace.WriteLine("MultiBindableConcurrentDictionary event handler threw exception.  Detaching.  Exception: " + ex.ToString());
            //        //        }
            //        //    }
            //        //}
            //    }
            //}
        }

        public bool TryAdd(TKey key, TValue value)
        {
            bool result = dictionary.TryAdd(key, value);
            if (result)
            {
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value)); // OPTIMIZE?
            }
            return result;
        }

        public void Add(TKey key, TValue value)
        {
            //dictionary.AddOrUpdate(key, value, (k,v)=> value);
            if (!dictionary.TryAdd(key, value))
            {
                throw new DuplicateNotAllowedException("Dictionary already contains key");
            }
            //dictionary.Add(key, value);

            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value)); // OPTIMIZE?
            //RaiseCollectionChanged(NotifyCollectionChangedAction.Add, value);
        }

        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

        public ICollection<TKey> Keys => dictionary.Keys;

#if !AOT
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => dictionary.Keys;
        IEnumerable<TKey> System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.Keys => dictionary.Keys;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => dictionary.Keys;
#endif
        public bool Remove(TKey key)
        {
            bool result = dictionary.TryRemove(key, out TValue val);
            if (result) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, val)); }
            return result;
        }

        public TValue TryGetValue(TKey key)
        {
            if (dictionary.TryGetValue(key, out TValue val))
            {
                return val;
            }
            else
            {
                return default;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values => dictionary.Values;
#if !AOT
        IEnumerable<TValue> System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.Values => dictionary.Values;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => dictionary.Values;
#endif

        public TValue this[TKey key]
        {
            // REVIEW: Best approach for concurrent
            get
            {
                if (!dictionary.ContainsKey(key) && AutoCreate != null)
                {
                    dictionary.TryAdd(key, AutoCreate(key));
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
                // REVIEW: Send Reset instead?
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
#else
            ((IDictionary<TKey, TValue>)dictionary).CopyTo(array, arrayIndex);
#endif
        }

        public int Count => dictionary.Count;

        public bool IsReadOnly
        {
            get
            {
#if AOT
                throw new NotSupportedException("AOT");
#else
                return ((IDictionary<TKey, TValue>)dictionary).IsReadOnly;
#endif
            }
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count => throw new NotImplementedException();

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => throw new NotImplementedException();

        TValue System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.this[TKey key] => throw new NotImplementedException();

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
#if AOT
            throw new NotSupportedException("AOT");
#else
            return ((IDictionary<TKey, TValue>)dictionary).Remove(item.Key);
#endif
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
#if AOT
            throw new NotSupportedException("AOT");
#else
            return dictionary.Values.GetEnumerator();
#endif
        }

        public void Add(TValue item)
        {
            if (GetKey == null) throw new Exception("GetKey function must be set");
            TKey key = GetKey(item);
            if (key == null) throw new Exception("GetKey returned null");
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
#if AOT
            throw new NotSupportedException("AOT");
#else
            return dictionary.Values.Contains(item);
#endif
        }
//        bool IReadOnlyCollection<TValue>.Contains(TValue item)
//        {
//#if AOT
//            throw new NotSupportedException("AOT");
//#else
//            return dictionary.Values.Contains(item);
//#endif
//        }

        void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex)
        {
#if AOT
            throw new NotSupportedException("AOT");
#else
            dictionary.Values.CopyTo(array, arrayIndex);
#endif
        }
//        void System.Collections.Generic.IReadOnlyCollection<TValue>.CopyTo(TValue[] array, int arrayIndex)
//        {
//#if AOT
//            throw new NotSupportedException("AOT");
//#else
//            dictionary.Values.CopyTo(array, arrayIndex);
//#endif
//        }

        TValue[] INotifyingCollection<TValue>.ToArray() => Values.ToArray();
        //TValue[] System.Collections.Generic.IReadOnlyCollection<TValue>.ToArray() => Values.ToArray();

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

            Type type = typeof(MultiBindableCollectionFilter<,>).MakeGenericType(typeof(BaseKey), typeof(BaseValue), typeof(TKey), typeof(TValue));

            return (INotifyingDictionary<BaseKey, BaseValue>)Activator.CreateInstance(type, this);
        }
#endif
        public TValue AddOrUpdate(TKey key, TValue val, Func<TKey, TValue, TValue> updateValueFactory)
        {
            return dictionary.AddOrUpdate(key, val, updateValueFactory);
        }
        public TValue AddOrUpdate(TKey key, TValue val)
        {

            return dictionary.AddOrUpdate(key, val, (k, v) => val);
        }

        INotifyingDictionary<FilterKey, FilterValue> INotifyingDictionary<TKey, TValue>.Filter<FilterKey, FilterValue>() => throw new NotImplementedException();
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => throw new NotImplementedException();
        bool IDictionary<TKey, TValue>.ContainsKey(TKey key) => throw new NotImplementedException();
        bool IDictionary<TKey, TValue>.Remove(TKey key) => throw new NotImplementedException();
        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) => throw new NotImplementedException();
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException();
        void ICollection<KeyValuePair<TKey, TValue>>.Clear() => throw new NotImplementedException();
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException();
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => throw new NotImplementedException();
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException();
        bool System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key) => throw new NotImplementedException();
        bool System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) => throw new NotImplementedException();
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => throw new NotImplementedException();
        void ICollection<TValue>.Add(TValue item) => throw new NotImplementedException();
        void ICollection<TValue>.Clear() => throw new NotImplementedException();
        bool ICollection<TValue>.Remove(TValue item) => throw new NotImplementedException();
    }

#if !AOT

    /// <summary>
    /// TODO: More Support for upcast failures without throwing InvalidCastExceptions
    /// TODO: Use Implicit interface implementations to avoid repetition.
    /// </summary>
    /// <typeparam name="BaseT"></typeparam>
    /// <typeparam name="DerivedT"></typeparam>
    public class MultiBindableConcurrentDictionaryFilter<BaseKey, BaseValue, DerivedKey, DerivedValue> : INotifyingDictionary<BaseKey, BaseValue>, INotifyingCollection<BaseValue>
        , INotifyCollectionChanged
        where DerivedKey : BaseKey
        where DerivedValue : class, BaseValue
    {
        private INotifyingDictionary<DerivedKey, DerivedValue> target;

        public MultiBindableConcurrentDictionaryFilter(INotifyingDictionary<DerivedKey, DerivedValue> target)
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
        IEnumerable<BaseKey> System.Collections.Generic.IReadOnlyDictionary<BaseKey, BaseValue>.Keys
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

        public ICollection<BaseValue> Values => throw new NotImplementedException();
#if !AOT
        IEnumerable<BaseValue> System.Collections.Generic.IReadOnlyDictionary<BaseKey, BaseValue>.Values => throw new NotImplementedException();
#endif

        public BaseValue this[BaseKey key]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public void Add(KeyValuePair<BaseKey, BaseValue> item) => throw new NotImplementedException();

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<BaseKey, BaseValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<BaseKey, BaseValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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

        #region INotifyCollectionChanged

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                l.Warn("INotifyCollectionChanged not implemented");
                ((INotifyCollectionChanged)target).CollectionChanged += value;
            }
            remove
            {
                l.Warn("INotifyCollectionChanged not implemented");
                ((INotifyCollectionChanged)target).CollectionChanged -= value;
            }
        }

        #endregion

        private static ILogger l = Log.Get();

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

            var newE = new NotifyCollectionChangedEventArgs<BaseValue>();
            newE.Action = e.Action;

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
    }
#endif
}
