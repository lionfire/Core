#define ConcurrentHandles
#define WARN_VOB
//#define INFO_VOB
#define TRACE_VOB

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Assets;
using LionFire.Collections;
using LionFire.Extensions.ObjectBus;
using LionFire.Instantiating;
using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Structures;
using LionFire.Types;
using LionFire.Vos;
using Microsoft.Extensions.Logging;

namespace LionFire.Vos
{

    //public class FilteredVob
    //{
    //    // Filtered notifying collection?
    //    // Filtered LionBinding collection binding?
    //}

    // ----- Mount notes -----
    //        //public INotifyingReadOnlyCollection<Mount> Mounts { get { return mounts; } }
    //        ////private MultiBindableDictionary<string, Mount> mountsByPath = new MultiBindableDictionary<string, Mount>();
    //        //private MultiBindableCollection<Mount> mounts = new MultiBindableCollection<Mount>();

    //        // Disk location: ValorDir/DataPacks/Official/Valor1/Core.zip
    //        // /Valor/Packages/Nextrek/Maps/Bronco.Map
    //        // /Valor/Packages/Nextrek/Maps/Bronco/Settings/Vanilla
    //        // /Valor/Packages/Nextrek/Maps/Bronco/Settings/INL
    //        // Disk location: ValorDir/DataPacks/Official/Expansion1.zip
    //        // Disk location: ValorDir/DataPacks/Official/Maps/MapPack1.zip
    //        // /Valor/Packages/Nextrek/Maps/Chaos.Map
    //        // /Valor/Packages/Nextrek/Maps/Chaos/Settings/Cibola
    //        // Disk location: ValorDir/DataPacks/Official/Maps/MapPack2.zip

    //        // Disk location: ValorDir/DataPacks/Official/Expansion1/Core.zip
    //        // Disk location: ValorDir/DataPacks/Official/Expansion2/Core.zip

    //        // Disk location: ValorDir/DataPacks/Downloaded/Vanilla2.zip
    //        // Disk location: ValorDir/DataPacks/Mods/TAMod/Core.1.0.zip
    //        // Disk location: ValorDir/DataPacks/Mods/TAMod/Core.1.1.zip
    //        // Disk location: ValorDir/DataPacks/Mods/TAMod/MapPack1.zip

    //        // /Valor/Packages/Nextrek/Maps/Bronco/Settings/Vanilla2

    //        // Data Packs Menu:
    //        //   - Core
    //        //      - Base
    //        //   - Custom
    //        //      - Vanilla2

    //        // Disk location: ValorDir/Data (rooted at Valor/Packages)/Nextrek/Maps/Bronco/Settings/Vanilla3

    //        // Disk location: ValorDir/Data

    //        // 3 mounts:
    //        // Valor/Data | file:///c:/Program Files/LionFire/Valor/Data
    //        // Valor/Data | file:///c:/Program Files/LionFire/Valor/DataPacks

    // ---------------- Other notes
    //    // vos://host/path/to/node%Type
    //    // vos://host/path/to/.node.Type.ext
    //    // vos://host/path/to/node%Type[instanceName]
    //    // vos://host/path/to/node/.TYPEs/instanceName
    //    // vos://host/path/to/node%Type[] - all instances, except main instance


    /// <summary>
    /// 
    /// VOB:
    ///  knows where it is (has reference)
    ///  keeps track of its children (is an active node in a VosBase)
    ///  is invalidated when mounts change
    ///  has handles to objects in multiple layers
    ///  has handles to its multi-type objects 
    ///   - can be locked to a uni-type object
    /// 
    /// Dynamic Object.  
    /// 
    /// Brainstorm:
    /// 
    /// Dob: created automatically as a multitype object (if needed)
    ///     UnitType (references components of types)
    /// Pob: Partial object.  Object piece.  Can have subchildren
    ///       - Version (optional decorator, but may be required by app)
    ///          - Could be implemented as a field, then implement the read-only IMultiType to provide OBus access to it
    ///       - Personal notes (saved in personal layer)
    ///       
    ///  - For child objects: is it an 'is a' or 'has a' relationship?  Containment vs reference / decorators
    ///     - allow nested subobjects?????
    ///  
    ///  Anyobject support: hooray for POCO
    /// 
    ///  Can Proxies/mixins help?
    ///   - load an object, get a mixin back?  Doesn't seem so hard now that I think I've done it
    /// </summary>
    /// <remarks>
    /// Handle overrides:
    ///  - (TODO) Object set/get.
    ///    - get_Object - for Vob{T} this will return the object as T, if any.  Otherwise, it may return a single object, or a MultiType object
    ///    - set_Object - depending on the situation, it may assign into a MultiType object
    /// </remarks>
    public class Vob :
        //CachingHandleBase<Vob, Vob>,
        //CachingChildren<Vob>,
        //IHasHandle,
        //IHandle, // TODO - make this a smarter handle.  The object might be a dynamically created MultiType for complex scenarios
        IReferencable,
#if AOT
		IParented
#else
        IParented<Vob>
#endif
    {

        //public IHandle Handle { get { return UnitypeHandle; } }

        #region Object State

        //[Ignore]
        //protected List<IHandle> objectHandles; UNUSED

        #endregion

        #region Handles

        public VobHandle<T1> GetHandle<T1>()
            where T1 : class
        {
#if ConcurrentHandles
            return (VobHandle<T1>)handles.GetOrAdd(typeof(T1), t => _CreateHandle(t));
#else
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
#endif
        }

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
        private IVobHandle _CreateHandle(Type T1)
        {
            Type vhType = typeof(VobHandle<>).MakeGenericType(T1);
            return (IVobHandle)Activator.CreateInstance(vhType, this);
        }

#if !ConcurrentHandles
        private IVobHandle _CreateAndAddHandle(Type T1)
        {            
            IVobHandle vh =_CreateHandle(T1);

            handles.Add(T1, vh);
            return vh;
        }
#endif

        /// <returns>Existing handle (should be used instead), or the specified handle if none existed</returns>
        internal VobHandle<T1> TrySetHandle<T1>(VobHandle<T1> handle)
            where T1 : class
        {
            // OPTIMIZE - Create a parallel generic version
            return (VobHandle<T1>)TrySetHandle(handle, typeof(T1));
        }

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

#if ConcurrentHandles
        private ConcurrentDictionary<Type, IVobHandle> handles = new ConcurrentDictionary<Type, IVobHandle>(new zEC());

        private class zEC : IEqualityComparer<Type>
        {
            //public int Compare(Type x, Type y)
            //{
            //    //if(x.Equals(y))return0;
            //    return x.FullName.CompareTo(y.FullName);
            //}
            public bool Equals(Type x, Type y) => x == y;

            public int GetHashCode(Type obj) => obj.GetHashCode();
        }

#else
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


        #endregion

        #region Vocs

        public Voc<T1> GetSubVoc<T1>()
            where T1 : class, new()
        {
            string subpath = Voc<T1>.DefaultSubpath;
            return this[subpath].GetVoc<T1>();
        }

        public Voc<T1> GetVoc<T1>()
            where T1 : class, new()
        {
            lock (vocsLock)
            {
                if (vocs.ContainsKey(typeof(T1)))
                {
                    return (Voc<T1>)vocs[typeof(T1)];
                }
                else
                {
                    var vh = new Voc<T1>(this);
                    try
                    {
                        vocs.Add(typeof(T1), vh);
                    }
                    catch (Exception ex)
                    {
                        l.Error("Exception in adding voc.  " + ex.ToString());
                        if (vocs.ContainsKey(typeof(T1)))
                        {
                            return (Voc<T1>)vocs[typeof(T1)];
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

        public IVoc GetVoc(Type T1)
        {
            lock (vocsLock)
            {
                if (vocs.ContainsKey(T1))
                {
                    return vocs[T1];
                }
                else
                {
                    return _CreateAndAddVoc(T1);
                }
            }
        }
        private IVoc _CreateAndAddVoc(Type T1)
        {
            Type vhType = typeof(Voc<>).MakeGenericType(T1);
            IVoc vh = (IVoc)Activator.CreateInstance(vhType, this);
            vocs.Add(T1, vh);
            return vh;
        }

        /// <returns>Existing handle (should be used instead), or the specified handle if none existed</returns>
        internal Voc<T1> TrySetVoc<T1>(Voc<T1> voc)
            where T1 : class, new()
        {
            // OPTIMIZE - Create a parallel generic version
            return (Voc<T1>)TrySetVoc(voc, typeof(T1));
        }

        /// <returns>Existing voc (should be used instead)</returns>
        internal IVoc TrySetVoc(IVoc voc, Type T1)
        {
            lock (vocsLock)
            {
                if (vocs.ContainsKey(T1))
                {
                    throw new AlreadySetException();
                    //var existing = vocs[T1];
                    //l.Warn("Non-authoritative VobHandle detected for type " + T1.Name + " at " + this + "");
                    //existing.MergeWith(voc);
                    ////vocs[T1] = handle;
                    //return existing;
                }
                else if (voc.GetType() != typeof(Voc<>).MakeGenericType(T1))
                {
                    throw new ArgumentException("voc.GetType() != typeof(Voc<>).MakeGenericType(T1)");
                    //var newVoc = _CreateAndAddVoc(T1);
                    //newVoc.MergeWith(voc);
                    //return newVoc;
                }
                else
                {
                    vocs.Add(T1, voc);
                    return voc;
                }
            }
        }

        private readonly object vocsLock = new object();
        private SortedDictionary<Type, IVoc> vocs = new SortedDictionary<Type, IVoc>(); // TODO: Make ConcurrentDictionary

        #endregion

        #region Object Uni-Type vs IMultiTyped support

        #region Uni Type
#if VosUnitype

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

        #region Misc MOVE

        //public Vob<T> AsVobType<T>() // TODO REVIEW FUTURE Use ((IMultiTyped)this).AsType{T}() instead
        //    where T : class, new()
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        #region Ontology

        #region Vos

        public VBase Vos => vos;
        private readonly VBase vos;

        #endregion

        #region IParented

#if AOT
        object IParented.Parent { get { return this.Parent; } set { throw new NotSupportedException(); } }
#endif

        #region Parent

        public Vob Parent {
            get => parent;
            set => throw new NotSupportedException();
        }
        private readonly Vob parent;

        #endregion

        #endregion

        public string Name => name;
        private readonly string name;

        public string Path => path;
        private readonly string path;

        public IEnumerable<string> PathElements
        {
            get
            {
                if (Parent != null)
                {
                    foreach (var pathElement in Parent.PathElements)
                    {
                        yield return pathElement;
                    }
                }

                if (!String.IsNullOrEmpty(Name))
                {
                    yield return Name;
                }
                else
                {
                    if (Parent == null)
                    {
                        // yield nothing
                    }
                    else
                    {
                        yield return null; // Shouldn't happen.
                        //yield return String.Empty; // REVIEW - what to do here?
                    }
                }
            }
        }

        public IEnumerable<string> PathElementsReverse
        {
            get
            {
                yield return Name;

                if (Parent != null)
                {
                    foreach (var pathElement in Parent.PathElementsReverse)
                    {
                        yield return pathElement;
                    }
                }
            }
        }

        #endregion

        #region Construction

        public Vob(VBase vos, Vob parent, string name)
        {
            if (vos == null)
            {
                throw new ArgumentNullException("vos");
            }

            if (GetType() == typeof(RootVob))
            {
                if (!String.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("name must be null or empty for root");
                }
            }
            else
            {
                if (parent == null)
                {
                    throw new ArgumentNullException("parent must be set for all non-RootVob Vobs.");
                }

                if (name == null)
                {
                    throw new ArgumentNullException("name must not be null for non-root");
                }
            }

            //if (StringX.IsNullOrWhiteSpace(name) && this.GetType() != typeof(RootVob)) throw new ArgumentNullException("Name must be set for all non-RootVob Vobs.");

            this.vos = vos;
            this.parent = parent;
            this.name = name;

            path = LionPath.CleanAbsolutePathEnds(LionPath.Combine((parent == null ? "" : parent.Path), name));
            VobDepth = LionPath.GetAbsolutePathDepth(path);

            InitializeEffectiveMounts();
            //MountStateVersion = -1;
        }

        //public Vob(Vos vos, Vob parent, string name, VosReference reference)
        //    : this(vos, parent, name)
        //{
        //    this.Reference = reference;
        //}

        #endregion

        #region Properties

        public enum VobSettings
        {
            Unspecified = 0,
            DeepOverlay = 1 << 0, // FUTURE?
            DisallowDeepOverlay = 1 << 1, // FUTURE?
        }

        #endregion

        #region IReferencable

        #region Reference

        public string Key => VosReference.Key;

        public VosReference VosReference
        {
            get
            {
                if (vosReference == null)
                {
                    vosReference = new VosReference(path);
                }
                return vosReference;
            }
            set
            {
                if (vosReference == value)
                {
                    return;
                }

                if (vosReference != default(IReference))
                {
                    throw new NotSupportedException("Reference can only be set once.  To relocate, use the Move() method.");
                }

                vosReference = value;
            }
        }
        private VosReference vosReference;

        public IReference Reference // TODO MEMORYOPTIMIZE: I think a base class has an IReference field
        {
            get => VosReference;
            set
            {
                if (value == null) { VosReference = null; return; }
                VosReference vr = value as VosReference;
                if (vr != null)
                {
                    VosReference = vr; return;
                }
                else
                {
                    //new VosReference(value); // FUTURE: Try converting
                    throw new ArgumentException("Reference for a Vob must be VosReference");
                }
            }
        }

        #endregion

        #endregion

        #region Mounts

        #region Mounts Collection

        #region HasMounts

        public bool HasMounts
        {
            get => hasMounts;
            private set
            {
                if (hasMounts == value)
                {
                    return;
                }

                hasMounts = value;
                if (hasMounts)
                {
                    vos.VobsWithMounts.Add(this);
                }
                else
                {
                    vos.VobsWithMounts.Remove(this);
                }
                InitializeEffectiveMounts();
            }
        }
        private bool hasMounts;

        private void UpdateHasMounts() => HasMounts = mounts != null && mounts.Count > 0;

        #endregion



        internal MultiBindableDictionary<string, Mount> Mounts
        {
            get
            {

                if (mounts == null)
                {
                    mounts = new MultiBindableDictionary<string, Mount>();
                    mounts.CollectionChanged += new NotifyCollectionChangedHandler<ObjectBus.Mount>(OnMountsCollectionChanged);
                    // MEMOPTIMIZE: Attach events.  Dispose dictionary when all are unmounted
                }
                return mounts;
            }
        }
        private MultiBindableDictionary<string, Mount> mounts;

        private readonly object mountsLock = new object();

        #endregion

        #region (internal) Mount method

        /// <summary>
        /// To mount create a new instance of Mount and set IsEnabled to true.
        /// </summary>
        /// <param name="mount"></param>
        internal void Mount(Mount mount)
        {
            {
                var ev = Mounting;
                if (ev != null)
                {
                    var args = new CancelableEventArgs<Mount>(mount);
                    ev(this, args);
                    if (args.IsCanceled)
                    {
                        return;
                    }
                }
            }

            lock (mountsLock)
            {
                // TODO EVENTS: mounting/mounted
                try
                {
                    Mounts.Add(mount);
                }
                catch (Exception ex)
                {
                    Log.Info("Failed to mount with key " + Mounts.GetKey(mount) + " for " + mount + " " + ex);
                }
            }
        }

        #endregion

        #region Unmount methods

        private void _unmount(string mountKey, Mount mount)
        {
            if (mountKey != mount.Root.Key)
            {
                throw new ArgumentException("mountName mismatch for mount");
            }

            {
                var ev = Unmounting;
                if (ev != null)
                {
                    var args = new CancelableEventArgs<Mount>(mount);
                    ev(this, args);
                    if (args.IsCanceled)
                    {
                        return;
                    }
                }
            }

            lock (mountsLock)
            {
                // Fires mounts changed event. 
                // REVIEW: Fire events outside the lock?  Fire unmounting/mounting event outside the lock?
                Mounts.Remove(mountKey);
            }
        }

        internal void Unmount(string mountKey)
        {
            Mount knownMount = Mounts.TryGetValue(mountKey);
            if (knownMount == null)
            {
                // Already unmounted, if it ever was
                return;
            }
            _unmount(mountKey, knownMount);
        }

        internal void Unmount(Mount mount)
        {
            Mount knownMount = Mounts.TryGetValue(mount.Root.Key);
            if (!System.Object.ReferenceEquals(knownMount, mount))
            {
                return;
            }

            _unmount(mount.Root.Key, knownMount);
        }

        public void UnmountAll()
        {
            foreach (var mount in Mounts.Values.ToArray())
            {
                mount.IsEnabled = false;
            }
        }

        #endregion

        #region Mount Events

        public event Action<Vob, CancelableEventArgs<Mount>> Mounting;
        public event Action<Vob, CancelableEventArgs<Mount>> Unmounting;
        public event Action<Vob, Mount> Mounted;
        public event Action<Vob, Mount> Unmounted;

        private void OnMountsCollectionChanged(NotifyCollectionChangedEventArgs<Mount> e)
        {
            InitializeEffectiveMounts(); // OPTIMIZE TEMP - overkill

            UpdateHasMounts();
            vos.OnMountsChangedFor(this, e);

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                l.Warn("[MOUNT] Mounts collection reset (TODO: Handle)");
            }
            else
            {
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        //                        l.Info("[MOUNT] " + this + " ==> " + item.RootHandle);
                        var ev = Mounted;
                        if (ev != null)
                        {
                            ev(this, item);
                        }
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        l.Info("[UNMOUNT] " + this + " ==> " + item.Root);
                        var ev = Unmounted;
                        if (ev != null)
                        {
                            ev(this, item);
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

        #region EffectiveMounts

        internal readonly int VobDepth; // REVIEW - needed only for mounts?

        private MultiValueSortedList<int, Mount> effectiveMountsByReadPriority;
        private MultiValueSortedList<int, Mount> effectiveMountsByWritePriority;
        private Dictionary<string, Mount> effectiveMountsByName;
        private bool AreEffectiveMountsInitialized => effectiveMountsByReadPriority != null;
        //private Vob FirstAncestorWithMounts;
        //private int FirstAncestorWithMountsRelativeDepth;
        //private IEnumerable<string> FirstAncestorToThisSubPath;

        private bool InitializeEffectiveMounts(bool reset = false)
        {
            if (AreEffectiveMountsInitialized && !reset)
            {
                return true;
            }

            //// TODO: OPTIMIZE: To save memory, pass the buck to the first ancestor that has a mount.
            //if (!HasMounts)
            //{
            ////    effectiveMountsByReadPriority = null;
            ////    effectiveMountsByWritePriority = null;
            ////    effectiveMountsByName = null;

            ////    Vob firstAncestorWithMounts = null;
            ////    IEnumerable<string> firstAncestorToThisSubPath = null;
            ////    for (Vob ancestor = this.Parent; ancestor != null; ancestor = ancestor.Parent)
            ////    {
            ////        if (ancestor.HasMounts)
            ////        {
            ////            firstAncestorWithMounts = ancestor;
            ////            firstAncestorToThisSubPath = this.PathElements.Skip(ancestor.VobDepth);
            ////            break;
            ////        }
            ////    }
            ////    this.FirstAncestorWithMounts = firstAncestorWithMounts;
            ////    this.FirstAncestorToThisSubPath = firstAncestorToThisSubPath;
            ////    return;
            //}
            ////else
            ////{
            ////    this.FirstAncestorWithMounts = this;
            ////    this.FirstAncestorToThisSubPath = new string[] { };
            ////}

            effectiveMountsByReadPriority = new MultiValueSortedList<int, ObjectBus.Mount>(
#if AOT
				Singleton<IntComparer>.Instance
#endif
);
            effectiveMountsByWritePriority = new MultiValueSortedList<int, ObjectBus.Mount>(
#if AOT
				Singleton<IntComparer>.Instance
#endif
);
            effectiveMountsByName = new Dictionary<string, Mount>(); // FUTURE: If this is rarely used, create on demand

            bool gotSomething = false;
            for (Vob vob = this; vob != null; vob = vob.Parent)
            {
                foreach (KeyValuePair<string, Mount> kvp in
#if AOT
                        (IEnumerable)
#endif
 vob.Mounts)
                {
                    Mount mount = kvp.Value;
                    effectiveMountsByReadPriority.Add(mount.MountOptions.ReadPriority, mount);
                    gotSomething = true;

                    if (VosConfiguration.AllowIsReadonlyOverride || !mount.MountOptions.IsReadOnly)
                    {
                        effectiveMountsByWritePriority.Add(mount.MountOptions.WritePriority, mount);
                    }
                    effectiveMountsByName.Add(mount.Root.Key, mount);
                }
            }
            if (!gotSomething)
            {
                effectiveMountsByWritePriority = null;
                effectiveMountsByReadPriority = null;
                effectiveMountsByName = null;
                //l.Trace("Got no mounts for " + this.ToString());
                return false;
            }
            return true;
        }


        public void OnAncestorMountsChanged(Vob ancestor, NotifyCollectionChangedEventArgs<Mount> e)
        {
            // TODO: Adapt changes
            InitializeEffectiveMounts(reset: true);
        }

        #endregion

        #region Mounts

        private IHandle<T> GetMountHandle<T>(Mount mount)
            where T : class
        {
            IHandle<T> result;

            // TODO TO_ASSERT mount path is a parent of this.Path

            //int mountDepthDelta = this.VobDepth - mount.VobDepth; // MICROOPTIMIZE - move to mount

            //if (mountDepthDelta == 0)

            if (Object.ReferenceEquals(mount.Vob, this))
            {
                result = mount.RootHandle.GetHandle<T>();
            }
            else
            {
                result = mount.RootHandle.GetHandle<T>(PathElements.Skip(mount.VobDepth));

                //result = mount.RootHandle[this.PathElements.Skip(mount.VobDepth)].ToHandle();
                //.GetHandle<T>(); // OPTIMIZE: cache this enumerable alongside the mount
            }

            //result.Mount = mount;
            return result;
        }


        private IHandle GetMountHandle(Mount mount)
        {
            IHandle result;

            // TODO TO_ASSERT mount path is a parent of this.Path

            //int mountDepthDelta = this.VobDepth - mount.VobDepth; // MICROOPTIMIZE - move to mount

            //if (mountDepthDelta == 0)

            if (System.Object.ReferenceEquals(mount.Vob, this))
            {
                result = mount.RootHandle;
            }
            else
            {
                result = mount.RootHandle[PathElements.Skip(mount.VobDepth)]; // OPTIMIZE: cache this enumerable alongside the mount
            }

            //result.Mount = mount;
            return result;
        }

        internal string GetMountPath(Mount mount)
        {
            string result;

            if (System.Object.ReferenceEquals(mount.Vob, this))
            {
                result = mount.Root.Path;
                l.Trace("UNTESTED: (alt) MountPath: " + result);
            }
            else
            {
                result = LionPath.Combine(mount.Root.Path, PathElements.Skip(mount.VobDepth)); // OPTIMIZE: cache this enumerable alongside the mount
                l.Trace("UNTESTED: MountPath: " + result);
            }

            return result;
        }

        #endregion

#if !UNITY
        public IEnumerable<IHandle> Handles => ReadHandles.Concat(handles.Values).Distinct();
#else
        public IEnumerable<IHandle> Handles {
            get {
                throw new NotImplementedException("Not AOT safe?");
                //return ReadHandles.Concat(handles.Values).Distinct();
            }
        }
#endif

        #region Persistence

        #region Persistence Utils

        public IEnumerable<IHandle> ReadHandles
        {
            get
            {
                if (!InitializeEffectiveMounts())
                {
                    yield break;
                }
                //if (HasMounts)
                {
                    foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
                    {
                        yield return GetMountHandle(mount);
                    }
                }
            }
        }

        //private IEnumerable<KeyValuePair<IHandle, Mount>> ReadHandlesWithMounts
        //{
        //    get
        //    {
        //        if (!InitializeEffectiveMounts()) yield break;
        //        //if (HasMounts)
        //        {
        //            foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
        //            {
        //                yield return new KeyValuePair<IHandle, Mount>(GetMountHandle(mount), mount);
        //            }
        //        }

        //    }
        //}
        private IEnumerable<Mount> ReadHandleMounts
        {
            get
            {
                if (!InitializeEffectiveMounts())
                {
                    yield break;
                }
                //if (HasMounts)
                {
                    foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
                    {
                        yield return mount;
                    }
                }

            }
        }

        public bool CanWrite => WriteHandleMounts.Where(m => !m.MountOptions.IsReadOnly).Any();

        public IEnumerable<IHandle> WriteHandles
        {
            get
            {
                if (!InitializeEffectiveMounts())
                {
                    yield break;
                }

                //if (!HasMounts) yield break;
                foreach (Mount mount in effectiveMountsByWritePriority.Values.SelectMany(x => x))
                {
                    yield return GetMountHandle(mount);
                }
            }
        }

        //private IEnumerable<KeyValuePair<IHandle, Mount>> WriteHandlesWithMounts
        //{
        //    get
        //    {
        //        if (!InitializeEffectiveMounts()) yield break;
        //        //if (HasMounts)
        //        {
        //            foreach (Mount mount in effectiveMountsByWritePriority.Values.SelectMany(x => x))
        //            {
        //                yield return new KeyValuePair<IHandle, Mount>(GetMountHandle(mount), mount);
        //            }
        //        }

        //    }
        //}
        private IEnumerable<Mount> WriteHandleMounts
        {
            get
            {
                if (!InitializeEffectiveMounts())
                {
                    yield break;
                }
                //if (HasMounts)
                {
                    foreach (Mount mount in effectiveMountsByWritePriority.Values.SelectMany(x => x))
                    {
                        yield return mount;
                    }
                }

            }
        }

        private IEnumerable<Mount> EffectiveWriteMounts
        {
            get
            {
                if (effectiveMountsByWritePriority == null) { InitializeEffectiveMounts(); }
                if (effectiveMountsByWritePriority == null) { return Enumerable.Empty<Mount>(); }
                return effectiveMountsByWritePriority.Values.SelectMany(x => x);
            }
        }
        private IHandle FirstWriteHandle // REVIEW Don't use this?
        {
            get
            {
                //if (!HasMounts) return null;
                foreach (Mount mount in
#if AOT
                        (IEnumerable)
#endif
 EffectiveWriteMounts)
                {
                    if (mount.MountOptions.IsReadOnly && !VosContext.Current.IgnoreReadonly)
                    {
                        continue;
                    }

                    return GetMountHandle(mount);
                }
                return null;
            }
        }
        private IHandle<T> GetFirstWriteHandle<T>()
            where T : class
        {
            //get
            {
                //if (!HasMounts) return null;
                foreach (Mount mount in
#if AOT
                        (IEnumerable)
#endif
 EffectiveWriteMounts)
                {
                    if (mount.MountOptions.IsReadOnly && !VosContext.Current.IgnoreReadonly)
                    {
                        continue;
                    }

                    return GetMountHandle<T>(mount);
                }
                return null;
            }
        }

        //private VobHandle<T> GetFirstWriteHandle<T>()
        //{
        //    IHandle objectHandle;
        //    objectHandle = FirstWriteHandle<T>();
        //    return objectHandle;
        //}

        private IHandle GetFirstWriteHandle()
        {
            IHandle objectHandle;

            //if (package == null && layer == null)
            {
                objectHandle = FirstWriteHandle;
                return objectHandle;
            }
            //            else
            //            {
            //                if (package != null && layer != null)
            //                {
            //                    string mountName = LionFire.Vos.Mount.GetMountName(package, layer);
            //                    Mount mount = effectiveMountsByName.TryGetValue(mountName);
            //                    if (mount.MountOptions.IsReadOnly)
            //                    {
            //                        l.Trace("GetFirstWriteHandle found mount by name but it IsReadOnly");
            //                        return null;
            //                    }
            //                    else
            //                    {
            //                        return GetMountHandle(mount);
            //                    }
            //                }
            //                else
            //                {
            //                    if (package != null)
            //                    {
            //                        return GetMountHandle(EffectiveWriteMounts.Where(m => m.PackageName == package).FirstOrDefault());
            //                    }
            //                    else // layer != null
            //                    {
            //#if SanityChecks
            //                        if (layer == null) throw new UnreachableCodeException("layer != null");
            //#endif
            //                        return GetMountHandle(EffectiveWriteMounts.Where(m => m.LayerName == layer).FirstOrDefault());
            //                    }
            //                }
            //            }
        }



        #endregion

        #region Delete

        //public DeleteMode DeleteMode { get; set; }
        //enum DeleteMode // FUTURE
        //{
        //    Unspecified = 0,
        //    DeleteRetrieved,
        //    //DeleteOne, - silly?
        //    DeleteAll,
        //}

        public bool TryDelete(bool preview = false)
        {
            bool deletedSomething = false;

            foreach (IHandle handle in
#if AOT
 (IEnumerable)
#endif
 WriteHandles)
            {
                deletedSomething |= handle.TryDelete(preview: preview);
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

            foreach (IHandle handle in
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

        #endregion

        #region Save

        public VobHandle<T> SaveObject<T>(T obj)
            where T : class
        {
            IVobHandle vhObj = obj as IVobHandle;
            VobHandle<T> actualVH;

            #region IVobHandle

            if (vhObj != null)
            {
                throw new ArgumentException("Use Save method to save VobHandles.");
                //Type vhType = vhObj.Type;
                //if (vhType != typeof(T))
                //{

                //}
                //actualVH = (VobHandle<T>)TrySetHandle(vhObj, vhType);


                ////VobHandle<T> vhGen = vhObj as VobHandle<T>;
                ////if(vhGen==null)
                ////{
                ////    l.TraceWarn("Unexpected vobHandle type, not attempting to use it as an authoritative VobHandle for this Vob: " + obj.GetType().FullName + " (" + this.ToString()+ ")");
                ////    var realVHGen = this.GetHandle<T>(); 
                ////    realVHGen
                ////}

                ////if (this.UnitypeHandle == null) 
                ////{
                ////    this.UnitypeHandle = vhObj; 
                ////}

                ////VobHandle<T> vhGen = this.UnitypeHandle as VobHandle<T>;
                ////if (vhGen != null)
                ////{
                ////    var authoritativeHandle = this.TrySetHandle<T>(vhGen);

                ////    return authoritativeHandle;
                ////}
                ////else

            }
            #endregion
            else
            {
                //VobHandle<T> h = this.UnitypeHandle as VobHandle<T>;
                actualVH = GetHandle<T>();
                actualVH.Object = obj;
            }

#if AOT
            Save(h, typeof(T));
#else
            Save<T>(actualVH);
#endif

#if VosUnitype
            if (UnitypeHandle == null)
            {
                UnitypeHandle = actualVH;
            }
#endif
            return actualVH;
        }



#if AOT
		public void Save<T>(VobHandle<T> vosLogicalHandle)
			where T : class
		{
			Save(vosLogicalHandle, typeof(T));

		}
        public void Save(IVobHandle vosLogicalHandle, Type T, bool preview = false)
		{
			if(Path == null)
				throw new ArgumentException("this.Path cannot be null");
			if(vosLogicalHandle == null)
				throw new ArgumentNullException("vosLogicalHandle");
			if(T == null)
				throw new ArgumentNullException("T");

			object obj = null;
#else
        public void Save<T>(VobHandle<T> vosLogicalHandle, bool allowDelete = false, bool preview = false)
            where T : class
        {
            T obj = null;

#endif
#if AOT && true // AOTTEMP
#else
            //l.Info("Vob.Save " + vosLogicalHandle.Path + "<"+T.Name+">  " + Environment.StackTrace);
            //this.Type = typeof(T);
            if (vosLogicalHandle.HasObject)
            {
                try
                {
                    obj = vosLogicalHandle.Object;
                }
                catch (Exception ex)
                {
                    l.Error("Failed to get Object from VobHandle: " + ex);
                    throw;
                }
            }
            //vosLogicalHandle.Object = obj;
            //vosLogicalHandle.Save();

            if (obj == null) // FUTURE: test IsDeleted flag instead or in addition?
            {
                if (!preview) { l.Trace("REVIEW - Save called when !HasObject.  Attempting Delete. - " + allowDelete + " " + ToString()); }
                if (allowDelete)
                {
                    TryDelete(preview: preview);
                }
                else
                {
                    l.Trace(() => "Attempt to Vob.Save((VobHandle<>) vh, allowDelete: false) when vh.Object is null.  Doing nothing. " + this);
                }
                return;
            }

            if (!preview)
            {
                INotifyOnSavingTo nos = obj as INotifyOnSavingTo;
                if (nos != null)
                {
                    nos.OnSavingTo(this);
                }
            }

            //    Save(_object, package, location);
            //}
            //public void Save(object obj, string package = null, string location = null)
            //{
            var context = VosContext.Current;
            string package = vosLogicalHandle.EffectivePackage;
            string location = vosLogicalHandle.EffectiveStore;

            if (package == null)
            {
                if (context != null)
                {
                    package = context.Package;
                }
            }
            if (location == null)
            {
                if (context != null)
                {
                    location = context.Store;
                }
            }

            IHandle<T> saveHandle = null;

            //this.StackCheck(110);
            if (vosLogicalHandle.Mount != null
                //|| vosLogicalHandle.Location != null || vosLogicalHandle.Package != null
                )
            {
                // TOVALIDATE?
                saveHandle = GetMountHandle<T>(vosLogicalHandle.Mount);
                //l.Trace("SAVE: specifying Mount.  Got handle: " + saveHandle + ". TODO: Does this mean this object was saved more than once, unintentionally?");
            }

            //if (
            //     location != null 
            //     || package != null
            //    )
            //{
            //    l.Warn("SAVE: Not implemented: specifying Location/Package ");
            //}

            ////AssetContext.Current.DefaultSavePackage
            //string packageSubpath = null;
            //var packageMount = effectiveMountsByName.TryGetValue(package);
            //if (packageMount != null)
            //{
            //}

            if (saveHandle == null)
            {
                // TODO: 
                // - Test this.IsPackage(packageName), same for location.
                // - lock down Vob branches as being only for a location/package. 

#if AOT
                var tempSaveHandle = "".PathToVobHandle( package, location,T);
#else
                var tempSaveHandle = "".PathToVobHandle<T>(package, location);
#endif
                //#error SAving Timeline--no save location??
                if (!StringX.IsNullOrWhiteSpace(tempSaveHandle.Path.TrimEnd(LionPath.PathDelimiter)) && Path.StartsWith(tempSaveHandle.Path.TrimEnd(LionPath.PathDelimiter))) // TEMP approach TODO
                {
                    saveHandle = GetFirstWriteHandle<T>();

                }
                else
                {
                    // First time: don't save to generic mounts (null package/store)
                    foreach (var kvp in WriteHandleMounts)
                    {
                        var mount = kvp;

                        if (package != null && package != mount.Package)
                        {
                            continue;
                        }

                        if (location != null && location != mount.Store)
                        {
                            continue;
                        }

                        //saveHandle = kvp.Key;
                        //saveHandle = mount.Vob.GetHandle<T>();
                        //saveHandle = mount.Root.GetHandle<T>(); - wrong
                        saveHandle = GetMountHandle<T>(mount);

                        if (vosLogicalHandle.Mount == null)
                        {
                            vosLogicalHandle.Mount = mount;
                        }
                        break;
                    }

                    // First time: save to any generic mounts that are available (null package/store)
                    if (saveHandle == null)
                    {
                        //foreach (var kvp in WriteHandlesWithMounts)
                        foreach (var kvp in WriteHandleMounts)
                        {
                            var mount = kvp;

                            if (package != null && mount.Package != null && package != mount.Package)
                            {
                                continue;
                            }

                            if (location != null && mount.Store != null && location != mount.Store)
                            {
                                continue;
                            }

                            //saveHandle = kvp.Key;
                            //saveHandle = mount.Vob.GetHandle<T>();
                            //saveHandle = mount.Root.GetHandle<T>(); - wrong
                            saveHandle = GetMountHandle<T>(mount);

                            if (vosLogicalHandle.Mount == null)
                            {
                                vosLogicalHandle.Mount = mount;
                            }
                            break;
                        }
                    }
                }

                //saveHandle = this.Path.PathToVobHandleRENAME<T>(package, location);
                //if (saveHandle == this)
                //{
                //    saveHandle = GetFirstWriteHandle();
                //}
            }

            //if (saveHandle == null)
            //{
            //    saveHandle = objectHandle;
            //}
            //if (saveHandle == null)
            //{
            //    saveHandle = GetFirstWriteHandle();
            //    //objectHandle = saveHandle;
            //}

            //IHandle mountHandle = FirstWriteHandle;

            if (saveHandle == null)
            {
                Vos.OnNoSaveLocation(this);
                return;
            }

            if (!preview)
            {
                if (saveHandle.HasObject)
                {
                    if (!System.Object.ReferenceEquals(saveHandle.Object, obj))
                    {
#if WARN_VOB
                        if (saveHandle.Object.GetType() != obj.GetType())
                        {
                            lSave.Warn(ToString() + ": Vob.Save Replacing object '" + saveHandle.Object.GetType().Name + "' in concrete OBase with object of different type: " + obj.GetType().Name);
                        }
                        else
                        {
                            lSave.Info(ToString() + ": Vob.Save Replacing object in concrete OBase with object of same type. "
                                           //							           + Environment.NewLine + Environment.StackTrace
                                           );
                        }
#endif
                        saveHandle.Object = obj;
                    }
                }
                else
                {
                    saveHandle.Object = obj;
                }

                MBus.Current.Publish(new VobSaveEvent(VosReference, saveHandle.Reference));


#if INFO_VOB
            if (saveHandle.ToString().Contains("file:"))
            {
                lSave.Info("[SAVE] Vob " + vosLogicalHandle.ToString() + " => " + saveHandle.ToString());
            }
            else
            {
                lSave.Debug("[save] Vob " + vosLogicalHandle.ToString() + " => " + saveHandle.ToString());
            }
#endif

                saveHandle.Save();

#if VosUnitype
                if (this.UnitypeHandle == null)
                {
                    this.UnitypeHandle = vosLogicalHandle;
                }
#endif
            }
#endif
        }




        // OLD, but rework?  - no more saving without specifying object.
        //        /// <summary>
        //        /// Save to a Vob location, which may be a virtual and layered location.  (If you wanted to save to a particular location or package, you should save to
        //        /// another Vob representing that desire.)
        //        /// </summary>
        //        public void Save()
        //        {
        //            //    this.Save(null, null);
        //            //}
        //            //public void Save(string package = null, string location = null)
        //            //{
        //            object obj = _object;

        //            if (obj == null) // FUTURE: test IsDeleted flag instead or in addition?
        //            {
        //                l.Warn("REVIEW - Save called when !HasObject.  Attempting Delete.");
        //                TryDelete();
        //                return;
        //            }

        //            //    Save(_object, package, location);
        //            //}
        //            //public void Save(object obj, string package = null, string location = null)
        //            //{
        //            var context = VosContext.Current;

        //            IHandle saveHandle = null;

        //            //if (package == null)
        //            //{
        //            //    if (context != null) package = context.Package;
        //            //}
        //            //if (location == null)
        //            //{
        //            //    if (context != null) location = context.Location;
        //            //}

        //            ////AssetContext.Current.DefaultSavePackage
        //            //string packageSubpath = null;
        //            //var packageMount = effectiveMountsByName.TryGetValue(package);
        //            //if (packageMount != null)
        //            //{
        //            //}

        //            //if (location != null)
        //            //{

        //            //    saveHandle = this.Path.PathToVobHandle<object>(package, location);
        //            //}

        //            //if (saveHandle == null)
        //            //{
        //            //    saveHandle = objectHandle;
        //            //}
        //            if (saveHandle == null)
        //            {
        //                saveHandle = GetFirstWriteHandle();
        //                //objectHandle = saveHandle;
        //            }

        //            //IHandle mountHandle = FirstWriteHandle;

        //            if (saveHandle == null)
        //            {
        //                this.Vos.OnNoSaveLocation(this);
        //                return;
        //            }

        //            if (saveHandle.HasObject)
        //            {
        //                if (!System.Object.ReferenceEquals(saveHandle.Object, obj))
        //                {
        //#if WARN_VOB
        //                    if (saveHandle.Object.GetType() != obj.GetType())
        //                    {
        //                        l.Warn(this.ToString() + ": Replacing object in concrete OBase with object of different type.");
        //                    }
        //                    else
        //                    {
        //                        l.Info(this.ToString() + ": Replacing object in concrete OBase with object of same type.");
        //                    }
        //#endif
        //                    saveHandle.Object = obj;
        //                }
        //            }
        //            else
        //            {
        //                saveHandle.Object = obj;
        //            }

        //#if INFO_VOB
        //            if (saveHandle.ToString().Contains("file:"))
        //            {
        //                lSave.Info("[SAVE] Vob " + this.ToString() + " => " + saveHandle.ToString());
        //            }
        //            else
        //            {
        //                lSave.Debug("[save] Vob " + this.ToString() + " => " + saveHandle.ToString());
        //            }
        //#endif
        //            saveHandle.Save();
        //        }

        #endregion

        #region Retrieve

        //public Mount GetMountForHandle(IHandle handleParam) OLD Unneeded
        //{
        //    if (handleParam == null) return null;

        //    if(handleParam.Mount != null) return handleParam.Mount;



        //    foreach (var kvp in ReadHandlesWithMounts)
        //    {
        //        var handle = kvp.Key;
        //        if (handleParam.Equals(handle))
        //        {
        //            //l.LogCritical("== - " + handle + " " + handleParam);
        //            return kvp.Value;
        //        }
        //        else
        //        {
        //            //l.Warn("!= - " + handle + " " + handleParam);
        //        }
        //    }
        //    return null;
        //}

        //public bool TryRetrieve(bool setToNullOnFail = true)
        //{
        //    object result = null;

        //    foreach (var kvp in ReadHandlesWithMounts)
        //    {
        //        var handle = kvp.Key;
        //        if (handle.TryRetrieve())
        //        {
        //            result = handle.Object;

        //            this.UnitypeHandle = handle;
        //            this.ObjectHandleMount = kvp.Value;

        //            this.OnRetrieved(result);

        //            return true;
        //        }
        //    }

        //    if (setToNullOnFail) SetObjectToNull();
        //    //OnRetrievedNothing();
        //    return false;
        //}

        //public object TryRetrieve(VosReference reference)
        //{
        //    if (reference.Path != this.Path)
        //    {
        //        throw new ArgumentException("Path invalid for this vob");
        //    }
        //    if (reference.Type != null)
        //    {
        //        return TryRetrieve(type: reference.Type, reference.Package );
        //    }
        //    else
        //    {
        //        return TryRetrieveByTypeName(typeName: reference.TypeName, reference.Package);
        //    }
        //}

        //public object TryRetrieveByTypeName(string typeName = null, string location = null, )
        //{
        //    Type type = Type.GetType(typeName, true);
        //    return TryRetrieve(type: type, location: location, );
        //}

        protected void OnRetrieved(object retrievedObject)
        {
            //base.OnRetrieved(retrievedObject);

            if (retrievedObject != null)
            {
                //this.Type = retrievedObject.GetType();
            }
        }

        //protected void OnRetrievedNothing()
        //{
        //}

        //private void SetObjectToNull()
        //{
        //    this._object = null;
        //    this.UnitypeHandle = null;
        //    this.ObjectHandleMount = null;
        //    //objectHandle = null;

        //    //objectHandles = null;
        //}

        public IEnumerable<object> TryEnsureRetrievedAllLayers()
        {
            foreach (var handle in ReadHandles)
            {
                if (!handle.TryEnsureRetrieved())
                {
                    continue;
                }

                yield return handle.Object;
            }
        }
        public IEnumerable<IHandle> AllRetrievableReadHandles
        {
            get
            {
                foreach (var handle in ReadHandles)
                {
                    if (!handle.TryEnsureRetrieved())
                    {
                        continue;
                    }

                    yield return handle;
                }
            }
        }



#if !NoGenericMethods
        public T AsTypeOrCreate<T>()
            where T : class, new()
        {
            var first = AsType<T>();
            if (first == null)
            {
                first = new T();
            }
            return first;
        }

        public T AsType<T>()
            where T : class
        {
            var first = AllLayersOfType<T>().FirstOrDefault();
            if (first != null && AllLayersOfType<T>().ElementAtOrDefault(1) != null)
            {
                l.Warn("AsType has more than one match: " + this);
            }
            return first;
        }

        public IEnumerable<T> AllLayersOfType<T>()
            where T : class
        {
            foreach (var handle in ReadHandles)
            {
                if (!handle.TryEnsureRetrieved())
                {
                    continue;
                }

                T obj = handle.Object as T;
                if (obj != null)
                {
                    yield return obj;
                    continue;
                }

                IMultiTyped mt = obj as IMultiTyped;
                if (mt != null)
                {
                    obj = mt.AsType<T>();
                    if (obj != null)
                    {
                        yield return obj;
                        continue;
                    }
                }

            }
        }
#endif

        [AotReplacement]
        public object AsTypeOrCreate(Type type) => throw new NotImplementedException("Vob.AsTypeOrCreate");

        [AotReplacement]
        public object AsType(Type T) => AllLayersOfType(T).FirstOrDefault();

        [AotReplacement] // TODO - support this in Rewriter
        public IEnumerable<object> AllLayersOfType(Type T)
        {
            foreach (var handle in ReadHandles)
            {
                if (!handle.TryEnsureRetrieved())
                {
                    continue;
                }

                object obj = handle.Object;
                if (T.IsAssignableFrom(obj.GetType()))
                {
                    yield return obj;
                    continue;
                }

                IMultiTyped mt = obj as IMultiTyped;
                if (mt != null)
                {
                    obj = mt.AsType(T);
                    if (obj != null)
                    {
                        yield return obj;
                        continue;
                    }
                }

            }
        }

        public IEnumerable<object> AllLayers()
        {
            foreach (var handle in ReadHandles)
            {
                if (!handle.TryEnsureRetrieved())
                {
                    continue;
                }

                object obj = handle.Object;

                IMultiTyped mt = obj as IMultiTyped;
                if (mt != null)
                {
                    foreach (var o in mt.SubTypes)
                    {
                        yield return o;
                    }
                    continue;
                }

                if (obj != null) { yield return obj; }
            }
        }
        public bool Exists => AllLayers().Any();

#if !AOT
        // FUTURE ENH: Return IHandle<T>'s? or VobHandle<T>'s, with T based on detected type of the object?
        public IEnumerable<IHandle> AllHandlesOfType<T>()
            where T : class
        {
            foreach (var handle in ReadHandles)
            {
                if (!handle.TryEnsureRetrieved())
                {
                    continue;
                }

                T obj = handle.Object as T;
                if (obj != null)
                {
                    yield return handle;
                }

                IMultiTyped mt = obj as IMultiTyped;
                obj = mt.AsType<T>();
                if (obj != null)
                {
                    yield return handle;
                }
            }
        }

        internal RetrieveType TryRetrieve<RetrieveType>(VobHandle<RetrieveType> vosLogicalHandle = null)
            where RetrieveType : class => TryEnsureRetrieved_(true, vosLogicalHandle);

        internal RetrieveType TryEnsureRetrieved<RetrieveType>(VobHandle<RetrieveType> vosLogicalHandle = null)
            where RetrieveType : class => TryEnsureRetrieved_(false, vosLogicalHandle);

#if FUTURE // Get read handle
        private IVobHandle TryGetReadHandle<RetrieveType>(bool reload, VobHandle<RetrieveType> vosLogicalHandle = null)
            where RetrieveType : class
        {
            IVobHandle resultHandle;
            object resultObj;
            RetrieveType result = null;

        #region UnitypeHandle

            if (UnitypeHandle != null)
            {
                if (!reload)
                {
                    //UnitypeHandle.Retrieve();

                    object uniTypeObj = UnitypeHandle.HasObject ? UnitypeHandle.Object : null;

                    result = uniTypeObj as RetrieveType;
                    if (result != null) return UnitypeHandle;
                    else
                    {
                        if (uniTypeObj != null)
                        {
                            l.Trace("UNTESTED - UnitypeHandle has object of different type (" + uniTypeObj.GetType().Name + "), but type " + typeof(RetrieveType).Name + " was requested.");
                        }
                    }
                }
            }

        #endregion


            //    //return (RetrieveType)TryRetrieve(type: typeof(RetrieveType), location: vh.Location, mount: vh.Mount);
            //    return (RetrieveType)TryRetrieve(typeof(RetrieveType), vh);
            //}

            //public object TryRetrieve<RetrieveType>(Type type = null,
            //    //string location = null, Mount mount = null, 
            //    VobHandle<RetrieveType> vosLogicalHandle = null)
            //    where RetrieveType : class
            //{
            string location = vosLogicalHandle?.Store;
            Mount mount = vosLogicalHandle?.Mount;

#if SanityChecks
            if (vosLogicalHandle?.Path != this.Path)
            {
                throw new UnreachableCodeException("vh.Path != this.Path: " + vosLogicalHandle.Path + " != " + this.Path);
            }
#endif


            //if (layer == null && type == null) -- what was this for?  infiniteloop
            //{
            //    TryRetrieve();
            //    return _object;
            //}

            // REVIEW: Yikes lots of branches

            try
            {
                if (location != null)
                {
                    if (mount == null)
                    {
                        mount = effectiveMountsByName.TryGetValue(location);

                    }
                    else
                    {
                        // TOVALIDATE - verify mount matches location?
                        if (mount.Store != location)
                        {
                            l.Warn("VALIDATION fail: mount.LayerName != location --> " + mount.Store + " != " + location);
                        }
                    }
                    if (mount == null)
                    {
                        result = null;
                        return null;
                    }

                    IHandle<RetrieveType> handle = GetMountHandle<RetrieveType>(mount);

                    //if (type == null) Identical to block below. OLD DELETE
                    //{
                    //    if (handle.TryEnsureRetrieved())
                    //    {
                    //        System.Diagnostics.Debug.Assert(handle.Object != null, "handle.Object == null after TryEnsureRetrieved == true");
                    //        resultObj = handle.Object;
                    //        result = resultObj as RetrieveType;
                    //        if (resultObj != null && result == null)
                    //        {
                    //            lLoad.Warn("Specified location found object but doesn't match requested RetrieveType of " + typeof(RetrieveType).Name);
                    //        }
                    //        return result;
                    //    }
                    //    else
                    //    {
                    //        result = null;
                    //        return result;
                    //    }
                    //}
                    //else
                    {
                        // FUTURE: RetrieveAsType? multitype stuff?
                        if (reload ? handle.TryRetrieve() : handle.TryEnsureRetrieved())
                        {
                            System.Diagnostics.Debug.Assert(handle.Object != null, "handle.Object == null after TryEnsureRetrieved == true");
                            resultObj = handle.Object;
                            result = resultObj as RetrieveType;
                            if (resultObj != null && result == null)
                            {
                                lLoad.Warn("Specified location found object but doesn't match requested RetrieveType of " + typeof(RetrieveType).Name);
                            }
                            return result;
                            //if (resultObj==null || typeof(RetrieveType).IsAssignableFrom(resultObj.GetType()))
                            //{
                            //    return resultObj;
                            //}
                            //else
                            //{
                            //    result = null;
                            //    return resultObj;
                            //}
                        }
                        else
                        {
                            result = null;
                            return result;
                        }
                    }
                }
                else // location == null
                {
                    //if (this.IsMultiTypeObjectMerged) // Never set!
                    //{
                    //    l.Debug("UNTESTED REVIEW IsMultiTypeObjectMerged - may not make sense");
                    //    var results = TryEnsureRetrievedAllLayers().OfType<RetrieveType>();
                    //    if (results == null || !results.Any())
                    //    {
                    //        result = null;
                    //        return result;
                    //    }

                    //    if (results.ElementAtOrDefault(2) != null) // if more than 1, create a multitype
                    //    {
                    //        #region Not Implemented: Same types:

                    //        List<Type> types = new List<Type>();
                    //        foreach (var result in results)
                    //        {
                    //            if (types.Contains(result.GetType())) throw new NotImplementedException("Vobs with multiple results of same type");
                    //            types.Add(result.GetType());
                    //        }

                    //        #endregion

                    //        MultiType mt = new MultiType(results);
                    //        // TODO: Set objectHandles; (doh - need to get from TryRetrieveAllLayers?)
                    //        resultObj = mt;
                    //        result = mt.AsType<RetrieveType>();
                    //        return result;
                    //    }
                    //    else
                    //    {
                    //        resultObj = results.ElementAt(0);
                    //        return resultObj;
                    //    }
                    //}
                    //else // Return the first object in the Vos Read Stack
                    {
                        IHandle<RetrieveType> handle = null; // FUTURE: get hint from VobHandle.TargetHandle?

                        if (mount != null) handle = GetMountHandle<RetrieveType>(mount);

        #region If mount is already known, try it first

                        if (handle != null && mount != null)
                        {
                            result = _TryRetrieve(handle, mount, vosLogicalHandle, reload: reload);
                            if (result != null)
                            {
                                // REVIEW - allow mount hints? make this an option?  Downside is that desired mount may change, and hint would not be updated

                                //l.Trace("Retrieved obj from mount hint: " + mount);
                                return result;
                            }
                        }
        #endregion

                        //IMergeable mergeable = null;

        #region Get the first hit on the read stack

                        foreach (var readMount in ReadHandleMounts)
                        {
                            handle = GetMountHandle<RetrieveType>(readMount);
                            result = _TryRetrieve(handle, mount, vosLogicalHandle, reload: reload);
                            if (result != null)
                            {
                                return result;
                            }
                        }

        #endregion

                        return null;
                    }
                }
            }
            finally
            {
                if (result != null && UnitypeHandle == null)
                {
                    UnitypeHandle = vosLogicalHandle;
                }
            }
        }
#endif

        // REVIEW - this is too complicated?
        private RetrieveType TryEnsureRetrieved_<RetrieveType>(bool reload, VobHandle<RetrieveType> vosLogicalHandle = null)
            where RetrieveType : class
        {
            object resultObj;
            RetrieveType result = null;

#if VosUnitype
            if (UnitypeHandle != null)
            {
                if (!reload)
                {
                    //UnitypeHandle.Retrieve();

                    object uniTypeObj = UnitypeHandle.HasObject ? UnitypeHandle.Object : null;

                    result = uniTypeObj as RetrieveType;
                    if (result != null) return result;
                    else
                    {
                        if (uniTypeObj != null)
                        {
                            l.Trace("UNTESTED - UnitypeHandle has object of different type (" + uniTypeObj.GetType().Name + "), but type " + typeof(RetrieveType).Name + " was requested.");
                        }
                    }
                }
            }
#endif

            //    //return (RetrieveType)TryRetrieve(type: typeof(RetrieveType), location: vh.Location, mount: vh.Mount);
            //    return (RetrieveType)TryRetrieve(typeof(RetrieveType), vh);
            //}

            //public object TryRetrieve<RetrieveType>(Type type = null,
            //    //string location = null, Mount mount = null, 
            //    VobHandle<RetrieveType> vosLogicalHandle = null)
            //    where RetrieveType : class
            //{
            string location = vosLogicalHandle.Store;
            Mount mount = vosLogicalHandle.Mount;

#if SanityChecks
            if (vosLogicalHandle.Path != this.Path)
            {
                throw new UnreachableCodeException("vh.Path != this.Path: " + vosLogicalHandle.Path + " != " + this.Path);
            }
#endif

            //if (layer == null && type == null) -- what was this for?  infiniteloop
            //{
            //    TryRetrieve();
            //    return _object;
            //}

            // REVIEW: Yikes lots of branches
            IHandle<RetrieveType> handle = null;
            Mount readFromMount = null;
            try
            {
                if (location != null)
                {
                    if (mount == null)
                    {
                        mount = effectiveMountsByName.TryGetValue(location);
                    }
                    else
                    {
                        // TOVALIDATE - verify mount matches location?
                        if (mount.Store != location)
                        {
                            l.Warn("VALIDATION fail: mount.LayerName != location --> " + mount.Store + " != " + location);
                        }
                    }
                    if (mount == null) // No mount for specified location
                    {
                        l.Trace($"Retrieve specified location '{location}' but no mount was found at {Path}");
                        result = null;
                        goto end;
                    }

                    handle = GetMountHandle<RetrieveType>(mount);

                    //if (type == null) Identical to block below. OLD DELETE
                    //{
                    //    if (handle.TryEnsureRetrieved())
                    //    {
                    //        System.Diagnostics.Debug.Assert(handle.Object != null, "handle.Object == null after TryEnsureRetrieved == true");
                    //        resultObj = handle.Object;
                    //        result = resultObj as RetrieveType;
                    //        if (resultObj != null && result == null)
                    //        {
                    //            lLoad.Warn("Specified location found object but doesn't match requested RetrieveType of " + typeof(RetrieveType).Name);
                    //        }
                    //        return result;
                    //    }
                    //    else
                    //    {
                    //        result = null;
                    //        return result;
                    //    }
                    //}
                    //else
                    {
                        // FUTURE: RetrieveAsType? multitype stuff?
                        if (reload ? handle.TryRetrieve() : handle.TryEnsureRetrieved())
                        {

                            System.Diagnostics.Debug.Assert(handle.Object != null, "handle.Object == null after TryEnsureRetrieved == true");
                            resultObj = handle.Object;
                            result = resultObj as RetrieveType;
                            if (resultObj != null && result == null)
                            {
                                lLoad.Warn($"Specified location found object of type {resultObj.GetType().Name} but doesn't match requested RetrieveType of " + typeof(RetrieveType).Name);
                            }
                            readFromMount = mount;
                            goto end;
                            //if (resultObj==null || typeof(RetrieveType).IsAssignableFrom(resultObj.GetType()))
                            //{
                            //    return resultObj;
                            //}
                            //else
                            //{
                            //    result = null;
                            //    return resultObj;
                            //}
                        }
                        else
                        {
                            result = null;
                            goto end;
                        }
                    }
                }
                else // location == null
                {
                    //if (this.IsMultiTypeObjectMerged) // Never set!
                    //{
                    //    l.Debug("UNTESTED REVIEW IsMultiTypeObjectMerged - may not make sense");
                    //    var results = TryEnsureRetrievedAllLayers().OfType<RetrieveType>();
                    //    if (results == null || !results.Any())
                    //    {
                    //        result = null;
                    //        return result;
                    //    }

                    //    if (results.ElementAtOrDefault(2) != null) // if more than 1, create a multitype
                    //    {
                    //        #region Not Implemented: Same types:

                    //        List<Type> types = new List<Type>();
                    //        foreach (var result in results)
                    //        {
                    //            if (types.Contains(result.GetType())) throw new NotImplementedException("Vobs with multiple results of same type");
                    //            types.Add(result.GetType());
                    //        }

                    //        #endregion

                    //        MultiType mt = new MultiType(results);
                    //        // TODO: Set objectHandles; (doh - need to get from TryRetrieveAllLayers?)
                    //        resultObj = mt;
                    //        result = mt.AsType<RetrieveType>();
                    //        return result;
                    //    }
                    //    else
                    //    {
                    //        resultObj = results.ElementAt(0);
                    //        return resultObj;
                    //    }
                    //}
                    //else // Return the first object in the Vos Read Stack
                    {
                        handle = null; // FUTURE: get hint from VobHandle.TargetHandle?

                        if (mount != null) { handle = GetMountHandle<RetrieveType>(mount); }

                        #region If mount is already known, try it first

                        if (handle != null && mount != null)
                        {
                            result = _TryRetrieve(handle, mount, vosLogicalHandle, reload: reload);
                            if (result != null)
                            {
                                readFromMount = mount;
                                // REVIEW - allow mount hints? make this an option?  Downside is that desired mount may change, and hint would not be updated

                                l.Trace($"Retrieved obj of type '{typeof(RetrieveType).Name}' from mount hint: '{mount}' at {Path}");
                                goto end;
                            }
                        }
                        #endregion

                        //IMergeable mergeable = null;

                        #region Get the first hit on the read stack

                        foreach (var readMount in ReadHandleMounts)
                        {
                            handle = GetMountHandle<RetrieveType>(readMount);
                            result = _TryRetrieve(handle, mount, vosLogicalHandle, reload: reload); // REVIEW - FIXME should this mount be readMount?
                            if (result != null)
                            {
                                readFromMount = readMount;
                                goto end;
                            }
                        }

                        #endregion

                        result = null;
                        goto end;
                    }
                }
            }
            finally
            {
#if VosUnitype
                if (result != null && UnitypeHandle == null)
                {
                    UnitypeHandle = vosLogicalHandle;
                }
#endif
            }
            end:
            if (result != null && vosLogicalHandle.VosRetrieveInfo != null)
            {
                vosLogicalHandle.VosRetrieveInfo.Mount = readFromMount;
                vosLogicalHandle.VosRetrieveInfo.ReadHandle = handle;
            }
            return result;
        }
#else
        internal object TryRetrieve(IVobHandle vosLogicalHandle)
		{
			Type retrieveType = vosLogicalHandle.Type;
			object resultObj;
			object result = null;
			
			if (UnitypeHandle != null)
			{
				object uniTypeObj = UnitypeHandle.HasObject ? UnitypeHandle.Object : null;

				if(uniTypeObj != null && uniTypeObj.GetType() == retrieveType)
				{
					return uniTypeObj;
				}
//				result = uniTypeObj as RetrieveType;
//				if (result != null) return result;
				else
				{
					if (uniTypeObj != null)
					{
						if(retrieveType != typeof(object) && !retrieveType.IsInterface ) // TEMP - avoid this?  - TODO REVIEW
						{
							l.Trace("UNTESTED - UnitypeHandle has object of different type (" + uniTypeObj.GetType().Name + "), but type " + retrieveType.Name + " was requested.");
						}
					}
				}
			}
			
			//    //return (RetrieveType)TryRetrieve(type: typeof(RetrieveType), location: vh.Location, mount: vh.Mount);
			//    return (RetrieveType)TryRetrieve(typeof(RetrieveType), vh);
			//}
			
			//public object TryRetrieve<RetrieveType>(Type type = null,
			//    //string location = null, Mount mount = null, 
			//    VobHandle<RetrieveType> vosLogicalHandle = null)
			//    where RetrieveType : class
			//{
			string location = vosLogicalHandle.Store;
			Mount mount = vosLogicalHandle.Mount;
			
#if SanityChecks
			if (vosLogicalHandle.Path != this.Path)
			{
				throw new UnreachableCodeException("vh.Path != this.Path: " + vosLogicalHandle.Path + " != " + this.Path);
			}
#endif
			
			
			//if (layer == null && type == null) -- what was this for?  infiniteloop
			//{
			//    TryRetrieve();
			//    return _object;
			//}
			
			// REVIEW: Yikes lots of branches
			
			try
			{
				if (location != null)
				{
					if (mount == null)
					{
						mount = 
#if AOT
								(Mount)
#endif
								effectiveMountsByName.TryGetValue(location);
						
					}
					else
					{
						// TOVALIDATE - verify mount matches location?
						if (mount.Store != location)
						{
							l.Warn("VALIDATION fail: mount.LayerName != location --> " + mount.Store + " != " + location);
						}
					}
					if (mount == null)
					{
						result = null;
						return result;
					}
					
					IHandle handle = GetMountHandle(mount);
					
					//if (type == null) Identical to block below. OLD DELETE
					//{
					//    if (handle.TryEnsureRetrieved())
					//    {
					//        System.Diagnostics.Debug.Assert(handle.Object != null, "handle.Object == null after TryEnsureRetrieved == true");
					//        resultObj = handle.Object;
					//        result = resultObj as RetrieveType;
					//        if (resultObj != null && result == null)
					//        {
					//            lLoad.Warn("Specified location found object but doesn't match requested RetrieveType of " + typeof(RetrieveType).Name);
					//        }
					//        return result;
					//    }
					//    else
					//    {
					//        result = null;
					//        return result;
					//    }
					//}
					//else
					{
						// FUTURE: RetrieveAsType? multitype stuff?
						if (handle.TryEnsureRetrieved())
						{
							System.Diagnostics.Debug.Assert(handle.Object != null, "handle.Object == null after TryEnsureRetrieved == true");
							resultObj = handle.Object;
							if (resultObj != null && resultObj.GetType() != retrieveType)
							{
								lLoad.Warn("Specified location found object but doesn't match requested RetrieveType of " + retrieveType.Name);
							}
							return result;
							//if (resultObj==null || typeof(RetrieveType).IsAssignableFrom(resultObj.GetType()))
							//{
							//    return resultObj;
							//}
							//else
							//{
							//    result = null;
							//    return resultObj;
							//}
						}
						else
						{
							result = null;
							return result;
						}
					}
				}
				else // location == null
				{
					//if (this.IsMultiTypeObjectMerged) // Never set!
					//{
					//    l.Debug("UNTESTED REVIEW IsMultiTypeObjectMerged - may not make sense");
					//    var results = TryEnsureRetrievedAllLayers().OfType<RetrieveType>();
					//    if (results == null || !results.Any())
					//    {
					//        result = null;
					//        return result;
					//    }
					
					//    if (results.ElementAtOrDefault(2) != null) // if more than 1, create a multitype
					//    {
					//        #region Not Implemented: Same types:
					
					//        List<Type> types = new List<Type>();
					//        foreach (var result in results)
					//        {
					//            if (types.Contains(result.GetType())) throw new NotImplementedException("Vobs with multiple results of same type");
					//            types.Add(result.GetType());
					//        }
					
					//        #endregion
					
					//        MultiType mt = new MultiType(results);
					//        // TODO: Set objectHandles; (doh - need to get from TryRetrieveAllLayers?)
					//        resultObj = mt;
					//        result = mt.AsType<RetrieveType>();
					//        return result;
					//    }
					//    else
					//    {
					//        resultObj = results.ElementAt(0);
					//        return resultObj;
					//    }
					//}
					//else // Return the first object in the Vos Read Stack
					{
						IHandle handle = null; // FUTURE: get hint from VobHandle.TargetHandle?
						
						if (mount != null) handle = GetMountHandle(mount);
						
        #region If mount is already known, try it first
						
						if (handle != null && mount != null)
						{
							result = _TryRetrieve(handle, mount, vosLogicalHandle, retrieveType);
							if (result != null)
							{
								//l.Trace("Retrieved obj from mount hint: " + mount);
								return result;
							}
						}
        #endregion
						
						//IMergeable mergeable = null;
						
        #region Get the first hit on the read stack
						
						foreach (KeyValuePair<IHandle, Mount> kvp in
#if AOT
 (IEnumerable)
#endif
                            ReadHandlesWithMounts)
						{
							result = _TryRetrieve(kvp.Key, kvp.Value, vosLogicalHandle, retrieveType);
							if (result != null)
							{
								return result;
							}
						}
						
        #endregion
						
						return null;
					}
				}
			}
			finally
			{
				if (result != null && UnitypeHandle == null)
				{
					UnitypeHandle = vosLogicalHandle;
				}
			}
		}
#endif

#if !AOT
        private ObjType _TryRetrieve<ObjType>(IHandle<ObjType> handle, Mount mount, VobHandle<ObjType> vosLogicalHandle, bool reload)
            where ObjType : class
        {
            if (reload)
            {
                if (!handle.TryRetrieve(setToNullOnFail: true))
                {
                    return null;
                }
                //handle.ForgetObject(); // FUTURE?
                //if (!handle.TryEnsureRetrieved()) return null;
            }
            else
            {
                if (!handle.TryEnsureRetrieved())
                {
                    return null;
                }
            }

            //ObjType result = handle.Object as ObjType;
            ObjType result = handle.Object;

            if (result == null) // OLD UNREACHABLE 
            {
                l.Warn("UNREACHABLE Retrieved object of unexpected type for handle.  Expected: " + typeof(ObjType).Name + ".  Got: " + handle.Object.GetType().Name);
                return result;
            }

            if (vosLogicalHandle != null)
            {
                //l.LogCritical("TEMP Retrieve succeeded.  Setting VobHandle<>.Mount: " + vosLogicalHandle + " => " + mount);
                vosLogicalHandle.Mount = mount;
                // REVIEW - also store handle as a hint to go with mount, or instead of it?
            }

            //IVobHandle vh = handle as IVobHandle;
            //if (vh != null)
            //{
            //    l.Debug("vh != null in _TryRetrieve. setting mount.");
            //    vh.Mount = mount;
            //}
            //else
            //{
            //    l.Trace("vh == null in _TryRetrieve");
            //}

            //this.Object = handle.Object;
#if VosUnitype
            if (_unitypeHandle == null)
            {
                //UnitypeHandle = handle;
                UnitypeHandle = vosLogicalHandle;
            }
#endif
            //ObjectHandleMount = mount;

            OnRetrieved(result);
            return result;
        }
#else
        private object _TryRetrieve(IHandle handle, Mount mount, IVobHandle vosLogicalHandle, Type objType)
//            where ObjType : class
        {
            if (!handle.TryEnsureRetrieved()) return null;

			object result = handle.Object ;
//			as ObjType;

            if (result != null && result.GetType() != objType)
            {
				if(objType != typeof(object))
				{
            	    l.Warn("Retrieved object of unexpected type for handle.  Expected: " + objType. Name + ".  Got: " + handle.Object.GetType().Name);
				}
//                return result;
            }

            if (vosLogicalHandle != null)
            {
                //l.LogCritical("TEMP Retrieve succeeded.  Setting VobHandle<>.Mount: " + vosLogicalHandle + " => " + mount);
                vosLogicalHandle.Mount = mount;
                // REVIEW - also store handle as a hint to go with mount, or instead of it?
            }

            //IVobHandle vh = handle as IVobHandle;
            //if (vh != null)
            //{
            //    l.Debug("vh != null in _TryRetrieve. setting mount.");
            //    vh.Mount = mount;
            //}
            //else
            //{
            //    l.Trace("vh == null in _TryRetrieve");
            //}

            //this.Object = handle.Object;
            if (_unitypeHandle == null)
            {
                //UnitypeHandle = handle;
                UnitypeHandle = vosLogicalHandle;
            }
            //ObjectHandleMount = mount;

            OnRetrieved(result);
            return result;
        }
#endif

        //public object TryRetrieve(VosReference reference, string[] pathChunks, int pathIndex)
        //{
        //    List<Mount> mounts = new List<Mount>();

        //    object obj=null;

        //    if(reference.Layer != null)
        //    {
        //        Mount mount = effectiveMountsByName.TryGetValue(reference.Layer);

        //        if(mount != null)
        //        {
        //        }
        //        else
        //        {
        //            throw new ArgumentException("Layer not mounted: " + (reference.Layer ?? "null"));
        //        }
        //    }
        //    else
        //    {
        //    foreach(var kvp in effectiveMountsByReadPriority)
        //    {

        //        if(kvp.Value.Count > 1)
        //        {
        //            throw new NotImplementedException("Equal mount read priorities");
        //        }
        //        else if(kvp.Value.Count==1)
        //        {
        //            Mount mount = kvp.Value.First();


        //            mount.RootHandle.Reference.Path
        //        }
        //    }
        //    }

        //    Vob vob;

        //    for(vob = this; vob != null; vob = vob.Parent)
        //    {
        //        mounts.Add(vob.Mounts)
        //    }
        //    //foreach (var mount in Mounts.GetMountsForPath(reference.Path))
        //    //{
        //    //    mount.RootObject
        //    //}
        //    return null;
        //}

        #endregion

        #region ...

        #endregion

        #endregion

        #region Children List Accessor

        ///// <summary>
        ///// TODO
        ///// If true, keep the Children list up to date with the children from the underlying mounts
        ///// </summary>
        //public bool SyncChildrenReferences
        //{
        //    get;
        //    set;
        //}

#if !AOT
        public void RetrieveChildrenReferences()
        {
            foreach (var mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
            {
                mount.RootHandle.GetChildrenNames();
            }
        }
#endif

        #region OBase Children Implementation

        // IDEA: Cool idea: Do Task.WhenAll, and populate a custom list class with an "IsDone" option, and maybe download stats like the new windows copy dialog

        public IEnumerable<string> GetChildrenNames(bool includeHidden = false, bool persistedOnly = true)
        {
            HashSet<string> namesDiscovered = new HashSet<string>();
            //List<string> children = new List<string>();
            if (effectiveMountsByReadPriority != null) // RECENTCHANGE 140720 changed == to !=
            {
                if (InitializeEffectiveMounts())
                {
                    foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
                    {
                        foreach (string childName in GetMountHandle(mount).GetChildrenNames())
                        {
                            if (!includeHidden && LionPath.IsHidden(childName))
                            {
                                continue;
                            }

                            if (namesDiscovered.Contains(childName))
                            {
                                continue;
                            }

                            namesDiscovered.Add(childName);

                            yield return childName;
                        }
                    }
                }
            }
            foreach (var childName in children.Keys)
            {
                if (namesDiscovered.Contains(childName))
                {
                    continue;
                }

                var child = children[childName].Target;
                if (child == null)
                {
                    continue;
                }

                if (persistedOnly && !child.Handles.Where(h => h.IsPersisted).Any())
                {
                    continue;
                }

                namesDiscovered.Add(childName);
                yield return childName;
            }
        }

        public IEnumerable<string> GetChildrenNamesOfType(Type childType)
        {
            HashSet<string> namesDiscovered = new HashSet<string>();

            foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
            {
                foreach (var c in GetMountHandle(mount).GetChildrenNamesOfType(childType))
                {
                    if (namesDiscovered.Contains(c))
                    {
                        continue;
                    }

                    namesDiscovered.Add(c);

                    yield return c;
                }
            }
            //l.Warn("PARTIALLY IMPLEMENTED: GetChildrenNamesOfType - does not filter types");
            //return GetChildrenNames();
        }

        public IEnumerable<string> GetChildrenNamesOfType<ChildType>()
             where ChildType : class, new()
        {
            HashSet<string> namesDiscovered = new HashSet<string>();

            foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
            {
                foreach (var c in GetMountHandle(mount).GetChildrenNamesOfType<ChildType>())
                {
                    if (namesDiscovered.Contains(c))
                    {
                        continue;
                    }

                    namesDiscovered.Add(c);
                    yield return c;
                }
            }
            //l.Warn("PARTIALLY IMPLEMENTED: GetChildrenNamesOfType - does not filter types");
            //return GetChildrenNames();
        }

        //private void RetrieveChildrenHandles()
        //{
        //    List<string> children = new List<string>();
        //    foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
        //    {

        //    }
        //    foreach (var c in GetMountHandle(mount).GetChildrenOfType<ChildType>())
        //    {
        //        yield return c;
        //    }
        //    //return children;
        //}

        public IEnumerable<Vob> GetChildren() => children.Values.Select(wr => wr.IsAlive ? wr.Target : null).Where(h => h != null);

#if !AOT
        public IEnumerable<VobHandle<ChildType>> GetVobChildrenOfType<ChildType>()
            where ChildType : class, new()
        {
            foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
            {
                foreach (var c in GetMountHandle(mount).GetChildrenOfType<ChildType>())
                {
                    yield return new VobHandle<ChildType>(new Vob(vos, this, c.Reference.Name));
                }
            }
        }
#endif

        public IEnumerable<IHandle> GetChildrenOfType(Type childType)
        {
            //List<IHandle<ChildType>> children = new List<IHandle<ChildType>>();
            foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
            {
                foreach (var c in GetMountHandle(mount).GetChildrenOfType(childType))
                {
                    yield return c;
                }
            }
            //return children;

            //return this.children.Values.Select(wr => wr.IsAlive ? wr.Target : null).Where(h => h != null).Select(vob => vob.AsVobType<ChildType>());
            //l.Warn("PARTIALLY IMPLEMENTED: GetChildrenNamesOfType - does not filter types");
        }

#if !AOT
        public IEnumerable<IHandle<ChildType>> GetChildrenOfType<ChildType>()
            where ChildType : class//, new()
        {
            //List<IHandle<ChildType>> children = new List<IHandle<ChildType>>();
            foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
            {
                var mountHandle = GetMountHandle(mount);

                foreach (var c in mountHandle.GetChildrenOfType<ChildType>())
                {
                    yield return c;
                }
            }
            //return children;

            //return this.children.Values.Select(wr => wr.IsAlive ? wr.Target : null).Where(h => h != null).Select(vob => vob.AsVobType<ChildType>());
            //l.Warn("PARTIALLY IMPLEMENTED: GetChildrenNamesOfType - does not filter types");
        }
#endif

#if !AOT
        public IEnumerable<VobHandle<ChildType>> GetVobHandleChildrenOfType<ChildType>()
            where ChildType : class, new()
        {
            //List<IHandle<ChildType>> children = new List<IHandle<ChildType>>();
            foreach (Mount mount in effectiveMountsByReadPriority.Values.SelectMany(x => x))
            {
                foreach (var c in GetMountHandle(mount).GetChildrenOfType<ChildType>())
                {
                    var vh = this[c.Reference.Name].ToHandle<ChildType>();
                    vh.Mount = mount;
                    //l.LogCritical("TEMP GetVobHandleChildrenOfType: " + vh + " mount: " + mount);
                    yield return vh;
                }
            }
            //return children;

            //return this.children.Values.Select(wr => wr.IsAlive ? wr.Target : null).Where(h => h != null).Select(vob => vob.AsVobType<ChildType>());
            //l.Warn("PARTIALLY IMPLEMENTED: GetChildrenNamesOfType - does not filter types");
        }
#endif

        protected Vob CreateChild(string childName) // TODO
        {
            Vob parent = this;
            return new Vob(vos, parent, childName);
        }
        //protected override Vob CreateChild(Vob parent, string childName)
        //{
        //    return new Vob(this.vos, (Vob)parent, childName);
        //}

        #endregion

        #endregion

        //#region Handle OLD

        //public IHandle Handle
        //{
        //    get
        //    {
        //        if (handle == null)
        //        {
        //            handle = this.ToHandle();
        //        }
        //        return handle;
        //    }
        //} private IHandle handle;

        //#endregion

        //#region IMultiTyped OLD

        //public object this[Type type]
        //{
        //    get { return handle[type]; }
        //}

        //public ChildType AsType<ChildType>() where ChildType : class
        //{
        //    return handle.AsType<ChildType>();
        //}

        //public ChildType[] OfType<ChildType>() where ChildType : class
        //{
        //    return handle.OfType<ChildType>();
        //}

        //public object[] SubTypes
        //{
        //    get { return handle.SubTypes; }
        //}

        //#endregion

        //public Vob<T> FilterAsType<T>()
        //{

        //}

        #region Index accessors (GetChild)

        public Vob this[string subpath]
        {
            get
            {
                if (subpath == null)
                {
                    return this;
                }

                return this[0, subpath.ToPathArray()];
            }
        }

        public Vob this[IEnumerator<string> subpathChunks] => GetChild(subpathChunks);

        public Vob this[IEnumerable<string> subpathChunks] => GetChild(subpathChunks);

        public Vob this[int index, string[] subpathChunks] => GetChild(subpathChunks, index);

        public Vob this[params string[] subpathChunks] => GetChild(subpathChunks);

        #endregion


        #region Cached Children

        // TODO: Change to ConcurrentDictionary
        [Ignore]
        protected MultiBindableDictionary<string, LionFire.Structures.WeakReferenceX<Vob>> children = new MultiBindableDictionary<string, LionFire.Structures.WeakReferenceX<Vob>>();

        public readonly object SyncRoot = new object();

        public bool CacheChildren
        {
            get => children != null;
            set
            {
                if (value == CacheChildren)
                {
                    return;
                }

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

        private void CleanDeadChildReferences()
        {
            IEnumerable<KeyValuePair<string, WeakReferenceX<Vob>>> ce = children;

            foreach (var kvp in ce.ToArray())
            {
                if (!kvp.Value.IsAlive)
                {
                    children.Remove(kvp.Key);
                }
            }
        }

        #region Child accessor

        #region Get / Query Logic

        //protected abstract Vob CreateChild(string childName);

        private Vob GetChild(IEnumerable
#if !AOT
<string>
#endif
 subpathChunks) => GetChild(subpathChunks.GetEnumerator());

        // DUPLICATED - Similar logic as GetChild and QueryChild
        private Vob GetChild(IEnumerator
#if !AOT
<string>
#endif
 subpathChunks)
        {
            if (subpathChunks == null)
            {
                return this;
            }

            Vob child;

            if (!subpathChunks.MoveNext() || StringX.IsNullOrWhiteSpace(subpathChunks.Current
#if AOT
                as string
#endif
))
            {
                return this;
            }

            string childName = subpathChunks.Current
#if AOT
 as string
#endif
;

            lock (SyncRoot)
            {
                if (childName == "..")
                {
                    child = Parent;
                }
                else if (childName == ".")
                {
                    child = this;
                }
                else
                {
                    var wVob = children.TryGetValue(childName);
                    if (wVob != null)
                    {
                        if (!wVob.IsAlive)
                        {
                            //vob = new ChildType(this.vos, this, childName);
                            child = CreateChild(childName);
                            wVob.Target = child;
                        }
                        else
                        {
                            child = wVob.Target;
                        }
                    }
                    else
                    {
                        child = CreateChild(childName);
                        children.Add(childName, new WeakReferenceX<Vob>(child));
                    }
                }
            }

            return child.GetChild(subpathChunks);

            //if (!subpathChunks.MoveNext())
            //{
            //    return child;
            //}
            //else
            //{
            //    return child[subpathChunks];
            //}
        }

        // DUPLICATED - Similar logic as GetChild and QueryChild
        internal Vob GetChild(string[] subpathChunks, int index)
        {
            Vob vob;

            if (subpathChunks == null || subpathChunks.Length == 0)
            {
                return this;
            }

            string childName = subpathChunks[index];

            lock (SyncRoot)
            {
                if (childName == "..")
                {
                    vob = Parent ?? this;
                }
                else if (childName == ".")
                {
                    vob = this;
                }
                else
                {
                    var wVob = children.TryGetValue(childName);
                    if (wVob != null)
                    {
                        if (!wVob.IsAlive || wVob.Target == null)
                        {
                            vob = CreateChild(childName);
                            wVob.Target = vob;
                        }
                        else
                        {
                            vob = wVob.Target;
                        }
                    }
                    else
                    {
                        vob = CreateChild(childName);
                        children.Add(childName, new WeakReferenceX<Vob>(vob));
                    }
                }
            }
#if SanityChecks
            if (vob == null) throw new UnreachableCodeException("vob == null");
#endif
            if (index == subpathChunks.Length - 1)
            {
                return vob;
            }
            else
            {
                return vob[index + 1, subpathChunks];
            }
        }

        // DUPLICATED - Similar logic as GetChild
        public Vob QueryChild(string[] subpathChunks, int index)
        {
            if (subpathChunks == null || subpathChunks.Length == 0)
            {
                return this;
            }

            Vob vob;

            string childName = subpathChunks[index];

            if (childName == "..")
            {
                vob = Parent;
            }
            else if (childName == ".")
            {
                vob = this;
            }
            else
            {
                var wVob = children.TryGetValue(childName);
                if (wVob != null)
                {
                    if (wVob.IsAlive)
                    {
                        vob = wVob.Target;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }

            if (index == subpathChunks.Length - 1)
            {
                return vob;
            }
            else
            {
                return vob.QueryChild(subpathChunks, index + 1);
            }
        }

        #endregion

        #region Children by VosReference

        public Vob this[VosReference reference] => this[reference.Path];

        public Vob GetChild(VosReference reference) => GetChild(reference.Path.ToPathArray(), 0);

        public Vob QueryChild(VosReference reference) => QueryChild(reference.Path.ToPathArray(), 0);

        #endregion

        #endregion

        #endregion

        #region Information


        #region Read

        public string DumpNonVirtualReadLocations
        {
            get
            {
                return GetNonVirtualReadLocations
#if NET35
.Cast<object>()
#endif
                .ToStringList();
            }
        }
        public IEnumerable<IReference> GetNonVirtualReadLocations
        {
            get
            {
                foreach (var handle in ReadHandles)
                {
                    var references = handle.ToNonVirtualReferences();
                    foreach (var reference in references)
                    {
                        if (reference != null)
                        {
                            yield return reference;
                        }
                    }
                }
            }
        }

        #endregion

        #region Write

        public string DumpNonVirtualWriteLocations
        {
            get
            {
                return GetNonVirtualWriteLocations
#if NET35
                .Cast<object>()
#endif
                .ToStringList();
            }
        }
        public IEnumerable<IReference> GetNonVirtualWriteLocations
        {
            get
            {
                foreach (var handle in WriteHandles)
                {
                    var references = handle.ToNonVirtualReferences();
                    foreach (var reference in references)
                    {
                        if (reference != null)
                        {
                            yield return reference;
                        }
                    }
                }
            }
        }

        #endregion


        #endregion


        #region Misc

        public override bool Equals(object obj)
        {
            var other = obj as Vob;
            if (other == null)
            {
                return false;
            }
#if DEBUG
            if (Path == other.Path && this != other) { l.TraceWarn("Two Vobs with same path but !="); }
            if (Path == other.Path && !object.ReferenceEquals(this, other)) { l.TraceWarn("Two Vobs with same path but !ReferenceEquals"); }
#endif
            return Path == other.Path;
        }

        public override int GetHashCode() => Path.GetHashCode();

        public override string ToString() => Path;

        private static ILogger l = Log.Get();
        private static ILogger lSave = Log.Get("LionFire.Vos.Vob.Save");
        private static ILogger lLoad = Log.Get("LionFire.Vos.Vob.Load");

        #endregion


        internal void Cleanup()
        {
            // TODO: Delete this and parent VOB nodes if they are no longer useful.  TODO: Instead, do weak references
        }


        //        internal static void DeleteChildren(VobFilter filter)
        //        {
        //            throw new NotImplementedException();
        //        }

        #region TreeLastModified

        public void UpdateTreeLastModified()
        {
            var vh = VHTreeLastModified;
            vh.ObjectOrCreate.DateTime = DateTime.Now;
            vh.Save();
        }

        public VobHandle<TimeStamp> VHTreeLastModified => this[VosPaths.MetaDataSubPath]["TreeLastModified"].GetHandle<TimeStamp>();
        public DateTime? GetTreeLastModified()
        {
            VHTreeLastModified.ForgetObject();
            var obj = VHTreeLastModified.Object;
            //if (obj != null) { l.Trace("[lastmodified] " + obj.DateTime.ToStringSafe() + " " + this); }
            return obj == null ? null : (DateTime?)obj.DateTime;
        }

        #endregion
    }

    /// <summary>
    /// Wrapper for DateTime
    /// </summary>
    public class TimeStamp
    {
        public DateTime DateTime { get; set; }
        public TimeStamp() { }
        public TimeStamp(DateTime dateTime) { DateTime = dateTime; }
        public static implicit operator TimeStamp(DateTime dateTime) => new TimeStamp(dateTime);
        public static implicit operator DateTime(TimeStamp timeStamp) => timeStamp.DateTime;
    }

    public static class VobExtensions
    {
        public static bool IsAncestorOf(this Vob potentialAncestor, Vob potentialChild)
        {
            for (Vob vobParent = potentialChild.Parent; vobParent != null; vobParent = vobParent.Parent)
            {
                if (Object.ReferenceEquals(vobParent, potentialAncestor))
                {
                    return true;
                }
            }
            return false;
        }
    }


    //// Is this a good idea?  Maybe use non-generic class Vob: Vob.AsType{T}().  Use VobHandle<> instead?
    //public class Vob<T> :
    //     IHandle<T> // REVIEW: IHandle<> vs IHasHandle<>
    //    where T : class, new()
    //{
    //    private Vob vob;
    //    private Vos vos;

    //    #region Construction

    //    public Vob(Vob mainVob) { this.vob = mainVob; }

    //    //public Vob(Vos vos, Vob vob, IHandle<T> objectHandle)
    //    //{
    //    //    // TODO: Complete member initialization
    //    //    this.vos = vos;
    //    //    this.vob = vob;
    //    //    this.objectHandle = objectHandle;

    //    //}

    //    //REVIEW TODO: Provide this ctor?  also in base?
    //    //public Vob(VosReference reference) : base(vos, parent, name) { this.Reference = reference; }

    //    #endregion

    //    //Handle<T> handle;

    //    #region Handle pass-through

    //    public void AssignFrom(T other)
    //    {
    //        vob.AssignFrom(other);
    //    }

    //    public T Object
    //    {
    //        get
    //        {
    //            return vob.AsType<T>();
    //        }
    //        set
    //        {
    //            vob.Object = value;
    //        }
    //    }

    //    object IHandle.Object
    //    {
    //        get
    //        {
    //            return vob.Object;
    //        }
    //        set
    //        {
    //            vob.Object = value;
    //        }
    //    }

    //    public bool HasObject
    //    {
    //        get { return vob.HasObject; }
    //    }

    //    public event Action<IChangeableReferencable> ReferenceChanged
    //    {
    //        add { vob.ReferenceChanged += value; }
    //        remove { vob.ReferenceChanged -= value; }
    //    }

    //    public void AssignFrom(object other)
    //    {
    //        vob.AssignFrom(other);
    //    }

    //    public bool RetrieveOnDemand
    //    {
    //        get
    //        {
    //            return vob.RetrieveOnDemand;
    //        }
    //        set
    //        {
    //            vob.RetrieveOnDemand = value;
    //        }
    //    }

    //    public void Retrieve()
    //    {
    //        vob.Retrieve();
    //    }

    //    public object TryRetrieve()
    //    {
    //        return vob.TryRetrieve();
    //    }

    //    public void EnsureRetrieved()
    //    {
    //        vob.EnsureRetrieved();
    //    }

    //    public bool TryEnsureRetrieved()
    //    {
    //        return vob.TryEnsureRetrieved();
    //    }

    //    public void RetrieveOrCreate()
    //    {
    //        vob.RetrieveOrCreate();
    //    }

    //    public void RetrieveOrCreateDefault(object defaultValue)
    //    {
    //        vob.RetrieveOrCreateDefault(defaultValue);
    //    }

    //    public void ConstructDefault()
    //    {
    //        vob.ConstructDefault();
    //    }

    //    public void Delete()
    //    {
    //        vob.Delete();
    //    }

    //    public void Move(IReference newReference)
    //    {
    //        vob.Move(newReference);
    //    }

    //    public void Copy(IReference newReference)
    //    {
    //        vob.Copy(newReference);
    //    }

    //    public bool AutoSave
    //    {
    //        get
    //        {
    //            return vob.AutoSave;
    //        }
    //        set
    //        {
    //            vob.AutoSave = value;
    //        }
    //    }

    //    public void Save()
    //    {
    //        vob.Save();
    //    }

    //    public void RetrieveOrCreateDefault(T defaultValue)
    //    {
    //        vob.RetrieveOrCreateDefault(defaultValue);
    //    }

    //    public IEnumerable<IHandle> GetChildren()
    //    {
    //        return vob.GetChildren();
    //    }

    //    public IEnumerable<IHandle<ChildType>> GetChildrenOfType<ChildType>() where ChildType : class, new()
    //    {
    //        return vob.GetChildrenOfType<ChildType>();
    //    }

    //    public IEnumerable<string> GetChildrenNamesOfType<ChildType>() where ChildType : class, new()
    //    {
    //        return vob.GetChildrenNamesOfType<ChildType>();
    //    }

    //    #endregion

    //    public IHandle this[string subpath]
    //    {
    //        get { return vob[subpath]; }
    //    }

    //    public IHandle this[IEnumerator<string> subpathChunks]
    //    {
    //        get { return vob[subpathChunks]; }
    //    }

    //    public IHandle this[IEnumerable<string> subpathChunks]
    //    {
    //        get { return vob[subpathChunks]; }
    //    }

    //    public IHandle this[int index, string[] subpathChunks ]
    //    {
    //        get { return vob[index, subpathChunks]; }
    //    }

    //    public IHandle this[params string[] subpathChunks]
    //    {
    //        get { return vob[subpathChunks]; }
    //    }

    //    public IReference Reference
    //    {
    //        get
    //        {
    //            return vob.Reference;
    //        }
    //        set
    //        {
    //            vob.Reference = value;
    //        }
    //    }


    //    bool IHandlePersistence.TryRetrieve()
    //    {
    //        return ((IHandlePersistence)vob).TryRetrieve();
    //    }

    //    public bool TryDelete()
    //    {
    //        return vob.TryDelete();
    //    }

    //    public IEnumerable<string> GetChildrenNames()
    //    {
    //        return vob.GetChildrenNames();
    //    }

    //    public IHandle Handle
    //    {
    //        get
    //        {
    //            return vob.Handle;
    //        }
    //        set
    //        {
    //            vob.Handle = value;
    //        }
    //    }

    //    public object this[Type type]
    //    {
    //        get { return vob[type]; }
    //    }

    //    public T AsType<T>() where T : class
    //    {
    //        return vob.AsType<T>();
    //    }

    //    public T[] OfType<T>() where T : class
    //    {
    //        return vob.OfType<T>();
    //    }

    //    public object[] SubTypes
    //    {
    //        get { return vob.SubTypes; }
    //    }
    //}
}
