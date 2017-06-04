using LionFire.Collections;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;

namespace LionFire.Serialization
{
    public abstract class ObservableReadHandleDictionary<TKey, THandle, T> 
        where THandle : IReadHandle<T>
    {

        public ObservableReadHandleDictionary()
        {
            handlesReadOnly = new ReadOnlyObservableDictionary<TKey, THandle>(handles);
            handles.Added += OnHandleAdded;
            handles.Removed += OnHandleRemoved;
            Handles.CollectionChanged += Handles_CollectionChanged;
        }

        private void Handles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
            Debug.WriteLine("NewItems: " + e.NewItems);
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
                await handle.TryResolveObject().ConfigureAwait(false);
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
                        var obj = handle.Object;
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

        public event Action<IReadHandle<T>, T, T> ObjectChanged
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
        private event Action<IReadHandle<T>, T, T> objectChanged;

        private void OnHandleObjectChanged(IReadHandle<T> handle, T oldValue, T newValue)
        {
            objectChanged?.Invoke(handle, oldValue, newValue);
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


        public abstract void RefreshHandles();

    }

}
