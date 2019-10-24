
#define OPTIMIZEMEM
//#define CACHE_OBJECT 
#define ALLOW_UNPATHED_VOBHANDLES // TEMP, for JsonEx serializer

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Serialization;
using LionFire.Assets;
using System.ComponentModel;
using LionFire.Vos.Filesystem;
using System.Reflection;
//using System.IO;

namespace LionFire.Vos
{


#if UNUSED
    public class VobHandleBase 
    {
        public IReference Reference
        {
            get { return VosReference; }
        }
        public VosReference VosReference
        {
            get
            {
                if (Path == null) return null;
                return new VosReference(Path)
                    {
                        Package = this.Package,
                        Location = this.Location,
                    }; // Use VosReference instead of storing path as a field here?  Use VosReference as a struct?
            }
        }

    #region Package

        public string Package
        {
            get { return package; }
            set
            {
                if (package == value) return;
                if (package != default(string)) throw new NotSupportedException("Package can only be set once.");
                package = value;
            }
        } private string package;

    #endregion

    #region Location

        public string Location
        {
            get { return location; }
            set
            {
                if (location == value) return;
                if (location != default(string)) throw new NotSupportedException("Location can only be set once.");
                location = value;
            }
        } private string location;

    #endregion

        public string Name
        {
            get
            {
                if (Path != null)
                {
                    return VosPath.GetName(Path);
                }
                else
                {
                    return null;
                }
            }
            //set
            //{
            //    if (Path != null)
            //    {
            //        throw new AlreadyException("Path & Name can only be set once");
            //    }
            //    Path = AssetPath.PathFromName<ObjectType>(value); 
            //}
        }

        public string Path
        {
            get
            {
#if OPTIMIZEMEM
                if (path == null)
                {
                    if (vob != null)
                    {
                        path = vob.Path;
                    }
                }
                return path;
#else
                return vob == null ? vob.Path :null;
#endif
            }
            set
            {

#if OPTIMIZEMEM
                if (Path == value) { return; }
                else if (vob != null || path != null)
                {
                    throw new AlreadyException("Target can only be set once");
                }
                path = value;
#else
                Vob = Vos.Default[value];
#endif

            }
        }
#if OPTIMIZEMEM
        protected string path;
#endif

        [Ignore]
        public Vob Vob
        {
            get
            {
                if (vob == null)
                {
#if OPTIMIZEMEM
                    if (path != null)
                    {
                        vob = Vos.Default[Path];
                    }
#endif
                    //        if (Path != null)
                    //        {
                    //            vob = Vos.Default[Path];
                    //            //if (obj != null)
                    //            //{
                    //            //    // REVIEW:
                    //            //    vob.Object = this.obj;
                    //            //}
                    //        }
                }
                return vob;
            }
            set
            {
                if (vob != null && vob != value) { throw new AlreadyException("Target can only be set once"); }
                if (path != null && value != null && value.Path != path) { throw new AlreadyException("Target can only be set once"); }

                vob = value;

                //if (Vob != null)
                //{
                //    if (Vob == value) { return; }
                //    else
                //    {
                //        throw new AlreadyException("Vob can only be set once");
                //    }
                //}
                vob = value;
            }
        } protected Vob vob;

        public override string ToString()
        {
            // TODO: Layer
            if (vob != null) return vob.ToString();
            if (path != null) return String.Concat(">", path, "<");
            return ">null<";
        }
    }
#endif
    //    /// <summary>
    //    /// Has a reference to a Vob.  Useful for serializing
    //    /// </summary>
    //    [LionSerializable(SerializeMethod.ByValue)]
    //    [JsonConvert(typeof(VobHandleSerializationConverter))]
    //    public class VobHandle : VobHandleBase, IVobHandle
    //    {

    //        #region Construction

    //        public VobHandle() { }
    //        public VobHandle(string path) { this.vob = Vos.Default[path]; }
    //        public VobHandle(Vob vobParent, string subPath) { this.vob = vobParent[subPath]; }
    //        public VobHandle(Vob vob) { this.vob = vob; }

    //        public VobHandle(VosReference path)
    //        {
    //            this.vob = Vos.Default[path.Path];
    //        }

    //        public static implicit operator VobHandle(Vob vob)
    //        {
    //            return new VobHandle(vob);
    //        }

    //        #endregion

    //        [Ignore]
    //        public object Object
    //        {
    //#if CACHE_OBJECT
    //            get { if(_object == null && Vob != null){ _object = Vob.Object; }return _object; } 
    //#else
    //            get { return Vob == null ? null : Vob.Object; }
    //#endif
    //        }
    //#if CACHE_OBJECT
    //        private object _object;
    //#endif

    //        #region Mount

    //        public Mount Mount
    //        {
    //            get { return mount; }
    //            set
    //            {
    //                if (mount == value) return;
    //                if (value != null && mount != default(Mount)) throw new NotSupportedException("Mount can only be set once, unless it is set back to null.");
    //                mount = value;
    //            }
    //        } private Mount mount;

    //        #endregion
    //    }

#if false // FUTURE ENH
    /// <summary>
    /// Readonly Vob Handle.  Can be used on abstract and interface types
    /// </summary>
    /// <typeparam name="ObjectType"></typeparam>
    public class VobReadHandle<ObjectType>
        : IVobReadHandle<ObjectType>
        where ObjectType : class 
    {

    }

#endif


    /// <summary>
    /// A lightweight reference to a Vob, with properties for retrieving and
    /// caching the Vob and its object of the type specified by generic parameter.
    /// Rename to OHandle/Handle?
    /// </summary>
    /// <typeparam name="ObjectType"></typeparam>
    [LionSerializable(SerializeMethod.ByValue)]
    [JsonConvert(typeof(VobHandleSerializationConverter))]
    public class VobHandle<ObjectType> : HandleBase<ObjectType>
        , IVobHandle
#if AOT
//		, IVobReadHandle - doesn't exist yet
#else
, IVobReadHandle<ObjectType>
#endif
 where ObjectType : class //,new() // TODO: Try removing new() restriction?
    {
        #region Retrieve Info

        public override RetrieveInfo RetrieveInfo { get { return VosRetrieveInfo; } set { VosRetrieveInfo = (VosRetrieveInfo)value; } }
        public VosRetrieveInfo VosRetrieveInfo { get; set; }
        public override bool IsRetrieveInfoEnabled {
            get { return VosRetrieveInfo != null; }
            set {
                if (value)
                {
                    if (VosRetrieveInfo == null) { VosRetrieveInfo = new VosRetrieveInfo(); }
                }
                else
                {
                    VosRetrieveInfo = null;
                }
            }
        }

        #endregion

        #region From VobHandleBase

        public override IReference Reference {
            get { return VosReference; }
        }

        //[Assignment(AssignmentMode.Assign)]
        [Ignore]
        public VosReference VosReference {
            get {
                if (Path == null) return null;

                var vosRef = new VosReference(Path)
                {
                    Package = this.Package,
                    Location = this.Store,
                }; // Use VosReference instead of storing path as a field here?  Use VosReference as a struct?
                return vosRef;

            }
            set {
            }
        }



        #region Mount

        protected override IReference MountReference {
            get {
                var path = MountPath;
                if (path == null) return null;
                return new VosReference(path);
            }
        }

        public string MountPath {
            get {
                if (Mount == null || Vob == null) return null;
                var result = Vob.GetMountPath(Mount);
                return result;
            }
        }

        [Assignment(AssignmentMode.Assign)]
        public Mount Mount {
            get { return mount; }
            set {
                if (mount == value) return;
                if (value != null && mount != default(Mount)) throw new NotSupportedException("Mount can only be set once, unless it is set back to null.");
                mount = value;
            }
        }
        private Mount mount;

        #endregion

        #region Package

        public string EffectivePackage {
            get {
                if (Mount != null && Mount.Package != null) return Mount.Package;
                return Package;
            }
        }
        public string Package {
            get {
                //if (Mount != null && Mount.Package != null) return Mount.Package;
                return package;
            }
            set {
                if (package == value) return;
                if (package != default(string)) throw new NotSupportedException("Package can only be set once.");
                package = value;
            }
        }
        private string package;

        #endregion

        #region Store

        public string EffectiveStore {
            get {
                if (Mount != null && Mount.Store != null) return Mount.Store;
                return Store;
            }
        }
        public string Store {
            get {
                if (Mount != null && Mount.Store != null) return Mount.Store;
                return store;
            }
            set {
                if (store == value) return;
                if (store != default(string)) throw new NotSupportedException("Store can only be set once.");
                store = value;
            }
        }
        private string store;

        #endregion

        public string Name {
            get {
                if (Path != null)
                {
                    return LionPath.GetName(Path);
                }
                else
                {
                    return null;
                }
            }
            //set
            //{
            //    if (Path != null)
            //    {
            //        throw new AlreadyException("Path & Name can only be set once");
            //    }
            //    Path = AssetPath.PathFromName<ObjectType>(value); 
            //}
        }

        public string Path {
            get {
#if OPTIMIZEMEM
                if (path == null)
                {
                    if (vob != null)
                    {
                        path = vob.Path;
                    }
                }
                return path;
#else
                return vob == null ? vob.Path :null;
#endif
            }
            set {

#if OPTIMIZEMEM
                if (Path == value) { return; }
                else if (vob != null || path != null)
                {
                    throw new AlreadyException("Path/Vob can only be set once");
                }
                path = value;
#else
                Vob = Vos.Default[value];
#endif

            }
        }
#if OPTIMIZEMEM
        protected string path;
#endif

        [Ignore]
        public Vob Vob {
            get {
                if (vob == null)
                {
#if OPTIMIZEMEM
                    if (path != null)
                    {
                        vob = VBase.Default[Path];
                    }
#endif
                    //        if (Path != null)
                    //        {
                    //            vob = Vos.Default[Path];
                    //            //if (obj != null)
                    //            //{
                    //            //    // REVIEW:
                    //            //    vob.Object = this.obj;
                    //            //}
                    //        }
                }
                return vob;
            }
            set {
                if (vob != null && vob != value) { throw new AlreadyException("Target can only be set once"); }
                if (path != null && value != null && value.Path != path) { throw new AlreadyException("Path/Vob can only be set once"); }

                vob = value;

                //if (Vob != null)
                //{
                //    if (Vob == value) { return; }
                //    else
                //    {
                //        throw new AlreadyException("Vob can only be set once");
                //    }
                //}
                vob = value;
            }
        }
        [Ignore]
        protected Vob vob;

        //public override string ToString()
        //{
        //    // TODO: Layer
        //    if (vob != null) return vob.ToString();
        //    if (path != null) return String.Concat(">", path, "<");
        //    return ">null<";
        //}
        #endregion

        #region Construction

#if ALLOW_UNPATHED_VOBHANDLES
        public bool IsParented {
            get {
                return Path != null;
            }
        }
#else
        public bool IsParented
        {
            get
            {
                return true;
            }
        }
#endif

        static VobHandle()
        {
            if (typeof(IVobHandle).IsAssignableFrom(typeof(ObjectType)))
            {
                l.LogCritical("Invalid VH type: " + typeof(ObjectType).FullName + " at " + Environment.StackTrace);
                //throw new ArgumentException("Cannot create VobHandles that point to VobHandles.");
            }
        }

#if ALLOW_UNPATHED_VOBHANDLES
        public VobHandle() { }
#endif

        /// <summary>
        /// Defers accessing (which may involve creating) the Vob
        /// </summary>
        /// <param name="path"></param>
        public VobHandle(string path)
        {
            this.path = path;
        }

        public VobHandle(Vob vob) { this.vob = vob; }

        public VobHandle(Vob vobParent, string subPath)
        {
            if (vobParent == null)
                vobParent = VBase.Default.Root;
            this.vob = vobParent[subPath];
        }

        public VobHandle(IVobHandle vobHandleParent, params string[] subPathChunks)
        {
            //if (vobHandleParent == null)
            //vobHandleParent = Vos.Default.Root.ToHandle();
            //this.vob = vobHandleParent.Vob[subPathChunks];
            this.vob = vobHandleParent.Vob[subPathChunks];
        }

        public VobHandle(params string[] subPathChunks)
        {
            //var vobHandleParent = new VobHandle(Vos.Default.Root); 
            //this.vob = vobHandleParent.Vob[subPathChunks];
            this.vob = VBase.Default.Root[subPathChunks];
        }

        //public VobHandle(string path, TValue obj)
        //    : this(path)
        //{
        //    this.Vob.Object = obj;
        //    this.obj = obj;
        //}

#if ALLOW_UNPATHED_VOBHANDLES
        public VobHandle(ObjectType obj)
        {
            this.Object = obj;
        }
#endif

        #region Construction Operators

        public static explicit operator VobHandle<ObjectType>(string path)
        {
            return VBase.Default[path].ToHandle<ObjectType>();
            //return new VobHandle<ObjectType>(path);
        }

#if ALLOW_UNPATHED_VOBHANDLES
        public static explicit operator VobHandle<ObjectType>(ObjectType obj)
        {
            var result = new VobHandle<ObjectType>(obj);
            return result;
        }
#endif

        public static implicit operator VobHandle<ObjectType>(AssetID assetID)
        {
#if !AOT
            return AssetReferenceResolver.AssetPathToHandle<ObjectType>(assetID.Path, assetID.PackageName);
#else
			return (VobHandle<ObjectType>)AssetReferenceResolver.AssetPathToHandle(assetID.Path, assetID.PackageName, T: typeof(ObjectType));
#endif
        }

        #endregion

        #endregion

        #region Implicit Operators to other types

        public static implicit operator ObjectType(VobHandle<ObjectType> assetRef)
        {
            return assetRef == null ? null : assetRef.Object;
        }

        #endregion

        #region Derived

        public Type Type { get { return typeof(ObjectType); } }

        IVobHandle IVobHandle.GetSibling(string name)
        {
            return GetSibling(name);
        }
        public VobHandle<ObjectType> GetSibling(string name, string store = null)
        {
            if (this.Name == name) return this;
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException();
            return this.Vob.Parent[name].GetHandle<ObjectType>();
        }
        public VobHandle<ObjectType> ForStore(string mount = null)
        {
            if (store == null) return this;

            //GetSubPathChunksOfAncestor

            //if ()
            l.Warn("Not implemented: ForMountName");
            return this;
        }

        #endregion

        object IVobHandle.Object {
            get { return Object; }
            set {
                ObjectType castedObj;

                try
                {
                    castedObj = (ObjectType)value;
                }
                catch (InvalidCastException ice)
                {
                    throw new ArgumentException("This VobHandle only supports objects castable to " + typeof(ObjectType), ice);
                }

                this.Object = castedObj;
            }
        }


        //        [Ignore]
        //        public ObjectType Object
        //        {
        //            get
        //            {
        //#if CACHE_OBJECT
        //                if (_obj != null) return _obj;

        //                if (Vob != null)
        //                {
        //                    obj = Vob.AsType<TValue>();
        //                }
        //                return obj;
        //#else
        //                return Vob == null ? null : Vob.AsType<ObjectType>();
        //#endif
        //            }
        //            set
        //            {
        //#if CACHE_OBJECT
        //                if(_obj == value) return;
        //                _obj = value;
        //                throw new NotImplementedException();
        //#else
        //                if (Vob == null)
        //                {
        //                    throw new NotSupportedException("Cannot set object when Vob == null");
        //                }
        //                else
        //                {
        //                    Vob.SetType<ObjectType>(value);
        //                }
        //#endif
        //            }

        //        }
        //#if CACHE_OBJECT
        //        private TValue _obj;
        //#endif

        //public override bool TryReload(bool setToNullOnFail = true)
        //{
        //}

        public override void DiscardObject()
        {
            base.ForgetObject();
            if (ForgetMountOnForgetObject) { this.Mount = null; }
        }
        private const bool ForgetMountOnForgetObject = true;

        public bool CanWriteToReadSource()
        {
            if (!IsRetrieveInfoEnabled)
            {
                IsRetrieveInfoEnabled = true;
                if (!TryRetrieve()) return false;
            }
            else
            {
                if (!TryEnsureRetrieved()) return false;
            }
            if (Object == null) return false;
#if SanityChecks
            if (VosRetrieveInfo?.Mount?.MountOptions == null) throw new UnreachableCodeException();
#endif
            if (VosRetrieveInfo.Mount.MountOptions.IsReadOnly) return false;

            var childVH = VosRetrieveInfo.ReadHandle as IVobHandle;
            if (childVH != null) return childVH.CanWriteToReadSource(); // FUTURE REVIEW: Put CanWriteToReadSource on IHandle and implement elsewhere?

            return true;
        }

        public override bool? CanDelete()
        {
            return CanWriteToReadSource();
        }

        public override bool TryRetrieve(bool setToNullOnFail = true)
        {

#if SanityChecks
            if (Vob.Path != this.Path)
            {
                throw new UnreachableCodeException("Vob.Path != this.Path: (" + Vob.Path + " != " + this.Path + ")");
            }
#endif
#if AOT
			object objO = Vob.TryRetrieve(this);
			ObjectType obj = (ObjectType)objO;
#else
            var obj = Vob.TryRetrieve<ObjectType>(this);
#endif
            if (obj == null)
            {
                if (setToNullOnFail) this.Object = null;
                return false;
            }
            this.Object = obj;

            IRetrievedListener ire = this.Object as IRetrievedListener;
            if (ire != null) ire.OnRetrieved(this);

            return true;
        }



        #region Save

        public VobHandle<ObjectType> Save(ObjectType obj)
        {
            this.Object = obj;
            Save();
            return this;
        }
        public override void Save(bool allowDelete = false, bool preview = false)
        {


            //Vob.Save<ObjectType>(this.Object); -- NO!
#if AOT
			Vob.Save(this, typeof(ObjectType));
#else
            Vob.Save(this, allowDelete: allowDelete, preview: preview);
#endif

        }

        #endregion

        //[Ignore]
        //public Vob Vob
        //{
        //    get
        //    {
        //        //if (this.VosReference != null) // OLD - alt
        //        //{
        //        //    vob = this.VosReference.GetVob();
        //        //}

        //        if (vob == null && Path != null)
        //        {
        //            vob = Vos.Default[Path];
        //            //if (obj != null)
        //            //{
        //            //    // REVIEW:
        //            //    vob.Object = this.obj;
        //            //}
        //        }
        //        return vob;
        //    }
        //    set
        //    {
        //        if (vob != null && vob != value) { throw new AlreadyException("Target can only be set once"); }
        //        if (path != null && value != null && value.Path != path) { throw new AlreadyException("Target can only be set once"); }

        //        vob = value;

        //    }
        //} private Vob vob;

        #region Conversion

        public VobHandle<SisterType> AsVobHandleType<SisterType>(bool allowCast = true)
            where SisterType : class, new()
        {
            VobHandle<SisterType> vobHandle;
            vobHandle = new VobHandle<SisterType>(this.Vob);

            if (allowCast && typeof(SisterType).IsAssignableFrom(typeof(ObjectType)))
            {
#if CACHE_OBJECT
                vobHandle._obj = this._obj;
#endif
            }
            else
            {
                vobHandle = new VobHandle<SisterType>(this.Vob);
            }
            return vobHandle;
        }

        #endregion

        public bool Exists {
            get {
                //l.Trace("TODO: Don't load VobHandle.Object to see if it exists");
                // FUTURE: Don't load VobHandle.Object to see if it exists  OPTIMIZE SLOW
                //return this.Vob.Exists<ObjectType>(); 

                ObjectType ot = this.Object;
                return ot != null;
            }
        }

        //public void OnResolvedToHandle(VobHandle<ObjectType> vh)
        //{
        //    l.LogCritical(this + " resolved to " + vh);
        //}

        #region Vob Pass-through

        public IEnumerable<string> PathElements {
            get {
                return Vob.PathElements;
            }
        }

        #endregion

        //internal void Rename(string newName, bool deleteOld, bool saveNew, bool createIfNeeded)
        //{
        //    throw new NotImplementedException();
        //}

        #region Save

        public VobHandle<ObjectType> SaveCopyAs(string name)
        //where TValue : class, new()
        {
            var vh = CopyTo(name);
            vh.Save();
            return vh;
        }

        public enum CopyLocation
        {
            Unspecified,
            /// <summary>
            /// Copy as sibling of current vob
            /// </summary>
            Immediate,

            /// <summary>
            /// Copy as sibling within same Package
            /// </summary>
            Package,

            /// <summary>
            /// Copy as sibling within same Store
            /// </summary>
            Store,

            /// <summary>
            /// Copy as sibling within lowest-level Vos location before going to a non-Vos OBase  (usually same as store)
            /// </summary>
            LeafVob,

            /// <summary>
            /// Copy as lowest level OBase reference
            /// </summary>
            PhysicalReference,
        }

        // TODO: Change default copylocation to package once it is implemented
        // TODO: HandleType?
        // FUTURE: AllowReplace option
        public VobHandle<ObjectType> CopyTo(string name, bool allowNonClone = false, bool save = true, CopyLocation copyLocation = CopyLocation.Immediate, bool allowReplace = true)
        //where TValue : class, new()
        {
            var pathElements = this.PathElements.ToArray();
            if (pathElements.Length == 0) throw new InvalidOperationException("Cannot SaveAs when PathElements is empty");

            if (Object == null) throw new InvalidOperationException("CopyTo: Object cannot be null");

            ObjectType obj = Object as ObjectType;

            if (obj == null) throw new ArgumentException("Cannot cast Object to specified type: " + typeof(ObjectType).FullName);

            pathElements[pathElements.Length - 1] = name;

            var objCopy = this.Object.DeepCopy();

#if OLD
            //ICloneable cloneable = this.Object as ICloneable;
            //if (cloneable != null)
            //{
            //    obj = (ObjectType)cloneable.Clone();
            //}
            //else
            //{
            //    if (allowNonClone)
            //    {
            //        l.TraceWarn("CopyTo: saving same object at a new Vos location.  It is not a copy! (Implement ICloneable to avoid this) " + this.ToString());
            //    }
            //    else
            //    {
            //        throw new NotSupportedException(typeof(ObjectType).Name + " must implement ICloneable");
            //    }
            //}
#endif

            var vob = VBase.Default[pathElements];

            VobHandle<ObjectType> newVH;

            switch (copyLocation)
            {
                case CopyLocation.Unspecified:
                    throw new ArgumentException();
                case CopyLocation.Immediate:

                    // FUTURE: AllowReplace option - detect existing here?
                    newVH = vob.GetHandle<ObjectType>();

                    //if(newVH.HasObject || 
                    //newVH = new VobHandle<ObjectType>(vob);
                    break;
                case CopyLocation.Package: // Ideal TODO
                    throw new NotImplementedException();
                    break;
                case CopyLocation.Store:
                    throw new NotImplementedException();
                case CopyLocation.LeafVob:
                    throw new NotImplementedException();
                case CopyLocation.PhysicalReference:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
            if (!allowReplace && (newVH.HasObject || newVH.TryEnsureRetrieved())) // RETRIEVE
            {
                throw new AlreadyException("Already exists at destination, and allowReplace = false");
            }

            newVH.Object = objCopy;
            var objSetVH = objCopy as ICanSetVobHandle;
            if (objSetVH != null) objSetVH.VobHandle = newVH;

            //vob.Object = this.Object;

            l.Trace("Copying " + this + " ==> " + newVH);

            if (save) { Save(); }

            return newVH;
        }

        #endregion

        #region Child/Sibling Handles

        public override IHandle<T> GetHandle<T>(params string[] subpathChunks)
        //where T1:class
        {
            return this.Vob[subpathChunks].GetHandle<T>();
        }

        #endregion

        #region Misc

        public override string ToString()
        {
            // TODO: Layer

            if (vob != null) return vob.ToString() + "(" + typeof(ObjectType).Name + ")";
            if (path != null) return ">" + path + "<(" + typeof(ObjectType).Name + ")";
            return ">null<[" + typeof(ObjectType).Name + "]";
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj)) return true;

            VobHandle<ObjectType> other = obj as VobHandle<ObjectType>;
            if (other == null) return false;


            if (this.VosReference == null)
            {
                return other.VosReference == null;
            }
            else
            {
                if (!this.VosReference.Equals(other.VosReference) &&
                     (HasObject && other.HasObject && object.ReferenceEquals(Object, other.Object)))
                {
                    // Potentially dangerous
                    //l.TraceWarn("Same object, different reference");
                }

                return this.VosReference.Equals(other.VosReference)
                    //|| (HasObject&&other.HasObject && object.ReferenceEquals(Object, other.Object)) // HACK?
                    ;
            }
        }

        public override int GetHashCode()
        {
            if (VosReference == null) return 0.GetHashCode();

            return VosReference.GetHashCode();
        }

        private static ILogger l {
            get {
                if (_l == null) { _l = Log.Get(); }
                return _l;
            }
        }
        private static ILogger _l;

        #endregion


        public void Unload()
        {
            this.Object = null;
            this.IsPersisted = false;
        }

        internal void OnRenamed(VobHandle<ObjectType> newVobHandle)
        {
            l.Debug("RENAME Vob: " + this.ToString() + " --> " + newVobHandle);
        }

        #region AutoLoad

        public bool AutoLoad {
            get { return autoLoad; }
            set {
                if (autoLoad == value) return;

                autoLoad = value;

                if (autoLoad)
                {
                    // TODO HACK - move this down to FS and other OBases
                    // Create abstract MonitorEvents method or FSW class.
                    foreach (var rh in this.Vob.ReadHandles)
                    {
                        if (rh.Reference.Scheme == LionFire.Vos.Filesystem.FileReference.UriScheme)
                        {
                            watcher = rh.Reference.GetOBases().First().GetWatcher(rh.Reference);
                            if (watcher == null) continue;

                            watcher.ReferenceChangedFor += AutoLoad_WatcherGotChange;
                        }
                    }
                }
                else
                {
                    watcher.Dispose();
                    watcher = null;
                    //foreach(var fsw in fsws)
                    //{
                    //    fsw.Dispose();
                    //}
                    //fsws.Clear();
                    //fsws = null;
                }
            }
        }
        private bool autoLoad;

        IObjectWatcher watcher;

        static TimeSpan threshold = TimeSpan.FromSeconds(3.8);

        void AutoLoad_WatcherGotChange(IObjectWatcher arg1, IReference arg2)
        {
            DateTime lastSave;
            var now = DateTime.UtcNow;

            TimeSpan lastSaveAgo = default(TimeSpan);
            //foreach (var x in FsPersistence.RecentSaves.Keys)
            //{
            //    l.Trace(x);
            //}
            //l.Debug(arg2.Path);
            if (FsPersistence.RecentSaves.TryGetValue(arg2.Path, out lastSave))
            {
                lastSaveAgo = now - lastSave;
                if (lastSaveAgo < threshold)
                {
                    //l.Trace("[autoload] Just saved, ignoring changes to: " + arg2);
                    return;
                }
            }
            l.Info("[AUTOLOAD] Detected changes.  (Last save: " + lastSaveAgo.TotalSeconds + "s) Reloading: " + arg2);
            try
            {
                this.TryRetrieve();
            }
            catch (Exception ex)
            {
                l.Error("[AUTOLOAD] Failed to load changed file: " + arg2.Path + " - " + ex.ToString());
            }
        }

        #endregion

        #region Merging  (REVIEW)

        // Not recursive!

        void IVobHandle.MergeWith(IVobHandle other)
        {
            l.Warn("VobHandle<" + typeof(ObjectType).Name + ">.MergeFrom (REVIEW) " + this.ToString());
            this.Object = (ObjectType)other.Value;

            other.MergeInto(this);
        }

        void IVobHandle.MergeInto(IVobHandle other)
        {
            foreach (FieldInfo mi in this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (typeof(Delegate).IsAssignableFrom(mi.FieldType))
                {
                    var obj = mi.GetValue(this);
                    if (obj != null)
                    {
                        l.Warn(" - Merge not implemented for delegate field: " + mi.Name + " on " + this.ToString());
                    }
                }
            }
        }

        #endregion


        public void AssignPropertiesFrom(VobHandle<ObjectType> other)
        {
            this.AutoSave = other.AutoSave;
            this.AutoLoad = other.AutoLoad;
        }


    }
}
