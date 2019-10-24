using LionFire.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Referencing;

namespace LionFire.Persistence.Handles
{
    public abstract class ObservableReadHandleDictionary<TKey, THandle, T> : IEnumerable<T>
        where THandle : RH<T>
    {

        public bool ContainsKey(TKey key)
        {
            return handles.ContainsKey(key);
        }

        public ObservableReadHandleDictionary()
        {
            handlesReadOnly = new ReadOnlyObservableDictionary<TKey, THandle>(handles);
            handles.Added += OnHandleAdded;
            handles.Removed += OnHandleRemoved;
            Handles.CollectionChanged += Handles_CollectionChanged;
        }

        private void Handles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsObjectsEnabled)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    objects.Clear();
                    foreach (var item in Handles.Select(h => h.Value))
                    {
                        objects.Add(item.Value);
                    }
                }
                else
                {
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems.OfType<KeyValuePair<TKey,THandle>>().Select(kvp=>kvp.Value.Value).OfType<T>())
                        {
                            objects.Add(item);
                        }
                    }
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems.OfType<KeyValuePair<TKey, THandle>>().Select(kvp => kvp.Value.Value).OfType<T>())
                        {
                            objects.Remove(item);
                        }
                    }
                }
            }
        }

        //public bool IsWritable => typeof(IWriteHandle<T>).IsAssignableFrom(typeof(THandle);

        private async void OnHandleAdded(TKey key, THandle handle)
        {
            if (objectChanged != null)
            {
                handle.ObjectChanged += OnHandleObjectChanged;
            }
            if (AutoResolveObjects)
            {
                await handle.ResolveAsync().ConfigureAwait(false);
            }
        }
        private void OnHandleRemoved(TKey key, THandle handle)
        {
            if (objectChanged != null)
            {
                handle.ObjectChanged -= OnHandleObjectChanged;
            }
        }

        #region AutoResolveObjects

        public bool AutoResolveObjects
        {
            get { return autoResolveObjects; }
            set { autoResolveObjects = value; }
        }
        private bool autoResolveObjects;

        #endregion

        #region Handles

        public IReadOnlyObservableDictionary<TKey, THandle> Handles
        {
            get => handlesReadOnly;
        }
        ReadOnlyObservableDictionary<TKey, THandle> handlesReadOnly;

        protected readonly ObservableDictionary<TKey, THandle> handles = new ObservableDictionary<TKey, THandle>();

        #endregion

        #region Objects

        #region IsObjectsEnabled

        public bool IsObjectsEnabled
        {
            get { return isObjectsEnabled; }
            set
            {
                if (isObjectsEnabled == value) return;
                isObjectsEnabled = value;
                if (isObjectsEnabled)
                {
                    foreach (var handle in handles.Values)
                    {
                        var obj = handle.Value;
                        if (obj != null)
                        {
                            objects.Add(obj);
                        }
                    }
                }
                else
                {
                    objects.Clear();
                }
            }
        }
        private bool isObjectsEnabled;

        #endregion

        public ReadOnlyObservableCollection<T> Objects
        {
            get
            {
                if (objectsReadOnly == null)
                {
                    objectsReadOnly = new ReadOnlyObservableCollection<T>(objects);
                }
                return objectsReadOnly;
            }
        }
        private ReadOnlyObservableCollection<T> objectsReadOnly;

        private ObservableCollection<T> objects
        {
            get
            {
                if (_objects == null) { _objects = new ObservableCollection<T>(); }
                return _objects;
            }
        }
        private ObservableCollection<T> _objects;

        public event NotifyCollectionChangedEventHandler ObjectsCollectionChanged
        {
            add
            {
                objects.CollectionChanged += value;
            }
            remove
            {
                objects.CollectionChanged -= value;
            }
        }

        #endregion

        public event Action<RH<T>> ObjectChanged
        {
            add
            {
                if (objectChanged == null)
                {
                    foreach (var handle in handles.Values)
                    {
                        handle.ObjectChanged += OnHandleObjectChanged;
                    }
                }
                objectChanged += value;
            }
            remove
            {
                objectChanged -= value;
                if (objectChanged == null)
                {
                    foreach (var handle in handles.Values)
                    {
                        handle.ObjectChanged += OnHandleObjectChanged;
                    }
                }
            }
        }
        private event Action<RH<T>> objectChanged;

        // This event is mapped to ObjectChanged on all child handles
        public event Action<RH<T>, T, T> ObjectReferenceChanged
        {
            add
            {
                if (objectReferenceChanged == null)
                {
                    foreach (var handle in handles.Values)
                    {
                        handle.ObjectReferenceChanged += OnHandleObjectReferenceChanged;
                    }
                }
                objectReferenceChanged += value;
            }
            remove
            {
                objectReferenceChanged -= value;
                if (objectReferenceChanged == null)
                {
                    foreach (var handle in handles.Values)
                    {
                        handle.ObjectReferenceChanged += OnHandleObjectReferenceChanged;
                    }
                }
            }
        }
        private event Action<RH<T>, T, T> objectReferenceChanged;

        private void OnHandleObjectReferenceChanged(RH<T> handle, T oldValue, T newValue)
        {
            objectReferenceChanged?.Invoke(handle, oldValue, newValue);
            if (IsObjectsEnabled)
            {
                if (oldValue != null)
                {
                    objects.Remove(oldValue);
                }
                if (newValue != null)
                {
                    objects.Add(newValue);
                }
            }
        }

        private void OnHandleObjectChanged(RH<T> handle)
        {
            objectChanged?.Invoke(handle);
        }


        public abstract Task RefreshHandles();

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)Objects).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)Objects).GetEnumerator();
        }

        #endregion

    }

}
