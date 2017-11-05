#define SanityChecks
// Retrieved from http://karlhulme.wordpress.com/2007/03/04/synchronizedobservablecollection-and-bindablecollection/ on Feb 7, 2010
// License: If anyone can make use of the above classes then you’re very welcome to use them, although you do so at your own risk! using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;
#if UNITY
using LionFire.Backports;
#endif

namespace LionFire.Collections
{
	public delegate void RaiseCollectionChangedEventHandler(NotifyCollectionChangedEventArgs e);

	internal static class MultiBindableEvents
	{
		public static bool AutoDetachThrowingHandlers = true;
		public static bool TraceDetachThrowingHandlers = true;
		public static bool RaiseResetOnThrow = true;

		internal static void RaiseCollectionChangedEventNonGeneric(this NotifyCollectionChangedEventHandler ev, object sender, NotifyCollectionChangedEventArgs e)
		{
			if(ev == null) {
				return;
			}

			if(e.Action == NotifyCollectionChangedAction.Remove && e.OldStartingIndex < 0) {
				l.TraceWarn("Replacing NotifyCollectionChangedAction.Remove event with Reset because there is no index");
				MultiBindableEvents.RaiseCollectionChangedEventNonGeneric(ev, sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				return;
			}

			foreach(NotifyCollectionChangedEventHandler del in ev.GetInvocationList()) {
				try {
					DispatcherObject dispatcherObject = del.Target as DispatcherObject;
					if(dispatcherObject != null) {
						if(dispatcherObject.Dispatcher != null && !dispatcherObject.Dispatcher.CheckAccess()) {
							dispatcherObject.Dispatcher.Invoke(del, new object[] { sender, e });
//                            dispatcherObject.Dispatcher.Invoke(del, sender, e);
							continue;
						}
					}
					del.Invoke(sender, e);
				} catch(Exception ex) {
					if((ex as ArgumentOutOfRangeException != null) && RaiseResetOnThrow) {
						try {
							del.Invoke(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
							continue;
						} catch(Exception ex2) {
							l.Warn("RaiseResetOnThrow also threw: " + ex2);
						}
					}
					if(AutoDetachThrowingHandlers) {
						ev -= del;
						if(TraceDetachThrowingHandlers) {
							l.Error("MultiBindableCollection event handler (NotifyCollectionChangedEventHandler) threw exception.  Detaching.  Exception: " + ex.ToString());
							//System.Diagnostics.Trace.WriteLine("MultiBindableCollection event handler threw exception.  Detaching.  Exception: " + ex.ToString());
						}
					}
				}
			}
		}

		internal static void RaiseCollectionChanged<ItemType>(NotifyCollectionChangedHandler<ItemType> ev, NotifyCollectionChangedEventArgs eventArgs)
		{
			// COPY See also: corresponding method in MultiBindableDictionary
			if(ev == null) {
				return;
			}

			var args = new NotifyCollectionChangedEventArgs<ItemType>(eventArgs);

			#if AOT
			AotSafe.ForEach<object>( ev.GetInvocationList(), delX => {
				NotifyCollectionChangedHandler<ItemType> del = (NotifyCollectionChangedHandler<ItemType>)delX;
#else
			foreach(NotifyCollectionChangedHandler<ItemType> del in ev.GetInvocationList()) {
#endif
			                
				try {
					if(del.Target as DispatcherObject != null) {
						DispatcherObject dispatcherObject = (DispatcherObject)del.Target;

						// REVIEW - Changed to BeginInvoke.  No point in waiting for the invocation to finish! (Unless perhaps we wanted to do error handling, or stuff was happening on a gui thread.) 
						if(dispatcherObject.Dispatcher != null && !dispatcherObject.Dispatcher.CheckAccess()) {
							dispatcherObject.Dispatcher.BeginInvoke(del, new object[] { args });
#if AOT
								return;
#else
							continue;
#endif
						}
					}
                    // By default, invoke on same thread                    
					del.Invoke(args);
				} catch(Exception ex) {
					if(AutoDetachThrowingHandlers) {
						ev -= del;
						if(TraceDetachThrowingHandlers) {
							l.Error("MultiBindableCollection event handler (NotifyCollectionChangedHandler<T>) threw exception.  Detaching.  Exception: " + ex.ToString());
							//System.Diagnostics.Trace.WriteLine("MultiBindableCollection event handler threw exception.  Detaching.  Exception: " + ex.ToString());
						}
					}
				}
			}
			#if AOT
			);
#endif
		}

		private static ILogger l = Log.Get();
	}
    /// <summary>
    /// TODO - Finish implementing.  Some methods will throw exception.  Some event raising needs review.
    /// THREADSAFETY - make threadsafe?
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public class MultiBindableCollection<T> : INotifyingList<T>, INotifyCollectionChanged,
        INotifyPropertyChanged,
        INotifyingReadOnlyCollection<T>
	{
#if AOT
				private ObservableList<T> list;
#else
				private IList<T> list;
#endif
        //private Dispatcher dispatcher;

        [Ignore]
#if AOT || NET35 
		private ThreadLocal<object> EventsEnabled = new ThreadLocal<object>(() => true);
#else
        private ThreadLocal<bool> EventsEnabled = new ThreadLocal<bool>(() => true);
#endif

		public T[] ToArray()
		{
			return list.ToArray();
		}
        #region Constructor

		public MultiBindableCollection() : this(null)
		{
		}
// Jared added
        //public MultiBindableCollection(IList<T> list) : this(list, null) { }

#if AOT
				public MultiBindableCollection(ObservableList<T> list
#else
				public MultiBindableCollection(IList<T> list
#endif
            //, Dispatcher dispatcher
		)
		{
			if(list == null) { // Jared added
				list = new ObservableList<T>();
			}
            
			if(list == null ||
				list as INotifyCollectionChanged == null ||
				list as INotifyPropertyChanged == null) {
				throw new ArgumentNullException("The list must support IList<>, INotifyCollectionChanged " +
                    "and INotifyPropertyChanged.");
			}

			this.list = list;

			//this.dispatcher = (dispatcher == null) ? Dispatcher.CurrentDispatcher : dispatcher;

			INotifyCollectionChanged collectionChanged = list as INotifyCollectionChanged;
			collectionChanged.CollectionChanged += OnCollectionChanged;

			INotifyPropertyChanged propertyChanged = list as INotifyPropertyChanged;
			propertyChanged.PropertyChanged += OnInnerPropertyChanged;
		}

		private void OnInnerPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			//if (!this.dispatcher.CheckAccess())
			//{
			//    this.dispatcher.Invoke(DispatcherPriority.Normal,
			//        new RaisePropertyChangedEventHandler(RaisePropertyChangedEvent), e);
			//}
			//else
//			{
				RaisePropertyChangedEvent(e);
//			}
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
		{
			if(!(bool)EventsEnabled.Value)
				return;
			bool firstTime = true;
			raise:
			try {
				//if (!this.dispatcher.CheckAccess())
				//{
				//    this.dispatcher.Invoke(DispatcherPriority.Normal,
				//        new RaiseCollectionChangedEventHandler(RaiseCollectionChangedEvent), e);
				//}
				//else
				{
//					l.Warn("XBJIZ - Pre MultiBindableEvents.RaiseCollectionChanged - does this work in AOT?");
//#if !AOT // TEMP AOTDEBUG FIXME
#if AOT
					MultiBindableEvents.RaiseCollectionChanged(CollectionChanged_event, eventArgs); // GENERICMETHOD - works in AOT
#else
                    MultiBindableEvents.RaiseCollectionChanged(CollectionChanged, eventArgs); // GENERICMETHOD - works in AOT
#endif
//#endif
//					l.Warn("XBJIZ - Post MultiBindableEvents.RaiseCollectionChanged");
					MultiBindableEvents.RaiseCollectionChangedEventNonGeneric(this.collectionChangedNonGeneric, this, eventArgs);
				}
			} catch(Exception ex) {
				if(firstTime) {
					l.Warn("Exception when raising event.  Trying again with reset event.  Exception: " + ex.ToString());
					eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
					firstTime = false;
					goto raise;
				} else {
					l.Error("Exception when raising event. Exception: " + ex.ToString());
				}
			}
		}

        public int RemoveAll(IEnumerable<T> items)
        {
            int count = 0;
            foreach(var item in items)
            {
                if (this.Remove(item)) count++;
            }
            return count;
        }

        private static ILogger l = Log.Get();
        #endregion

        #region INotifyCollectionChanged Members

		private event NotifyCollectionChangedEventHandler collectionChangedNonGeneric;

		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged {
			add { collectionChangedNonGeneric += value; }
			remove { collectionChangedNonGeneric -= value; }
		}
#if !AOT
        public event NotifyCollectionChangedHandler<T> CollectionChanged;
#else
				public event NotifyCollectionChangedHandler<T> CollectionChanged {
			add {
				if(CollectionChanged_event == null) {
					collectionChangedNonGeneric += OnNGCollectionChanged;
				}
				CollectionChanged_event += value;
			}
			remove {
				CollectionChanged_event -= value;
				if(CollectionChanged_event == null) {
					collectionChangedNonGeneric -= OnNGCollectionChanged;
				}
			}
		}

		NotifyCollectionChangedHandler<T> CollectionChanged_event;

		private void OnNGCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
#if !AOT // AOTDEBUG TEMP FIXME
			var gArgs = new NotifyCollectionChangedEventArgs<T>();
			gArgs.Action = args.Action;
			if(args.NewItems != null)
				gArgs.NewItems = args.NewItems.OfType<T>().ToArray();
			if(args.OldItems != null)
				gArgs.OldItems = args.OldItems.OfType<T>().ToArray();

			CollectionChanged_event(gArgs);
#endif
		}
#endif



        #endregion

        #region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            RaisePropertyChangedEvent(new PropertyChangedEventArgs(name));
        }

		private void RaisePropertyChangedEvent(PropertyChangedEventArgs e)
		{
			var ev = PropertyChanged;

			if(ev != null) {
				foreach(PropertyChangedEventHandler del in ev.GetInvocationList()) {
					if(del.Target as DispatcherObject != null) {
						DispatcherObject dispatcherObject = (DispatcherObject)del.Target;
						if(dispatcherObject.Dispatcher != null && !dispatcherObject.Dispatcher.CheckAccess()) {
							dispatcherObject.Dispatcher.Invoke(del, new object[] { this, e });
							continue;
						}
					}
					del.Invoke(this, e);
				}
			}
		}
        //private void RaisePropertyChangedEvent(PropertyChangedEventArgs e)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, e);
        //    }
        //}

		private delegate void RaisePropertyChangedEventHandler(PropertyChangedEventArgs e);
        #endregion

		public void AddRange(IEnumerable<T> items)
		{
			//bool disabledEvents = EventsEnabled.Value;
			//if (disabledEvents)
			//{
			//    EventsEnabled.Value = false;
			//}

			//RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item)); -OLD?

			foreach(T item in (IEnumerable)items) {
				list.Add(item);
			}

			//if (disabledEvents)
			//{
			//    EventsEnabled.Value = true;
			// FUTURE: Raise batched event:
			// RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item)); -OLD?
			//}
		}
        #region ICollection<T> Members

		public void Add(T item)
		{
			//RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
			list.Add(item);
		}

		public void Clear()
		{
#if true // Send an event that contains removed items

			// REVIEW - This may speed up a case in deselecting units in the HUD, although this may not be the best approach. (Keep a list of active items in the HUD?

			if(list.Count == 0) {
				list.Clear();
				return;
			}

			EventsEnabled.Value = false;
            
            //IList oldItems = list.ToList<T>();
			try {
				list.Clear();
			} finally {
				EventsEnabled.Value = true;
			}
            //OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems));
            OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
#else
                   //// REVIEW - send a copy of the list??
                //// REVIEW - Reset event?
                //RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, null,list)); 
            list.Clear();
#endif

		}

		public bool Contains(T item)
		{
			return list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		public int Count {
			get { return list.Count; }
		}

		public bool IsReadOnly {
			get { return list.IsReadOnly; }
		}

		public bool Remove(T item)
		{
			//RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
			return list.Remove(item);
		}
        #endregion

        #region IList<T> Members

		public int IndexOf(T item)
		{
			return list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			//RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
			list.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			//throw new NotImplementedException("No event support yet");
			list.RemoveAt(index);
		}

		public T this[int index] {
			get {
				return list[index];
			}
			set {
				list[index] = value;
			}
		}
        #endregion

        #region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}
        #endregion

        #region ICollection Members

        //void ICollection.CopyTo(Array array, int index)
        //{
        //    ((ICollection)list).CopyTo(array, index);
        //}

        //int ICollection.Count
        //{
        //    get { return ((ICollection)list).Count; }
        //}

        //bool ICollection.IsSynchronized
        //{
        //    get { return ((ICollection)list).IsSynchronized; }
        //}

        //object ICollection.SyncRoot
        //{
        //    get { return ((ICollection)list).SyncRoot; }
        //}

        #endregion

        #region IList Members

        //int IList.Add(object value)
        //{
        //    //var evList = new ArrayList();
        //    //evList.Add(value);
        //    //RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, evList, null));

        //    return ((IList)list).Add(value);
        //}

        //void IList.Clear()
        //{
        //    ((IList)list).Clear();
        //}

        //bool IList.Contains(object value)
        //{
        //    return ((IList)list).Contains(value);
        //}

        //int IList.IndexOf(object value)
        //{
        //    return ((IList)list).IndexOf(value);
        //}

        //void IList.Insert(int index, object value)
        //{
        //    //throw new NotImplementedException("No event support yet");
        //    ((IList)list).Insert(index, value);
        //}

        //bool IList.IsFixedSize
        //{
        //    get { return ((IList)list).IsFixedSize; }
        //}

        //bool IList.IsReadOnly
        //{
        //    get { return ((IList)list).IsReadOnly; }
        //}

        //void IList.Remove(object value)
        //{
        //    //var evList = new ArrayList();
        //    //evList.Add(value);
        //    //RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, null, evList));

        //    ((IList)list).Remove(value);
        //}

        //void IList.RemoveAt(int index)
        //{
        //    //throw new NotImplementedException("No event support yet");
        //    ((IList)list).RemoveAt(index);
        //}

        //object IList.this[int index]
        //{
        //    get
        //    {
        //        return ((IList)list)[index];
        //    }
        //    set
        //    {
        //        ((IList)list)[index] = value;
        //    }
        //}

        #endregion

        #region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((System.Collections.IEnumerable)list).GetEnumerator();
		}
        #endregion

        #region Operators

#if !AOT
		public static implicit operator MultiBindableCollection<T>(T[] array)
		{
			return new MultiBindableCollection<T>(array.ToList());
		}
#endif
        #endregion

        #region Subtypes



        #endregion

        #if !AOT
		public INotifyingList<FilterType> Filter<FilterType>()
		{
			//    return Filter<FilterType, T>();
			//}

			//public INotifyingList<FilterType> Filter<FilterType, DerivedType>()
			//    where DerivedType : FilterType
			//{
			if(!typeof(FilterType).IsAssignableFrom(typeof(T))) {
				throw new ArgumentException("FilterType must be assignable from this collection type.");
			}

			Type type = typeof(MultiBindableCollectionFilter<,>).MakeGenericType(typeof(FilterType), typeof(T));

			return (INotifyingList<FilterType>)Activator.CreateInstance(type, this);

			//return new MultiBindableCollectionFilter<FilterType, T>(this);
		}
#endif

        public void Set(T value)
        {
            Clear(); // TODO: Replace in one method
            Add(value);
        }
        public void Set(IEnumerable<T> value)
        {
            Clear(); // TODO: Replace in one method
            AddRange(value);
        }
    }

#if !AOT // INotifyingList<>
    /// <summary>
    /// TODO: More Support for upcast failures without throwing InvalidCastExceptions
    /// TODO: Use Implicit interface implementations to avoid repetition.
    /// </summary>
    /// <typeparam name="BaseT"></typeparam>
    /// <typeparam name="DerivedT"></typeparam>
	public class MultiBindableCollectionFilter<BaseT, DerivedT> : INotifyingList<BaseT>, INotifyCollectionChanged
        where DerivedT : class, BaseT
	{
		INotifyingList<DerivedT> target;

		public MultiBindableCollectionFilter(INotifyingList<DerivedT> target)
		{
			if(!typeof(BaseT).IsAssignableFrom(typeof(DerivedT))) {
				throw new ArgumentException("FilterType must be assignable from this collection type.");
			}

#if SanityChecks
			if(typeof(BaseT) == typeof(DerivedT)) {
				System.Diagnostics.Trace.WriteLine("typeof(BaseT) == typeof(DerivedT) - this is likely pointless and may be unintended. Type: " + typeof(BaseT).FullName);
			}
#endif

			this.target = target;
		}

		INotifyingList<FilterType> INotifyingList<BaseT>.Filter<FilterType>()
		{
			return target.Filter<FilterType>();
		}
        #region IEnumerable

		IEnumerator<BaseT> IEnumerable<BaseT>.GetEnumerator()
		{
			return target.Cast<BaseT>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)target).GetEnumerator();
		}
        #endregion

        #region INotifyCollectionChanged

		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged {
			add { ((INotifyCollectionChanged)target).CollectionChanged += value; }
			remove { ((INotifyCollectionChanged)target).CollectionChanged -= value; }
		}
        #endregion

        #region INotifyCollectionChanged<BaseT>

#if !AOT
        event NotifyCollectionChangedHandler<BaseT> INotifyCollectionChanged<BaseT>.CollectionChanged
        {
            add
            {
                if (derivedCollectionChanged == null)
                {
                    ((INotifyCollectionChanged<DerivedT>)target).CollectionChanged += new NotifyCollectionChangedHandler<DerivedT>(target_CollectionChanged);
                }
                collectionChanged += value;
            }
            remove
            {
                collectionChanged -= value;
                if (derivedCollectionChanged == null)
                {
                    ((INotifyCollectionChanged<DerivedT>)target).CollectionChanged -= new NotifyCollectionChangedHandler<DerivedT>(target_CollectionChanged);
                }
            }
        } event NotifyCollectionChangedHandler<BaseT> collectionChanged;
#endif
        		void target_CollectionChanged(NotifyCollectionChangedEventArgs<DerivedT> e)
		{
#if !AOT
			
            var ev = collectionChanged;
            if (ev == null) { return; }

            var newE = new NotifyCollectionChangedEventArgs<BaseT>();
            newE.Action = e.Action;

            if (e.OldItems != null)
            {
                newE.OldItems = e.OldItems.Cast<BaseT>().ToArray();
            }
            if (e.NewItems != null)
            {
                newE.NewItems = e.NewItems.Cast<BaseT>().ToArray();
            }

            ev(newE);
#endif
		}

		event NotifyCollectionChangedHandler<DerivedT> derivedCollectionChanged;
        #endregion

        #region ICollection<>

		void ICollection<BaseT>.Add(BaseT item)
		{
			target.Add((DerivedT)item);
		}

		public int Count {
            //get { return ((ICollection<BaseT>)target).Count; }
			get { return this.Count(); }
		}

		public void Clear()
		{
			target.Clear();
		}

		public void CopyTo(BaseT[] array, int arrayIndex)
		{
			throw new NotImplementedException();
			//target.OfType<BaseT>().ToArray();
		}

		public bool Contains(BaseT item)
		{
			DerivedT derived = item as DerivedT;
			if(item != null && derived == null)
				return false;
			return ((ICollection<BaseT>)target).Contains(derived);
		}

		public bool IsReadOnly {
			get { return ((ICollection<BaseT>)target).IsReadOnly; }
		}

		public BaseT[] ToArray()
		{
			return target.Cast<BaseT>().ToArray();
		}

		bool ICollection<BaseT>.Remove(BaseT item)
		{
			return target.Remove((DerivedT)item);
		}
        #endregion

        #region IList<>

		int IList<BaseT>.IndexOf(BaseT item)
		{
			return target.IndexOf((DerivedT)item);
		}

		void IList<BaseT>.Insert(int index, BaseT item)
		{
			target.Insert(index, (DerivedT)item);
		}

		void IList<BaseT>.RemoveAt(int index)
		{
			target.RemoveAt(index);
		}

		public BaseT this[int index] {
			get {
				return ((IList<BaseT>)target)[index];
			}
			set {
				((IList<BaseT>)target)[index] = (DerivedT)value;
			}
		}
        #endregion

	}
#endif
}
