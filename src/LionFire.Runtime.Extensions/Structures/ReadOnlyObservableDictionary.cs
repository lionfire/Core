//TODO
//  - once done, use in FsObjectCollection

/* Copyright (c) 2007, Dr. WPF
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *   * Redistributions of source code must retain the above copyright
 *     notice, this list of conditions and the following disclaimer.
 * 
 *   * Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 * 
 *   * The name Dr. WPF may not be used to endorse or promote products
 *     derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY Dr. WPF ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL Dr. WPF BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

// Modifications from original:
//  - Removed Serialization-related code
//  - Modifications for AOT (?)
//  - Started ReadOnlyObservableDictionary based on ObservableDictionary

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace LionFire.Collections
{
    public interface IBindableReadOnlyDictionary<TKey, TValue>
    {
        event Action<TKey, TValue> Removed;
        event Action<TKey, TValue> Added;
    }
    // REVIEW - is there a way to make TValue covariant?
    public interface IReadOnlyObservableDictionary<TKey, TValue> :
        IBindableReadOnlyDictionary<TKey, TValue>,
        IReadOnlyDictionary<TKey, TValue>,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>,
        //IDictionary,
        //ICollection,
        IEnumerable,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
    }
    public class ReadOnlyObservableDictionary<TKey, TValue> :
        IReadOnlyObservableDictionary<TKey, TValue>
    {
        public event Action<TKey, TValue> Removed
        {
            add { UnderlyingDictionary.Removed += value; }
            remove { UnderlyingDictionary.Removed -= value; }
        }
        public event Action<TKey, TValue> Added
        {
            add { UnderlyingDictionary.Added += value; }
            remove { UnderlyingDictionary.Added -= value; }
        }

        #region constructors

        #region public

        public ReadOnlyObservableDictionary(ObservableDictionary<TKey, TValue> dictionary)
        {
            UnderlyingDictionary = dictionary;
        }

        #endregion public

        #endregion constructors

        #region properties

        #region public

        public IEqualityComparer<TKey> Comparer
        {
            get { return UnderlyingDictionary.Comparer; }
        }

        public int Count
        {
            get { return UnderlyingDictionary.Count; }
        }

        public IEnumerable<TKey> Keys
        {
            get { return UnderlyingDictionary.Keys; }
        }

        public TValue this[TKey key]
        {
            get { return UnderlyingDictionary[key]; }
        }

        public IEnumerable<TValue> Values
        {
            get { return UnderlyingDictionary.Values; }
        }

        #endregion public

        #region private

        private ObservableDictionary<TKey, TValue> UnderlyingDictionary
        {
            get; set;
        }

        #endregion private

        #endregion properties

        #region methods

        #region public

        public bool ContainsKey(TKey key)
        {
            return UnderlyingDictionary.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return UnderlyingDictionary.ContainsValue(value);
        }

        public IEnumerator GetEnumerator()
        {
            return UnderlyingDictionary.GetEnumerator();
            //return new Enumerator<TKey, TValue>(this, false);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            bool result = UnderlyingDictionary.ContainsKey(key);
            value = result ? UnderlyingDictionary[key] : default(TValue);
            return result;
        }

        #endregion public

        #region protected

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion protected

        #endregion methods

        #region interfaces

        #region IDictionary<TKey, TValue>

        bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key)
        {
            return UnderlyingDictionary.ContainsKey(key);
        }

        bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            return TryGetValue(key, out value);
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get { return Keys; }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get { return Values; }
        }

        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key]
        {
            get { return UnderlyingDictionary[key]; }
        }

        #endregion IDictionary<TKey, TValue>

        #region IDictionary


        //IDictionaryEnumerator IDictionary.GetEnumerator()
        //{
        //    return new Enumerator<TKey, TValue>(this, true);
        //}

        //bool IDictionary.IsFixedSize
        //{
        //    get { return false; }
        //}

        //bool IDictionary.IsReadOnly
        //{
        //    get { return false; }
        //}

        //object IDictionary.this[object key]
        //{
        //    get { return UnderlyingDictionary[(TKey)key].Value; }
        //    set { DoSetEntry((TKey)key, (TValue)value); }
        //}

        //ICollection IDictionary.Keys
        //{
        //    get { return Keys; }
        //}

        //void IDictionary.Remove(object key)
        //{
        //    DoRemoveEntry((TKey)key);
        //}

        //ICollection IDictionary.Values
        //{
        //    get { return Values; }
        //}

        #endregion IDictionary

        #region IReadOnlyCollection<KeyValuePair<TKey, TValue>>

        int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count
        {
            get { return UnderlyingDictionary.Count; }
        }

        #endregion IReadOnlyCollection<KeyValuePair<TKey, TValue>>

        #region ICollection

        //void ICollection.CopyTo(Array array, int index)
        //{
        //    ((ICollection)UnderlyingDictionary).CopyTo(array, index);
        //}

        //int ICollection.Count
        //{
        //    get { return UnderlyingDictionary.Count; }
        //}

        //bool ICollection.IsSynchronized
        //{
        //    get { return ((ICollection)UnderlyingDictionary).IsSynchronized; }
        //}

        //object ICollection.SyncRoot
        //{
        //    get { return ((ICollection)UnderlyingDictionary).SyncRoot; }
        //}

        #endregion ICollection

        #region IEnumerable<KeyValuePair<TKey, TValue>>

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)UnderlyingDictionary).GetEnumerator();
        }

        #endregion IEnumerable<KeyValuePair<TKey, TValue>>

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable

        #region INotifyCollectionChanged

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { UnderlyingDictionary.CollectionChanged += value; }
            remove { UnderlyingDictionary.CollectionChanged -= value; }
        }

        #endregion INotifyCollectionChanged

        #region INotifyPropertyChanged

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }

        protected virtual event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged

        #endregion interfaces

        #region protected classes

        #region KeyedDictionaryEntryCollection<TKey>

        //protected class KeyedDictionaryEntryCollection
        //    //<TKey>
        //    : KeyedCollection<TKey, DictionaryEntry>
        //{
        //    #region constructors

        //    #region public

        //    public KeyedDictionaryEntryCollection() : base() { }

        //    public KeyedDictionaryEntryCollection(IEqualityComparer<TKey> comparer) : base(comparer) { }

        //    #endregion public

        //    #endregion constructors

        //    #region methods

        //    #region protected

        //    protected override TKey GetKeyForItem(DictionaryEntry entry)
        //    {
        //        return (TKey)entry.Key;
        //    }

        //    #endregion protected

        //    #endregion methods
        //}

        #endregion KeyedDictionaryEntryCollection<TKey>

        #endregion protected classes

        #region public structures

        #region Enumerator
#if enumerator
        [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator<TEnumKey, TEnumValue> : IEnumerator<KeyValuePair<TEnumKey, TEnumValue>>, IDisposable, IDictionaryEnumerator, IEnumerator
        {
        #region constructors

            internal Enumerator(ReadOnlyObservableDictionary<TEnumKey, TEnumValue> dictionary, bool isDictionaryEntryEnumerator)
            {
                _dictionary = dictionary;
                _version = dictionary._version;
                _index = -1;
                _isDictionaryEntryEnumerator = isDictionaryEntryEnumerator;
                _current = new KeyValuePair<TEnumKey, TEnumValue>();
            }

        #endregion constructors

        #region properties

        #region public

            public KeyValuePair<TEnumKey, TEnumValue> Current
            {
                get
                {
                    ValidateCurrent();
                    return _current;
                }
            }

        #endregion public

        #endregion properties

        #region methods

        #region public

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                ValidateVersion();
                _index++;
                if (_index < _dictionary.UnderlyingDictionary.Count)
                {
                    _current = new KeyValuePair<TEnumKey, TEnumValue>((TEnumKey)_dictionary.UnderlyingDictionary[_index].Key, (TEnumValue)_dictionary.UnderlyingDictionary[_index].Value);
                    return true;
                }
                _index = -2;
                _current = new KeyValuePair<TEnumKey, TEnumValue>();
                return false;
            }

        #endregion public

        #region private

            private void ValidateCurrent()
            {
                if (_index == -1)
                {
                    throw new InvalidOperationException("The enumerator has not been started.");
                }
                else if (_index == -2)
                {
                    throw new InvalidOperationException("The enumerator has reached the end of the collection.");
                }
            }

            private void ValidateVersion()
            {
                if (_version != _dictionary._version)
                {
                    throw new InvalidOperationException("The enumerator is not valid because the dictionary changed.");
                }
            }

        #endregion private

        #endregion methods

        #region IEnumerator implementation

            object IEnumerator.Current
            {
                get
                {
                    ValidateCurrent();
                    if (_isDictionaryEntryEnumerator)
                    {
                        return new DictionaryEntry(_current.Key, _current.Value);
                    }
                    return new KeyValuePair<TEnumKey, TEnumValue>(_current.Key, _current.Value);
                }
            }

            void IEnumerator.Reset()
            {
                ValidateVersion();
                _index = -1;
                _current = new KeyValuePair<TEnumKey, TEnumValue>();
            }

        #endregion IEnumerator implemenation

        #region IDictionaryEnumerator implemenation

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    ValidateCurrent();
                    return new DictionaryEntry(_current.Key, _current.Value);
                }
            }
            object IDictionaryEnumerator.Key
            {
                get
                {
                    ValidateCurrent();
                    return _current.Key;
                }
            }
            object IDictionaryEnumerator.Value
            {
                get
                {
                    ValidateCurrent();
                    return _current.Value;
                }
            }

        #endregion

        #region fields

            private ReadOnlyObservableDictionary<TEnumKey, TEnumValue> _dictionary;
            private int _version;
            private int _index;
            private KeyValuePair<TEnumKey, TEnumValue> _current;
            private bool _isDictionaryEntryEnumerator;

        #endregion fields
        }
#endif
        #endregion Enumerator

        #endregion public structures

        #region fields


        //private int _countCache = 0;
        //private Dictionary<TKey, TValue> _dictionaryCache = new Dictionary<TKey, TValue>();
        //private int _dictionaryCacheVersion = 0;
        //private int _version = 0;


        #endregion fields
    }
}