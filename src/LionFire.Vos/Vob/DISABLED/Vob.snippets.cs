//#define VosUnitype
#define ConcurrentHandles
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Vos
{
    public partial class Vob
    {
#if TOPORT
#if !UNITY
        /// <summary>
        /// Used to get children?
        /// </summary>
        public IEnumerable<R<MountHandleObject>> Handles => ReadHandles.Concat(handles.Values.OfType<R<MountHandleObject>>()).Distinct();
#else
        public IEnumerable<H> Handles {
            get {
                throw new NotImplementedException("Not AOT safe?");
                //return ReadHandles.Concat(handles.Values).Distinct();
            }
        }
#endif
#endif

#if UNUSED // Is this a good idea?
        /// <returns>Existing handle (should be used instead), or the specified handle if none existed</returns>
        internal Voc<T> TrySetVoc<T>(Voc<T> voc)
            where T : class, new()
        {
            // OPTIMIZE - Create a parallel generic version
            return (Voc<T>)TrySetVoc(voc, typeof(T));
        }

        /// <returns>Existing voc (should be used instead)</returns>
        internal IVoc TrySetVoc(IVoc voc, Type type)
        {
            lock (vocsLock)
            {
                if (vocs.ContainsKey(type))
                {
                    throw new AlreadySetException();
                    //var existing = vocs[T1];
                    //l.Warn("Non-authoritative VobHandle detected for type " + T1.Name + " at " + this + "");
                    //existing.MergeWith(voc);
                    ////vocs[T1] = handle;
                    //return existing;
                }
                else if (voc.GetType() != typeof(Voc<>).MakeGenericType(type))
                {
                    throw new ArgumentException("voc.GetType() != typeof(Voc<>).MakeGenericType(T1)");
                    //var newVoc = _CreateAndAddVoc(T1);
                    //newVoc.MergeWith(voc);
                    //return newVoc;
                }
                else
                {
                    vocs.Add(type, voc);
                    return voc;
                }
            }
        }
#endif


#if false // Children is always present because the assumption is there is only one Vob instance representing a path
        public bool CacheChildren
        {
            get { return children != null; }
            set
            {
                if (value == CacheChildren) return;
                if (value)
                {
                    children = new MultiBindableDictionary<string, WeakReferenceX<Vob>>();
                }
                else
                {
                    children = null;
                }
            }
        }
#endif

#if NonGeneric
        /// <returns>Existing handle (should be used instead)</returns>
        internal IVobHandle TrySetHandle(IVobHandle handle, Type T1)
        {
#if ConcurrentHandles
            return handles.AddOrUpdate(T1, handle, (x, y) => handle);
#else
            lock (handlesLock)
            {
                if (handles.ContainsKey(T1))
                {
                    var existing = handles[T1];
                    l.Warn("Non-authoritative VobHandle detected for type " + T1.Name + " at " + this + "");
                    existing.MergeWith(handle);
                    //handles[T1] = handle;
                    return existing;
                }
                else if (handle.GetType() != typeof(VobHandle<>).MakeGenericType(T1))
                {
                    var newHandle = _CreateAndAddHandle(T1);
                    newHandle.MergeWith(handle);
                    return newHandle;
                }
                else
                {
                    handles.Add(T1, handle);
                    return handle;
                }
            }
#endif
        }
#endif

#if !ConcurrentHandles
        private IVobHandle _CreateAndAddHandle(Type T1)
        {
            IVobHandle vh = _CreateHandle(T1);

            handles.Add(T1, vh);
            return vh;
        }
#endif

        /// <returns>Existing handle (should be used instead), or the specified handle if none existed</returns>
        //internal VobHandle<TValue> TrySetHandle<TValue>(VobHandle<TValue> handle) => (VobHandle<TValue>)handles.AddOrUpdate(typeof(TValue), handle, (existingType, existingValue) => existingValue); 
        // OPTIMIZE - Create a parallel generic version//return (VobHandle<T1>)TrySetHandle(handle, typeof(T1));


#if NonGeneric
        public IVobHandle GetHandle(Type T1)
        {
#if ConcurrentHandles
            return handles.GetOrAdd(T1, t => _CreateHandle(t));
#else
            lock (handlesLock)
            {
                if (handles.ContainsKey(T1))
                {
                    return (IVobHandle)handles[T1];
                }
                else
                {
                    return _CreateAndAddHandle(T1);
                }
            }
#endif
        }
#endif

        #region Experiment: handles with alternative equality comparer

        //private ConcurrentDictionary<Type, IVobHandle> handles = new ConcurrentDictionary<Type, IVobHandle>(new zEC());

        //private class zEC : IEqualityComparer<Type>
        //{
        //    //public int Compare(Type x, Type y)
        //    //{
        //    //    //if(x.Equals(y))return0;
        //    //    return x.FullName.CompareTo(y.FullName);
        //    //}
        //    public bool Equals(Type x, Type y) => x == y;

        //    public int GetHashCode(Type obj) => obj.GetHashCode();
        //}

        #endregion

#if !ConcurrentHandles
        private object handlesLock = new object();
        //private SortedDictionary<Type, IVobHandle> handles = new SortedDictionary<Type, IVobHandle>(new zC()); // REVIEW - why the comparer?  AOT Debug maybe?


        class zC : IComparer<Type>
        {
            public int Compare(Type x, Type y)
            {
                //if(x.Equals(y))return0;
                return x.FullName.CompareTo(y.FullName);
            }
        }
#endif

#if !ConcurrentHandles
        public VobHandle<T> GetHandle<T>()
        {
            lock (handlesLock)
            {
                if (handles.ContainsKey(typeof(T1)))
                {
                    return (VobHandle<T1>)handles[typeof(T1)];
                }
                else
                {
                    var vh = new VobHandle<T1>(this);
                    try
                    {
                        handles.Add(typeof(T1), vh);
                    }
                    catch (Exception ex)
                    {
                        l.Error("Exception in adding handle.  " + ex.ToString());
                        if (handles.ContainsKey(typeof(T1)))
                        {
                            return (VobHandle<T1>)handles[typeof(T1)];
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return vh;
                }
            }
        }
#endif

        #region Object State

        //[Ignore]
        //protected List<H> objectHandles; UNUSED

        #endregion

        #region Object Uni-Type vs IReadOnlyMultiTyped support

        #region Uni Type
#if VosUnitype
        
        //public H Handle { get { return UnitypeHandle; } }


        #region ObjectHandle
        /// <summary>
        /// Target handle.  Could be another Vob, if this location is a virtual one, or this could 
        /// be a handle to a file or database object.
        /// REVIEW - This isn't handled very well
        /// </summary>
        [Ignore]
        public IVobHandle UnitypeHandle {
            get { return _unitypeHandle; }
            set {
                if (_unitypeHandle == value) return;
                if (_unitypeHandle != null && value != null)
                {
                    l.Warn("REVIEW: _objectHandle changing to " + value + " from " + _unitypeHandle + " for " + this);
                }
                _unitypeHandle = value;
            }
        }
        private IVobHandle _unitypeHandle;

        private object _object {
            get {
                var h = UnitypeHandle;
                if (h != null) return h.HasObject ? h.Object : null;
                return null;
            }
        }

        #endregion


        //public Mount ObjectHandleMount
        //{
        //    get
        //    {
        //        if (objectHandleMount == null)
        //        {
        //            if (UnitypeHandle != null)
        //            {
        //                objectHandleMount = GetMountForHandle(UnitypeHandle);
        //            }
        //        }
        //        return objectHandleMount;
        //    }
        //    private set
        //    {
        //        objectHandleMount = value;
        //    }
        //} private Mount objectHandleMount;

        #region Type

        public Type PrimaryType {
            get { return UnitypeHandle == null ? null : UnitypeHandle.Type; }
            //get { return type; }
            //set
            //{
            //    if (type == value) return;
            //    if (type != null)
            //    {
            //        l.Warn("REVIEW: Vob type changing to " + value + " for " + this);
            //    }
            //    type = value;
            //}
        }// private Type type;

        #endregion
#endif
        #endregion

        ///// <summary>
        ///// FUTURE: Multiple OBase layers can contribute to a compound virtual object:
        ///// </summary>
        //private bool IsObjectVirtualMultiTyped = false; // UNUSED

        ///// <summary>
        ///// IDEA: When an object comes from multiple sources, merge (flatten) multitype objects
        ///// I'm not sure how this would work since VobHandles work with only one type and 
        ///// Vobs no longer work with any object directly.
        ///// </summary>
        //private bool IsMultiTypeObjectMerged = false; // UNUSED

        #endregion

#if OBSOLETED
        /// <summary>
        /// REVIEW - Does this make sense?  Need to rename this to TryDeleteAllHandlesFromReference, and then do a Query or something.
        /// </summary>
        /// <param name="preview"></param>
        /// <returns></returns>
        public bool TryDelete(bool preview = false)
        {
            throw new NotImplementedException();
            // Does this make sense?  
            bool deletedSomething = false;

            foreach (H handle in
#if AOT
 (IEnumerable)
#endif
 WriteHandles)
            {
                deletedSomething |= handle.TryDelete(/*preview: preview*/);
            }

            // FUTURE: Delete based on cached loaded objects only: (may require sync to make sure it's up to date)
            //if (IsObjectSynced)
            //{
            //if (objectHandle != null)
            //{
            //    deletedSomething |= objectHandle.TryDelete();

            //    return result;
            //}
            //else if (objectHandles != null && objectHandles.Count != 0)
            //{
            //}
            //else
            //{
            //}
            //}

            if (deletedSomething)
            {
                OnDeleted();
            }

            //objectHandle = null;
            //objectHandles = null;
            return deletedSomething;
        }


        public void Delete()
        {
            bool deletedSomething = false;

            foreach (H handle in
#if AOT
 (IEnumerable)
#endif
 WriteHandles)
            {
                deletedSomething |= handle.TryDelete();
            }

            //objectHandle = null;
            //objectHandles = null;

            if (deletedSomething)
            {
                OnDeleted();
            }
            else
            {
                throw new ObjectNotFoundException("Delete failed: no object deleted");
            }
        }

        protected void OnDeleted()
        {
            //IsPersisted = false;
        }
#endif

    }

}
