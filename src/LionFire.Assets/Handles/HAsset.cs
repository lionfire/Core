// ASSETCACHE flag means assets are saved/loaded in memory only, not in Vos
// TODO: Separate out more VobHandle stuff and make sure it isn't called while ASSETCACHE set
#define ASSETCACHEFLAT
#define HASSETG

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using LionFire.Assets.SerializationConverters;
using LionFire.Copying;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Serialization;
using LionFire.Structures;
using LionFire.Vos;
using Microsoft.Extensions.Logging;

namespace LionFire.Assets
{

#if HASSETG

    /// <summary>
    /// A read-only handle based on an asset name (which could have a subpath (UNTESTED)) and a type.
    /// Since it is read-only, the type could be an interface type.
    /// 
    /// REVIEW: Asset is just a Type plus a name or path for that type.  For a given app, it is deterministic where
    /// that Asset exists as a VosReference.  Instead of having this class, have something that resolves
    /// an AssetReference into a VobHandle via a VosReference. ... on second thought, there is some tricky context
    /// stuff that depends on which packages are loaded, and if you want to save an Asset, you need to pick a package/store.
    /// 
    /// REVIEW: Rethink all of this.  It is nice to have documents stored in one default spot.
    /// </summary>
    /// <typeparam name="AssetType"></typeparam>
    [Assignment(AssignmentMode.Assign)]
    [LionSerializable(SerializeMethod.ByValue)]
    //[JsonConvert(typeof(HAssetSerializationConverter))] TOPORT
    public class HAsset<AssetType>
        :
        //VobHandle<AssetType>,
        //			HAssetBaseTest<AssetType>,
        //#if !AOT
        //            RH<AssetType>
        //#else
        //			IReadHandle
        //#endif
        IHAsset<AssetType>
        , INotifyPropertyChanged
        //, IChangeableReferencable // FUTURE
     where AssetType : class
    {

        #region (Static) Accessors

        public static HAsset<AssetType> Get(string assetSubPath) => assetSubPath.ToHAsset<AssetType>();

        #endregion

        #region Ontology

        public Type Type => typeof(AssetType);

        #region AssetSubPath

        /// <summary>
        /// Example: MyLoadouts/MyLoadout
        /// </summary>
        public string AssetTypePath
        {
            get => assetTypeSubPath;
            set
            {
                if (assetTypeSubPath == value)
                {
                    return;
                }

                if (value.Contains("SimpleEntities"))
                {
                    throw new ArgumentException(); // TEMP
                }

                if (assetTypeSubPath != null)
                {
                    throw new NotSupportedException("AssetSubPath can only be set once.  Use Rename instead");
                }

                assetTypeSubPath = value;

                OnAssetTypeSubPathChanged();
            }
        }
        private string assetTypeSubPath;

        #region AssetPath

        public string AssetPath
        {
            get
            {
                //var assetPath = VosPath.Combine(AssetPaths.GetAssetTypeFolder(typeof(AssetType)), this.AssetTypePath); OLD

                var assetPath = AssetPaths.AssetPathFromAssetTypePath(AssetTypePath, typeof(AssetType));
                return assetPath;
            }
        }

        /// <summary>
        ///  Example: MyLoadout
        /// </summary>
        public string AssetName
        {
            get
            {
                if (AssetTypePath == null)
                {
                    return null;
                }

                return LionPath.GetName(AssetTypePath);
            }
        }

        protected virtual void OnAssetTypeSubPathChanged()
        {
            if (vobHandle != null)
            {
                var oldVobHandle = vobHandle;
                vobHandle = null;

            }
        }

        /// <summary>
        /// TODO: For HAsset and HandleBase: once a handle is renamed it is no longer valid, but it contains a HAsset/HandleBase reference to the new handle
        /// </summary>
        /// <param name="newAssetPath"></param>
        public void Rename(string newAssetPath)
        {
            if (newAssetPath == AssetTypePath)
            {
                return;
            }

            //l.Debug("RENAME - " + ToString() + " --> " + newAssetPath);

            assetTypeSubPath = null;
            AssetTypePath = newAssetPath;

            var oldVobHandle = vobHandle;
            oldVobHandle.OnRenamed(VobHandle);

            if (HasObject)
            {
                if (VobHandle.Object != Object)
                {
                    l.Warn("RENAME - Overwriting existing object at " + VobHandle);
                }
                VobHandle.Object = Object;
            }
            else
            {
                if (VobHandle.HasObject)
                {
                    Object = VobHandle.Object;
                }
            }
        }

        #endregion

        #endregion

        #region VobHandle

        //public IVobReadHandle<AssetType> VobReadHandle => VobHandle;
        public IVobReadHandle<AssetType> VobReadHandle => throw new NotImplementedException("TOPORT");
        /// <summary>
        /// TODO: Make this a read-only VobHandle
        /// </summary>
        public IVobHandle<AssetType> VobHandle
        {
            get
            {
#if !ASSETCACHE
                if (vobHandle == null && AssetTypePath != null)
                {
                    VobHandle = GetVobHandle(VobHandlePropertyIgnoreContext);

                    //this.VobHandle = this.AssetPath.AssetNameToHandle<AssetType>(ignoreContext: IgnoreContext);
                    //l.LogCritical("HAsset getting VobHandle from name: " + AssetName + " -- " + this.VobHandle.TryEnsureRetrieved());
                }
#else
                if (vobHandle == null)
                {
                    l.Warn("Attempt to get vobHandle when ASSETCACHE is set, and VobHandle is null.  Asset may not be retrievable.");
                }
#endif
                return vobHandle;
            }
            private set
            {
#if ASSETCACHE
                l.Warn("Ignoring HAsset.set_VobHandle");
                return;
#endif
                if (vobHandle == value)
                {
                    return;
                }

                if (vobHandle != null)
                {
                    throw new NotSupportedException("VobHandle can only be set once.");
                }

                vobHandle = value;

                if (objectField != null && vobHandle != null)
                {
                    vobHandle.Object = objectField;
                }
            }
        }
        private IVobHandle<AssetType> vobHandle;

        #endregion

        #region Derived

        #region IReferencable

        public IReference Reference => AssetReference;

        public RAsset AssetReference
        {
            get
            {
                return new RAsset()
                {
                    Path = AssetTypePath,
                    Type = typeof(AssetType),
                };
            }
            //set
            //{
            //    if(value.Type 
            //    AssetReferenceNew aref = new AssetReferenceNew(value);
            //    this.AssetName = aref.Path;
            //}
        }

        #endregion

        #endregion

        #endregion

        public Type ConcreteType
        {
            get
            {
                if (concreteType != null)
                {
                    return concreteType;
                }

                if (!typeof(AssetType).IsInterface)
                {
                    return typeof(AssetType);
                }

                //var attr = typeof(AssetType).GetCustomAttribute<AssetAttribute>();
                //if (attr != null)
                //{
                //    attr.Type
                //}

                var result = concreteType ?? (typeof(AssetType).IsInterface ? null : typeof(AssetType));

                //#if SanityChecks
                //                if (result == null)
                //                {
                //                    if (typeof(AssetType).GetCustomAttribute<AssetAttribute>() == null)
                //                    {
                //                        l.Warn("AssetType is an Interface, and could not get type of asset.  This should usually be avoided.  AssetType: " + typeof(AssetType) + " " + Environment.StackTrace);
                //                    }
                //                }
                //#endif

                return result;
            }
            set
            {
                if (!typeof(AssetType).IsInterface &&
                    !typeof(AssetType).IsAssignableFrom(value)
                        //value != typeof(AssetType)
                        )
                {
                    throw new ArgumentException("This instance is of type HAsset<" + typeof(AssetType).Name + "> and cannot have its ConcreteType set to " + value.Name);
                }

                concreteType = value;
            }
        }
        private Type concreteType = null;


        #region Construction

        private void ctorFinished()
        {
            //#if SanityChecks
            //            if (ConcreteType == null)
            //            {
            //                if (typeof(AssetType).GetCustomAttribute<AssetAttribute>() == null)
            //                {
            //                    l.Trace("Try not to use HAsset<> with interface types.  typeof(AssetType).IsInterface: " + typeof(AssetType) .Name + " " + Environment.StackTrace);
            //                }
            //            }
            //#endif
        }

        public HAsset() { ctorFinished(); } // TODO - remove once JsonEx doesn't need this?

        public HAsset(AssetType obj)
        {
            if (obj == null)
            {
#if SanityChecks
                l.Warn("obj == null in HAsset(AssetType obj): " + Environment.StackTrace);
#endif
                //throw new ArgumentNullException("obj");
            }
            InitWithObject(obj);

            ctorFinished();
        }

        private void InitWithObject(AssetType obj)
        {
            if (obj == null)
            {
                return;
            }

            ConcreteType = obj.GetType();
            objectField = obj;
        }

        public HAsset(AssetType obj, string assetTypePath)
        {
            InitWithObject(obj);
            AssetTypePath = assetTypePath;
            ctorFinished();
        }

        public bool IsRegistered { get; set; }
        #region (Internal) Constructors

        internal HAsset(AssetIdentifier<AssetType> reference) : this(reference.AssetTypePath)
        {
            VobHandle = AssetReferenceResolver.ToVobHandle(reference);
        }

        /// <remarks>
        ///Internal use:  Used in deserialization; see HAssetSerializationConverter.
        /// </remarks>
        /// <param name="assetTypePath"></param>
        public HAsset(string assetTypePath)
        {
            AssetTypePath = assetTypePath;
            ctorFinished();
        }

        #endregion

        private HAsset(string assetSubPath, IVobHandle<AssetType> vobHandle)
        {
            AssetTypePath = assetSubPath;
#if SanityChecks
            if (!vobHandle.Path.EndsWith(assetSubPath))
            {
                l.Warn($"!vobHandle.Path{{{vobHandle.Path}}}.EndsWith(assetSubPath{{{assetSubPath}}})");
            }
#endif
#if !ASSETCACHE
            this.vobHandle = vobHandle;
#endif
            ctorFinished();
        }
        private HAsset(IVobHandle<AssetType> vobHandle)
        {
            l.Warn("HAsset<AssetType>(VobHandle<AssetType>) - Does not support subpaths in asset names, cannot detect failure: " + vobHandle);
            string assetName = vobHandle.Name();
            AssetTypePath = assetName;
#if !ASSETCACHE
            this.vobHandle = vobHandle;
#endif
            ctorFinished();
        }

        public event PersistenceStateChangeHandler StateChanged
        {
            add
            {
                VobHandle.StateChanged += value;
            }

            remove
            {
                VobHandle.StateChanged -= value;
            }
        }

        public event Action<RH<AssetType>, HandleEvents> HandleEvents
        {
            add
            {
                VobHandle.HandleEvents += value;
            }

            remove
            {
                VobHandle.HandleEvents -= value;
            }
        }

        public event Action<RH<AssetType>, AssetType, AssetType> ObjectReferenceChanged
        {
            add
            {
                VobHandle.ObjectReferenceChanged += value;
            }

            remove
            {
                VobHandle.ObjectReferenceChanged -= value;
            }
        }

        public event Action<RH<AssetType>> ObjectChanged
        {
            add
            {
                VobHandle.ObjectChanged += value;
            }

            remove
            {
                VobHandle.ObjectChanged -= value;
            }
        }

        #region Static and Implicit construction

#if OLD
                public HAsset(string assetSubPath, bool resolveHandleIgnoringContext) : this(assetSubPath)
        {
#if !ASSETCACHE
            this.VobHandle = GetVobHandle(resolveHandleIgnoringContext);
#endif

            //l.Trace("NEW: resolved asset " + assetName + "<" + typeof(AssetType).Name + "> to " + VobHandle);
        }
        
#else
        public static HAsset<AssetType> ToHAsset(string assetSubType, bool resolveHandleIgnoringContext)
        {
            var hAsset = assetSubType.ToHAsset<AssetType>();

#if !ASSETCACHE
            hAsset.VobHandle = hAsset.GetVobHandle(resolveHandleIgnoringContext);
#endif
            //l.Trace("NEW: resolved asset " + assetName + "<" + typeof(AssetType).Name + "> to " + VobHandle);
            return hAsset;
        }
#endif

        public static implicit operator HAsset<AssetType>(string assetTypePath)
        {

            if (assetTypePath == null) { return null; }
#if SanityChecks
            if (typeof(AssetType).IsInterface || typeof(AssetType).IsAbstract)
            {
                l.Warn($"implicit operator HAsset<AssetType>(string assetName{{{assetTypePath}}})");
            }
#endif
            // FUTURE MEMORYOPTIMIZE: Return reused & cached HAsset from HAssetCache

            //return new HAsset<AssetType>(assetTypePath: assetTypePath.AssetTypePathToAssetPath<AssetType>());
            //return new HAsset<AssetType>(assetTypePath); OLD
            return assetTypePath.ToHAsset<AssetType>();
        }

#if !AOT
        public static implicit operator HAsset<AssetType>(AssetType obj)
        {
            if (obj == null)
            {
                return null;
            }
            var hasha = obj as IHasHAsset<AssetType>;
            if (hasha != null)
            {
                //l.Trace("Avoided creating new HAsset in implicit operator AssetType => HAsset<>");
                if (hasha.HAsset == null)
                {
#if true // RECENTCHANGE - false means unit templates have no path, and we fail loadout checks
                    hasha.HAsset = new HAsset<AssetType>(obj);
#else
					return new HAsset<AssetType>(obj); // unpathed HAsset
//					throw new ArgumentException("obj is IHasHAsset, but IHasAsset.HAsset == null");
#endif

                }

                else if (!hasha.HAsset.HasObject)
                {
#if AOT
					l.Warn("operator HAsset<AssetType>(AssetType obj) using obj.HAsset.  Warning: HAsset's Object was not set to obj.  (Can't set  it): " + hasha.HAsset);
#else
                    l.Warn("operator HAsset<AssetType>(AssetType obj) using obj.HAsset.  Warning: HAsset's Object was not set to obj, so setting it: " + hasha.HAsset);
                    hasha.HAsset.Object = obj;
#endif
                }
                return hasha.HAsset;
            }

            return new HAsset<AssetType>(obj);
        }
#endif

        public static implicit operator HAsset<AssetType>(VobHandle<AssetType> vobHandle) => new HAsset<AssetType>(vobHandle);

        public static implicit operator VobHandle<AssetType>(HAsset<AssetType> assetHandle) => (VobHandle<AssetType>)assetHandle?.VobHandle;

        public static implicit operator AssetType(HAsset<AssetType> assetHandle) => assetHandle?.Object;

        #endregion

        #endregion

        #region Derived Properties

        public bool HasPath => !String.IsNullOrEmpty(AssetTypePath);
        public bool HasPathOrObject => !String.IsNullOrEmpty(AssetTypePath) || HasObject;

        public string FullPath => AssetPaths.AssetPathFromAssetTypePath(AssetTypePath, typeof(AssetType));


        #endregion

        #region VobHandle

#if !AOT
        public VobHandle<AssetType> GetVobHandle(bool ignoreContext = false)
        {
            var vh = AssetPath.AssetPathToHandle<AssetType>(ignoreContext: ignoreContext, concreteType: ConcreteType);
            if (!vh.HasObject && HasObject)
            {
                vh.Object = Object;
            }
            return vh;
        }
#else
		public VobHandle<AssetType> GetVobHandle(bool ignoreContext = false)
		{
        throw new Exception("REVIEW this");
			return (VobHandle<AssetType>) this.AssetTypePath.AssetNameToHandle(ignoreContext: ignoreContext, concreteType: ConcreteType, T: typeof(AssetType));
		}
#endif

#if !ASSETCACHE
        public VobHandle<AssetType> ContextualVobHandle => GetVobHandle(false);
#endif

        private const bool VobHandlePropertyIgnoreContext = true; // RECENTCHANGE - was false, but TResourceDistributor loaded from ForCom instead of Nextrek


        #endregion

        #region IReadHandle

        //object IReadHandle.Object => Object;

        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public HAsset<AssetType> EnsureLoaded
        {
            get
            {
                if (Object == null)
                {
                    var msg = "Failed to load " + typeof(AssetType) + " from " + FullPath;
                    l.Error(msg);
                    throw new AssetNotFoundException(msg);
                }
                return this;
            }
        }

        [Ignore]
        public AssetType Object
        {
            get
            {
                if (ObjectField == null)
                {
#if ASSETCACHE
                    TryEnsureLoaded();
#else
                    if (VobHandle != null)
                    {
                        objectField = VobHandle.Object;
                    }
#endif
                }
                return ObjectField;
            }
            set => objectField = value;
        }

        public bool HasObject
        {
            get
            {
                if (objectField != null)
                {
                    return true;
                }
#if !ASSETCACHE
                if (vobHandle != null)
                {
                    return vobHandle != null && vobHandle.HasObject;
                }
#endif
                return false;
            }
        }

        [Ignore]
        public AssetType ObjectField => objectField;

        // REVIEW REFACTOR - Why is there both objectField and VobHandle.Object?  Try to remove it from here?
        [Ignore]
        private AssetType objectField
        {
            get => _objectField;
            set
            {
                if (_objectField == value)
                {
                    return;
                }

                _objectField = value;

                if (ConcreteType == null && value != null)
                {
                    ConcreteType = value.GetType();
                }

#if TOPORT
                //var hasHAsset = value as IHasHRAsset;
                //if (hasHAsset != null)
                //{
                //    //#if SanityChecks  
                //    //                    if (hasHAsset.HAsset != null && !object.ReferenceEquals(this, hasHAsset.HAsset)) STACKOVERFLOW
                //    //                    {
                //    //                        l.Warn("set_objectField: hasHAsset.HAsset != null && !object.ReferenceEquals(this, hasHAsset.HAsset) - Path: " + this.AssetPath);
                //    //                    }
                //    //#endif
                //    hasHAsset.HRAsset = this;
                //}
#endif

#if !ASSETCACHE
                if (vobHandle != null)
                {
                    vobHandle.Object = value;
                }
#endif

                OnPropertyChanged("Object");
            }
        }
        private AssetType _objectField;

#endregion

                #region CRUD Methods



        public async Task<bool> TryGetObject()
        {
#if ASSETCACHE
            if(objectField != null) return true;

#if ASSETCACHEFLAT
			var fullPath =FullPath;

#if AOT
			AssetType obj;
			if (assetCacheFlat.ContainsKey(fullPath)) obj = (AssetType) assetCacheFlat[fullPath];
			else obj = null;
#else
			AssetType obj = (AssetType)assetCacheFlat.TryGetValue(fullPath);
#endif
			if(obj == null)
			{
				l.Warn("[ASSETCACHE LOAD] " + typeof(AssetType) + ":" + fullPath + " " + (obj != null).ToString());
			}
			else
			{
				this.Object = obj;

			}
#else
#error TEMP 
            Dictionary<string, object> dictionary;
            if (assetCache.TryGetValue(typeof(AssetType), out dictionary))
            {
                AssetType obj = (AssetType)dictionary.TryGetValue(AssetPath);
				this.Object = obj;
            }
            else
            {
                l.Warn("[ASSETCACHE LOAD] " + typeof(AssetType) + ":" + this.AssetPath + " false2");
            }
			if(obj == null)
			{
				l.Warn("[ASSETCACHE LOAD] " + typeof(AssetType) + ":" + AssetSubPath + " " + (obj != null).ToString());
			}

#endif
			return obj != null;
#else
            if (VobHandle == null)
            {
                return false;
            }

            return await VobHandle.TryEnsureRetrieved().ConfigureAwait(false);
#endif
        }

        public async Task Save(bool allowDelete = false)
        {
#if ASSETCACHE
#if ASSETCACHEFLAT
			if(!HasObject) throw new Exception("!HasObject - use Delete or SaveOrDelete to delete.");

			var fullPath = AssetPath.PathFromSubPath(this.AssetSubPath, typeof(AssetType));
//			l.Info("[ASSETCACHE SAVE] " + typeof(AssetType) +"  " + fullPath);

			assetCacheFlat.Set(fullPath, this.Object);
#else

            var dictionary = (IDictionary) assetCache.GetOrAddDefault(typeof(AssetType), () => new Dictionary<string, object>());
            
            //return (VobHandle<AssetType>)this.AssetPath.AssetNameToHandle(ignoreContext: ignoreContext, T: typeof(AssetType));

			dictionary.Set(this.AssetPath, this.Object);

			l.Info("[ASSETCACHE-T SAVE] " + typeof(AssetType) +":" + this.AssetPath);
            //if (dictionary.Contains(this.AssetPath)) dictionary[this.AssetPath] = this.Object;
            //else dictionary.Add(this.AssetPath, this.Object);
#endif
#else
            //l.Debug("TEMP - HAsset<>.Save()");
            //var xThis = Object;
            //HAsset.VobHandle.Object = xThis;
            var h = ContextualHandle;
            await h.Commit(allowDelete);
            //Log.Info("ZX Saved - " + this.GetType().Name + " " + h.Reference.ToString());
            //return xThis;
#endif
        }

        public void Delete()
        {
#if ASSETCACHE
            throw new NotImplementedException();
#else
            //l.Debug("TEMP - HAsset<>.Delete()");
            //            var xThis = Object;
            //HAsset.VobHandle.Object = xThis;
            var h = ContextualHandle;
            h.Delete();
#endif
        }

                #endregion

                #region Asset Cache
#if ASSETCACHE

//        private static Dictionary<Type, Dictionary<string, object>> assetCache 
//        {get{return HAsset.assetCache;}}

		private static Dictionary<string, object> assetCacheFlat {get{return HAsset.assetCacheFlat;}}

#endif
                #endregion

#if !ASSETCACHE
        public H<AssetType> ContextualHandle
        {
            get
            {
                var vh = ContextualVobHandle;
                EnsureHandleSetToThis(vh);
                return vh;
            }
        }

        public PersistenceState State => VobHandle.State;

        public bool IsPersisted => VobHandle.IsPersisted;

#endif

        private void EnsureHandleSetToThis(VobHandle<AssetType> vh)
        {
            if (!vh.HasObject)
            {
                vh.Object = Object;
            }
            else if (!object.ReferenceEquals(vh.Object, Object))
            {
                l.Info("REVIEW 2 - EnsureHandleSetToThis: AssetBase<" + typeof(AssetType).Name + ">.get_Handle: vh.HasObject && !object.ReferenceEquals(vh.Object, AssetObject): " + vh + ", Existing object: " + vh.Object + Environment.StackTrace);
                vh.Object = Object;
            }
        }

                #region Misc

        private static ILogger l = Log.Get();

        public override bool Equals(object obj)
        {
            var other = obj as HAsset<AssetType>;
            if (other == null)
            {
                return false;
            }

            if (AssetTypePath != other.AssetTypePath)
            {
                return false;
            }

            if (other.HasObject && HasObject)
            {
                if (object.ReferenceEquals(other.Object, Object)) { return true; }
                else
                {
                    // REVIEW - Same assetpath pointing to two different objects
                    return Object.Equals(other.Object);
                }
            }
            else
            {
                return true;
            }
            // OLD
            //if (AssetPath == null)
            //{
            //    if (other.AssetPath == null) return true;
            //    else return false;
            //}

            //return this.AssetPath.Equals(other.AssetPath);
        }

        public override int GetHashCode()
        {
            if (AssetTypePath == null)
            {
                return 0;
            }

            return AssetTypePath.GetHashCode();
        }

        public override string ToString()
        {
#if ASSETCACHE
            if (HasObject)
            {
                return String.Concat("<", (Reference == null ? "null" : Reference.ToString()), ">");
            }
            else
            {
                return String.Concat(">", (Reference == null ? "null" : Reference.ToString()), "<") + "-" + FullPath;
            }
#else
            return (VobHandle == null ? "(null HAsset<" + typeof(AssetType) + ">)" : VobHandle.ToString()) + (IsRegistered ? "(Registered)" : "(Non-registered)");
#endif
        }

                #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public void ForgetObject() => VobHandle.ForgetObject();
        public Task<bool> TryRetrieveObject() => VobHandle.TryRetrieveObject();
        public Task<bool> Exists(bool forceCheck = false) => VobHandle.Exists(forceCheck);

                #endregion

                #endregion

#endif

            }

#if OLD_UNUSED
    /// <summary>
    /// A read-only handle based on an asset name (which could have a subpath (UNTESTED)) and a type.
    /// Since it is read-only, the type could be an interface type.
    /// </summary>
    /// <typeparam name="AssetType"></typeparam>
    [LionSerializable(SerializeMethod.ByValue)]
    [JsonConvert(typeof(AssetNameReadHandleSerializationConverter))]
    public class HRAsset<AssetType>
        : IReadHandle<AssetType>
        , IAssetNameReadHandle
        , IReferencable
        //, IChangeableReferencable // FUTURE
    where AssetType : class
    {

    #region IReferencable

        public IReference Reference
        {
            get
            {
                return new AssetReferenceNew()
                {
                    Path = this.AssetPath,
                    Type = typeof(AssetType),
                };
            }
            //set
            //{
            //    if(value.Type 
            //    AssetReferenceNew aref = new AssetReferenceNew(value);
            //    this.AssetName = aref.Path;
            //}
        }

    #endregion

    #region Construction

        public HRAsset() { } // TODO - remove once JsonEx doesn't need this

        public HRAsset(string assetName)
        {
            this.AssetPath = assetName;
        }

        private HRAsset(string assetName, IHandle handle)
        {
            this.AssetPath = assetName;
            this.handle = handle;
        }
        //private HRAsset(IHandle handle) - See warning
        //{
        //    l.Warn("implicit operator HRAsset<AssetType>(VobHandle<AssetType>) - Does not support subpaths in asset names, cannot detect failure: " + handle);
        //    string assetName = VosPath.GetName(handle.Reference.Path);
        //    this.AssetName = assetName;
        //    this.handle = handle;
        //}

        public static implicit operator HRAsset<AssetType>(string assetName)
        {
            return new HRAsset<AssetType>(assetName);
        }

    #endregion

    #region Implicit Conversion Operators

        //- See warning
        //public static implicit operator HRAsset<AssetType>(VobHandle<AssetType> vobHandle)
        //{
        //    return new HRAsset<AssetType>(vobHandle);
        //}

        private static ILogger l = Log.Get();

        public static implicit operator AssetType(HRAsset<AssetType> assetHandle)
        {
            return assetHandle.Object;
        }

    #endregion

    #region AssetName

        public string AssetPath
        {
            get { return assetName; }
            set
            {
                if (assetName == value) return;
                if (assetName != default(string)) throw new NotSupportedException("AssetName can only be set once.");
                assetName = value;
            }
        } private string assetName;

    #endregion

    #region Handle

        public IHandle Handle
        {
            get
            {
                if (handle == null && this.AssetPath != null)
                {
                    var vobHandle = this.AssetPath.AssetNameToHandle<AssetType>();
                    if (vobHandle.Object != null)
                    {
                        handle = vobHandle;
                    }
                    else
                    {
                        // REVIEW
                        foreach (var handleOfType in vobHandle.Vob.AllHandlesOfType<AssetType>())
                        {
                            handle = handleOfType;
                            break;
                        }
                    }
                }
                return handle;
            }
            private set
            {
                if (handle == value) return;
                if (handle != null) throw new NotSupportedException("Handle can only be set once.");
                handle = value;
            }
        } private IHandle handle;

    #endregion

    #region IReadHandle

        public AssetType Object
        {
            get
            {
                if (Handle == null) return null;
                object obj = Handle.Object;
                AssetType result = obj as AssetType;
                if (obj != null && result == null)
                {
                    l.Warn("Handle.Object is not null but it is not of type AssetType.");
                }
                return result;
            }
        }

        public bool HasObject
        {
            get { if (Handle == null)return false; return Handle.HasObject; }
        }

        public AssetType ObjectField
        {
            get
            {
                if (Handle == null) return null;

                object obj = Handle.Object;
                AssetType result = obj as AssetType;
                if (obj != null && result == null)
                {
                    l.Warn("Handle.Object is not null but it is not of type AssetType.");
                }
                return result;
            }
        }

    #endregion

    }
#endif

}
