#if TODO // Probably need to do own implementations.  Come up with a plan for simple/extended classes and their features and base classes.  Base class for handle Dictionaries that aren't handles themselves?
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
using LionFire.Resolves;

namespace LionFire.Persistence.Handles
{
    public class ObservableReadHandleDictionaryEx<TKey, THandle, T> : ObservableReadDictionary<TKey, THandle, T>
        where THandle : IReadHandle<T>, INotifyPersists<T>
    {
    }

    public class ReadHandleDictionaryBase<TKey, THandle, T> : IEnumerable<T>
        where THandle : IReadHandleBase<T>
    {

    }

    /// <summary>
    /// No Events
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="THandle"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class ReadHandleDictionary<TKey, THandle, T> : ReadHandleDictionaryBase<TKey, THandle, T>
    where THandle : IReadHandleBase<T>
    {
#region Construction

        public ReadHandleDictionary()
        {
            handlesReadOnly = new ReadOnlyObservableDictionary<TKey, THandle>(handles);
        }

#endregion
    }

    public class ObservableReadDictionaryHandle<TReference, THandle, TValue> : ObservableReadDictionary<TReference, THandle, TValue> : IResolves<IEnumerable<T>>, IReferencable<TReference>
        where THandle : IReadHandle<TValue>, INotifyPersists<TValue>
        where TReference : class, IReference // TODO: class only needed for == operator
    {
        public abstract Task RefreshHandles();

#region Reference

        public TReference Reference
        {
            get => reference;
            set
            {
                if (ReferenceEquals(reference, value)) { return; }
                if (reference != default(TReference)) { throw new AlreadySetException(); }
                reference = value;
            }
        }
        protected TReference reference;

#endregion

#region Persistence State

        public PersistenceFlags Flags
        {
            get => handleState;
            set
            {
                if (handleState == value) { return; }

                var oldValue = handleState;
                handleState = value;

                //OnStateChanged(value, oldValue);
            }
        }
        private PersistenceFlags handleState;

        //protected virtual void OnStateChanged(PersistenceFlags newValue, PersistenceFlags oldValue) { }

#endregion

    }

    /// <summary>
    /// TODO: Require that all handles added already have keys (References) set
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="THandle"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ObservableReadDictionary<TKey, THandle, TValue> : ReadHandleDictionary<TKey, THandle, TValue>
        , IReadOnlyObservableDictionary<TKey, TValu>
    where THandle : IReadHandle<TValue>, INotifyPersists<TValue>
    {
#region Construction

        public ObservableReadDictionary()
        {
            handlesReadOnly = new ReadOnlyObservableDictionary<TKey, THandle>(handles);
            handles.Added += OnHandleAdded;
            handles.Removed += OnHandleRemoved;
            Handles.CollectionChanged += Handles_CollectionChanged;
        }

#endregion

#region Properties

#region Handles

        public IReadOnlyObservableDictionary<TKey, THandle> Handles => handlesReadOnly;
        ReadOnlyObservableDictionary<TKey, THandle> handlesReadOnly;

        protected readonly ObservableDictionary<TKey, THandle> handles = new ObservableDictionary<TKey, THandle>(); // TODO: don't rely on this class -- do own implementation

#endregion

#endregion

#region Handles collection events

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
                        foreach (var item in e.NewItems.OfType<KeyValuePair<TKey, THandle>>().Select(kvp => kvp.Value.Value).OfType<TValue>())
                        {
                            objects.Add(item);
                        }
                    }
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems.OfType<KeyValuePair<TKey, THandle>>().Select(kvp => kvp.Value.Value).OfType<TValue>())
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
            if (childPersistenceStateChanged != null)
            {
                handle.PersistenceStateChanged += OnHandlePersistenceStateChanged;
            }
            if (AutoResolveObjects)
            {
                await handle.Resolve().ConfigureAwait(false);
            }
        }

        private void OnHandleRemoved(TKey key, THandle handle)
        {
            if (childPersistenceStateChanged != null)
            {
                handle.PersistenceStateChanged -= OnHandlePersistenceStateChanged;
            }
        }

#endregion

#region ChildPersistenceStateChanged - Pass thru for PersistenceStateChanged 

        public event Action<PersistenceEvent<TValue>> ChildPersistenceStateChanged
        {
            add
            {
                if (childPersistenceStateChanged == null)
                {
                    foreach (var handle in handles.Values)
                    {
                        handle.PersistenceStateChanged += OnHandlePersistenceStateChanged;
                    }
                }
                childPersistenceStateChanged += value;
            }
            remove
            {
                childPersistenceStateChanged -= value;
                if (childPersistenceStateChanged == null)
                {
                    foreach (var handle in handles.Values)
                    {
                        handle.PersistenceStateChanged -= OnHandlePersistenceStateChanged;
                    }
                }
            }
        }
        private event Action<PersistenceEvent<TValue>> childPersistenceStateChanged;

        // TODO: Make a simpler Object changed event off this?  Leave that to a derived Ex class?
        private void OnHandlePersistenceStateChanged(PersistenceEvent<TValue> ev) => childPersistenceStateChanged?.Invoke(ev);

#endregion

        public bool ContainsKey(TKey key) => handles.ContainsKey(key);

#region AutoResolveObjects 

        // MOVE to Ex class?

#if TODO
        public enum AutoResolveMode
        {
            None,
            Await,
            FireAndForget,
            //Block,
        }

        public AutoResolveMode AutoResolveMode { get; set; }
#else
        public bool AutoResolveObjects
        {
            get { return autoResolveObjects; }
            set { autoResolveObjects = value; }
        }
        private bool autoResolveObjects;
#endif

#endregion

#region Objects

#region IsObjectsEnabled

        //public bool IsObjectsEnabled
        //{
        //    get { return isObjectsEnabled; }
        //    set
        //    {
        //        if (isObjectsEnabled == value) return;
        //        isObjectsEnabled = value;
        //        if (isObjectsEnabled)
        //        {
        //            foreach (var handle in handles.Values)
        //            {
        //                var obj = handle.Value;
        //                if (obj != null)
        //                {
        //                    objects.Add(obj);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            objects.Clear();
        //        }
        //    }
        //}
        //private bool isObjectsEnabled;

#endregion

        //public ReadOnlyObservableCollection<T> Objects
        //{
        //    get
        //    {
        //        if (objectsReadOnly == null)
        //        {
        //            objectsReadOnly = new ReadOnlyObservableCollection<T>(objects);
        //        }
        //        return objectsReadOnly;
        //    }
        //}
        //private ReadOnlyObservableCollection<T> objectsReadOnly;

        //private ObservableCollection<T> objects
        //{
        //    get
        //    {
        //        if (_objects == null) { _objects = new ObservableCollection<T>(); }
        //        return _objects;
        //    }
        //}
        //private ObservableCollection<T> _objects;

        //public event NotifyCollectionChangedEventHandler ObjectsCollectionChanged
        //{
        //    add
        //    {
        //        objects.CollectionChanged += value;
        //    }
        //    remove
        //    {
        //        objects.CollectionChanged -= value;
        //    }
        //}

#endregion

        //public event Action<IReadHandleBase<T>> ObjectChanged
        //{
        //    add
        //    {
        //        if (objectChanged == null)
        //        {
        //            foreach (var handle in handles.Values)
        //            {
        //                handle.ObjectChanged += OnHandleObjectChanged;
        //            }
        //        }
        //        objectChanged += value;
        //    }
        //    remove
        //    {
        //        objectChanged -= value;
        //        if (objectChanged == null)
        //        {
        //            foreach (var handle in handles.Values)
        //            {
        //                handle.ObjectChanged -= OnHandleObjectChanged;
        //            }
        //        }
        //    }
        //}
        //private event Action<IReadHandleBase<T>> objectChanged;

        // This event is mapped to ObjectChanged on all child handles
        //public event Action<IReadHandleBase<T>, T, T> ObjectReferenceChanged
        //        {
        //            add
        //            {
        //                if (objectReferenceChanged == null)
        //                {
        //                    foreach (var handle in handles.Values)
        //                    {
        //                        handle.ObjectReferenceChanged += OnHandleObjectReferenceChanged;
        //                    }
        //}
        //objectReferenceChanged += value;
        //            }
        //            remove
        //            {
        //                objectReferenceChanged -= value;
        //                if (objectReferenceChanged == null)
        //                {
        //                    foreach (var handle in handles.Values)
        //                    {
        //                        handle.ObjectReferenceChanged += OnHandleObjectReferenceChanged;
        //                    }
        //                }
        //            }
        //        }
        //private event Action<IReadHandleBase<T>, T, T> objectReferenceChanged;

        //private void OnHandleObjectReferenceChanged(IReadHandleBase<T> handle, T oldValue, T newValue)
        //{
        //    objectReferenceChanged?.Invoke(handle, oldValue, newValue);
        //    if (IsObjectsEnabled)
        //    {
        //        if (oldValue != null)
        //        {
        //            objects.Remove(oldValue);
        //        }
        //        if (newValue != null)
        //        {
        //            objects.Add(newValue);
        //        }
        //    }
        //}

        //        private void OnHandleObjectChanged(IReadHandleBase<T> handle)
        //{
        //    childPersistenceStateChanged?.Invoke(handle);
        //}



    }

    public class ObservableReadDictionaryEx<TKey, THandle, TValue> : ObservableReadDictionary<TKey, THandle, TValue>, IEnumerable<TValue>
        where THandle : IReadHandle<TValue>, INotifyPersists<TValue>
    {
#region IEnumerable<T>

        public IEnumerator<TValue> GetEnumerator() => ((IEnumerable<TValue>)Handles.Values).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TValue>)Objects).GetEnumerator();

#endregion
    }

}

#endif