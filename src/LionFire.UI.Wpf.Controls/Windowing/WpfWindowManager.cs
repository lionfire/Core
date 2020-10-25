using LionFire.Shell;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics.CodeAnalysis;

namespace LionFire.UI
{
    public class CovariantConcurrentDictionary<TKey, TValue> : ICovariantReadOnlyDictionary<TKey, TValue>
        , ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, 
        
        IDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, 
        System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>, 

        where TKey : notnull
    {
        ConcurrentDictionary<TKey, TValue> _inner = new ConcurrentDictionary<TKey, TValue>();

        public TValue this[TKey key] { get => ((IDictionary<TKey, TValue>)_inner)[key]; set => ((IDictionary<TKey, TValue>)_inner)[key] = value; }
        public object this[object key] { get => ((IDictionary)_inner)[key]; set => ((IDictionary)_inner)[key] = value; }

        public int Count => ((ICollection<KeyValuePair<TKey, TValue>>)_inner).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_inner).IsReadOnly;

        public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)_inner).Keys;

        public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)_inner).Values;

        public bool IsSynchronized => ((ICollection)_inner).IsSynchronized;

        public object SyncRoot => ((ICollection)_inner).SyncRoot;

        public bool IsFixedSize => ((IDictionary)_inner).IsFixedSize;

        ICollection IDictionary.Keys => ((IDictionary)_inner).Keys;

        IEnumerable<TKey> System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.Keys => ((System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>)_inner).Keys;

        ICollection IDictionary.Values => ((IDictionary)_inner).Values;

        IEnumerable<TValue> System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>.Values => ((System.Collections.Generic.IReadOnlyDictionary<TKey, TValue>)_inner).Values;

        public void Add(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)_inner).Add(item);
        public void Add(TKey key, TValue value) => ((IDictionary<TKey, TValue>)_inner).Add(key, value);
        public void Add(object key, object value) => ((IDictionary)_inner).Add(key, value);
        public void Clear() => ((ICollection<KeyValuePair<TKey, TValue>>)_inner).Clear();
        public bool Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)_inner).Contains(item);
        public bool Contains(object key) => ((IDictionary)_inner).Contains(key);
        public bool ContainsKey(TKey key) => ((IDictionary<TKey, TValue>)_inner).ContainsKey(key);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)_inner).CopyTo(array, arrayIndex);
        public void CopyTo(Array array, int index) => ((ICollection)_inner).CopyTo(array, index);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)_inner).GetEnumerator();
        public bool Remove(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)_inner).Remove(item);
        public bool Remove(TKey key) => ((IDictionary<TKey, TValue>)_inner).Remove(key);
        public void Remove(object key) => ((IDictionary)_inner).Remove(key);
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => ((IDictionary<TKey, TValue>)_inner).TryGetValue(key, out value);
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => ((System.Collections.IEnumerable)_inner).GetEnumerator();
        IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary)_inner).GetEnumerator();
    }

    public abstract class WindowManagerBase : IWindowManager
    {
        public ICovariantReadOnlyDictionary<string, INamedUINode> Object => Windows;

        protected ConcurrentDictionary<string, IPresenter> Windows { get; } = new ConcurrentDictionary<string, IPresenter>();
        System.Collections.Generic.IReadOnlyDictionary<string, IPresenter> IWindowManager.Windows => Windows;

        public abstract Task<IPresenter> CreateWindow(string windowName, object context = null, IDictionary<string, object> settings = null);
    }

    public class WpfWindowManager : WindowManagerBase
    {
        #region Dependencies

        public IOptionsMonitor<UIOptions> UIOptionsMonitor { get; }

        #endregion

        #region Construction

        public WpfWindowManager(IOptionsMonitor<UIOptions> uiOptionsMonitor)
        {
            UIOptionsMonitor = uiOptionsMonitor;
        }

        #endregion

        #region (Public) Methods

        public IPresenter CreateWindow(string windowName, object context = null, IDictionary<string, object> settings = null)
        {
            var presenter = new WpfPresenter(parent: null);

            presenter.Show()

            return presenter;
        }

        #endregion

    }

    public interface IWindowVM
    {

    }

    public class WpfWindowVM
    {
        public 

    }
}
