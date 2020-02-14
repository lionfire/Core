
// Retrieved from http://karlhulme.wordpress.com/2007/03/04/synchronizedobservablecollection-and-bindablecollection/ on Feb 7, 2010
// License: If anyone can make use of the above classes then you’re very welcome to use them, although you do so at your own risk! 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;
//using System.Windows.Threading;
//#if !UNITY
//using System.Windows.Data;
//#endif
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using LionFire.Structures;

namespace LionFire.Collections
{
    //public interface ISynchronizedObservableWrappedCollectionWrapper
    //{
    //    object Object { get; }
    //}

    public class SynchronizedObservableWrappedCollection<T, WrapperType> : SynchronizedObservableCollection<WrapperType>
        //where WrapperType : ISynchronizedObservableWrappedCollectionWrapper
        where WrapperType : IReadWrapper<object>
    {

        #region Static

        static SynchronizedObservableWrappedCollection()
        {
            DefaultWrapperFactory = new Func<T, WrapperType>(targ => (WrapperType)Activator.CreateInstance(typeof(WrapperType), targ));
        }
        private static Func<T, WrapperType> DefaultWrapperFactory;

        #endregion

        public SynchronizedObservableWrappedCollection()
        {
        }
        public SynchronizedObservableWrappedCollection(SynchronizedObservableCollection<T> target, Func<T, WrapperType> wrapperFactory = null)
        {
            this.WrapperFactory = wrapperFactory; // Do this first!
            this.Target = target; // Invokes wrapper factory
        }

        public SynchronizedObservableCollection<T> Target
        {
            get { return target; }
            set
            {
                if (target != null)
                {
                    target.CollectionChanged -= target_CollectionChanged;
                }

                this.target = value;

                if (target != null)
                {
                    target.CollectionChanged += target_CollectionChanged;
                }

                OnTargetReset();
            }
        } private SynchronizedObservableCollection<T> target;

        private void OnTargetReset()
        {
            // TODO: Reuse wrapper objects if IWrapper.WrappedObject matches
            this.Clear();

            if (Target != null)
            {
                foreach (var item in Target.ToArray())
                {
                    this.Add(WrapperFactory(item));
                }
            }
        }


        void target_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                OnTargetReset();
            }
            else
            {
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems.OfType<T>().ToArray())
                    {
                        this.Add(WrapperFactory(item));
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems.OfType<T>().ToArray())
                    {
                        foreach (var wrapper in this.Where(w => object.ReferenceEquals(w.Value, item)).ToArray())
                        {
                            this.Remove(wrapper);
                        }
                    }
                }
            }
        }

        public Func<T, WrapperType> WrapperFactory
        {
            get { return wrapperFactory ?? DefaultWrapperFactory; }
            set { wrapperFactory = value; }
        } private Func<T, WrapperType> wrapperFactory;
    }

    [ComVisible(false)]
    public class SynchronizedObservableCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>,
        IList, ICollection, IEnumerable, INotifyPropertyChanged, INotifyCollectionChanged, INotifyingCollection<T>
    {
        //private static readonly ILogger l = Log.Get();

        private ObservableList<T> items;
        private Object sync;

        #region Constructors

        public SynchronizedObservableCollection(Object syncRoot, IEnumerable<T> list)
        {
            this.sync = (syncRoot == null) ? new Object() : syncRoot;
            this.items = (list == null) ? new ObservableList<T>() :
                new ObservableList<T>(new List<T>(list));

            items.CollectionChanged += delegate(Object sender, NotifyCollectionChangedEventArgs e)
            {
                OnCollectionChanged(e);
            };
            INotifyPropertyChanged propertyChangedInterface = items as INotifyPropertyChanged;
            propertyChangedInterface.PropertyChanged += delegate(Object sender, PropertyChangedEventArgs e)
            {
                OnPropertyChanged(e);
            };
        }

        public SynchronizedObservableCollection(object syncRoot) : this(syncRoot, null) { }

        public SynchronizedObservableCollection() : this(null, null) { }

        #endregion

        #region Methods

        public void Add(T item)
        {
            lock (this.sync)
            {
                this.items.Add(item);
                //int index = this.items.Count;
                //this.InsertItem(index, item);
            }
        }
        public void AddRange(IList<T> item)
        {
            InsertRange(Count, item);
            //lock (this.sync)
            //{
            //    this.items.Add(item);
            //    //int index = this.items.Count;
            //    //this.InsertItem(index, item);
            //}
        }

        public void Clear()
        {
            lock (this.sync)
            {
                this.ClearItems();
            }
        }

        protected virtual void ClearItems()
        {
            this.items.Clear();
        }

        public bool Contains(T item)
        {
            lock (this.sync)
            {
                return this.items.Contains(item);
            }
        }

        public void CopyTo(T[] array, int index)
        {
            lock (this.sync)
            {
                this.items.CopyTo(array, index);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (this.sync)
            {
                return this.items.GetEnumerator();
            }
        }

        public int IndexOf(T item)
        {

            // lock (this.sync) REVIEW
            {
                return this.InternalIndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (this.sync)
            {
                if ((index < 0) || (index > this.items.Count))
                {
                    throw new ArgumentOutOfRangeException("index", index, "Value must be in range.");
                }
                this.InsertItem(index, item);
            }
        }

        protected virtual void InsertItem(int index, T item)
        {
            this.items.Insert(index, item);
        }

        private int InternalIndexOf(T item)
        {
            int count = this.items.Count;
            for (int i = 0; i < count; i++)
            {
                if (object.Equals(this.items[i], item))
                {
                    return i;
                }
            }
            return -1;
        }

        public void InsertRange(int index, IEnumerable<T> coll)
        {
            if (!coll.Any()) return;
            // Checks?  Renentrancy?
            lock (this.sync)
            {
                var list = coll as IList<T>;
                if (list == null) list = coll.ToList<T>();
                this.items.InsertRange(index, list);
            }

            OnReset(); // OPTIMIZE - don't reset?
        }
        private void OnReset()
        {
            OnPropertyChanged("Count"); // Needed?
            OnPropertyChanged("Item[]");
            //OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                //NotifyCollectionChangedAction.Reset));
        }

        public bool Remove(T item)
        {
            lock (this.sync)
            {
                int index = this.InternalIndexOf(item);
                if (index < 0)
                {
                    return false;
                }
                this.RemoveItem(index);
                return true;
            }
        }

        public void RemoveAt(int index)
        {
            lock (this.sync)
            {
                if ((index < 0) || (index >= this.items.Count))
                {
                    throw new ArgumentOutOfRangeException("index", index,
                        "Value must be in range.");
                }
                this.RemoveItem(index);
            }
        }

        protected virtual void RemoveItem(int index)
        {
            lock (this.sync)
            {
                this.items.RemoveAt(index);
            }
        }
        public void RemoveRange(int index, int count) // UNTESTED
        {
            // Checks?  Renentrancy?
            lock (this.sync)
            {
                this.items.RemoveRange(index, count);
            }
            OnReset();  // OPTIMIZE - don't reset?
        }

        protected virtual void SetItem(int index, T item)
        {
            this.items[index] = item;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            lock (this.sync)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    array.SetValue(items[i], index + i);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        int IList.Add(object value)
        {
            VerifyValueType(value);
            lock (this.sync)
            {
                this.Add((T)value);
                return (this.Count - 1);
            }
        }

        bool IList.Contains(object value)
        {
            VerifyValueType(value);
            return this.Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            VerifyValueType(value);
            return this.IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            VerifyValueType(value);
            this.Insert(index, (T)value);
        }

        void IList.Remove(object value)
        {
            VerifyValueType(value);
            this.Remove((T)value);
        }

        private static void VerifyValueType(object value)
        {
            if (value == null)
            {
                if (typeof(T).IsValueType)
                {
                    throw new ArgumentException("Synchronized collection wrong type null.");
                }
            }
            else if (!(value is T))
            {
                throw new ArgumentException("Synchronized collection wrong type.");
            }
        }
        #endregion

        #region Properties

        public int Count
        {
            get
            {
                //lock (this.sync)
                {
                    return this.items.Count;
                }
            }
        }

        public T this[int index]
        {
            get
            {
                //lock (this.sync)
                {
                    return this.items[index];
                }
            }
            set
            {
                lock (this.sync)
                {
                    if ((index < 0) || (index >= this.items.Count))
                    {
                        throw new ArgumentOutOfRangeException("index", index,
                            "Value must be in range.");
                    }
                    this.SetItem(index, value);
                }
            }
        }

        protected ObservableList<T> Items
        {
            get
            {
                return this.items;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this.sync;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return true;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this.sync;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                VerifyValueType(value);
                this[index] = (T)value;
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;
            if (ev != null) ev(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var ev = PropertyChanged;
            if (ev != null) ev(this, e);
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {

#if OLD
            {
                var ev = CollectionChanged;
                if (ev != null) ev(this, e);
            }
#else
            // From http://stackoverflow.com/questions/2137769/where-do-i-get-a-thread-safe-collectionview
            // Another possibility: (wrap whole method with using?  If target is CollectionView?
            //ICollectionView view = CollectionViewSource.GetDefaultView(collection);
            //using (var disposable = view.DeferRefresh())
            //{
            //    // Add multiple items to collection
            //}
            NotifyCollectionChangedEventHandler CollectionChanged = this.CollectionChanged;
            if (CollectionChanged != null)
                foreach (NotifyCollectionChangedEventHandler nh in CollectionChanged.GetInvocationList())
                {
#if Dispatcher
                    var dispObj = nh.Target as DispatcherObject;
                    if (dispObj != null)
                    {
                        Dispatcher dispatcher = dispObj.Dispatcher;
                        var nhClosureCopy = nh;
                        if (dispatcher != null && !dispatcher.CheckAccess())
                        {
                            dispatcher.BeginInvoke(
                                (Action)(() =>
                                    {
                                        try
                                        {
#if true
#if !UNITY && !MONO
                                            if (
                                                ((e.NewItems != null && e.NewItems.Count > 1) || e.OldItems!=null && e.OldItems.Count > 1)
                                                &&
                                                nhClosureCopy.Target is CollectionView) // OPTIMIZE - only do this check for multi?
                                            {
                                                ((CollectionView)nhClosureCopy.Target).Refresh();
                                            }
                                            else
#endif
                                            {
                                                // RECENTCHANGE - try passing through the event
                                                nhClosureCopy.Invoke(this, e);
                                                //nhClosureCopy.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                                            }
#else // OLD
                                            nhClosureCopy.Invoke(this,
                                                   new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
#endif
                                        }
                                        catch (Exception ex)
                                        {
                                            l.Error("OnCollectionChanged dispatcher BeginInvoker's invoke threw: " + ex);
                                        }
                                    }
                                    ),
                                    DispatcherPriority.DataBind,
                                    new object[] { });
                            continue;
                        }
#endif
                    // else fall thru
                }
#if WPF
#if !UNITY && !MONO
            if (
                        ((e.NewItems != null
                        && e.NewItems.Count > 1
                        ) 
                        || e.OldItems != null
                        && e.OldItems.Count > 1
                        )
                                                &&
                                                nh.Target is CollectionView) // OPTIMIZE - only do this check for multi?
                    {
                        ((CollectionView)nh.Target).Refresh();
                    }
                    else
#endif
                    { 
                        nh.Invoke(this, e);
                    }
                }
                
#endif
#endif
#if !AOT
            {
                CollectionChanged2?.Invoke(new NotifyCollectionChangedEventArgs<T>(e));
            }
#endif
        }

        #endregion


        public T[] ToArray()
        {
            return items.ToArray();
        }

#if !AOT
        event NotifyCollectionChangedHandler<T> INotifyCollectionChanged<T>.CollectionChanged
        {
            add { this.CollectionChanged2 += value; }
            remove { this.CollectionChanged2 -= value; }
        }
        private event NotifyCollectionChangedHandler<T> CollectionChanged2;
#endif
    }
}
