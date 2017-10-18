// ASSETCACHE flag means assets are saved/loaded in memory only, not in Vos
// TODO: Separate out more VobHandle stuff and make sure it isn't called while ASSETCACHE set
#define ASSETCACHEFLAT

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using JsonExSerializer;
using LionFire.Assets.SerializationConverters;
using LionFire.ObjectBus;
using LionFire.Serialization;
using System.Collections;

namespace LionFire.Assets
{
    public partial  class HAsset
    {
        public static HAsset<AssetType> Get<AssetType>(string assetSubPath)
            where AssetType : class
        {
            return HAsset<AssetType>.Get(assetSubPath);
            return assetSubPath.ToHAsset<AssetType>();
        }
    }

#if ASSETCACHE


    /// <summary>
    /// A read-only handle based on an asset name (which could have a subpath (UNTESTED)) and a type.
    /// Since it is read-only, the type could be an interface type (as long as the system is configured for finding instances of the interface at the correct location)
    /// 
    /// REVIEW: Asset is just a Type plus a name or path for that type.  For a given app, it is deterministic where
    /// that Asset exists as a VosReference.  Instead of having this class, have something that resolves
    /// an AssetReference into a VobHandle via a VosReference. ... on second thought, there is some tricky context
    /// stuff that depends on which packages are loaded, and if you want to save an Asset, you need to pick a package/store.
    /// </summary>
    /// <typeparam name="AssetType"></typeparam>
    [LionSerializable(SerializeMethod.ByValue)]
    [JsonConvert(typeof(HAssetSerializationConverter))]
    public partial class HAsset : IReadHandle, IHAsset, IHRAsset, INotifyPropertyChanged

    {
//		#error NEXT - implement Handle for HAssetNG?  Or should I skip it with ASSETCACHE?
        #region AssetType

        public Type AssetType
        {
            get { return assetType; }
            set
            {
                //where AssetType : class

                if (assetType == value) return;
                if (assetType != default(Type)) throw new AlreadySetException();
#if SanityChecks
                if (value.IsValueType) throw new ArgumentException("Must not be a value type");
				if (value.IsInterface || value.IsAbstract) throw new ArgumentException("Must be a concrete type");
#endif
                assetType = value;
            }
        } private Type assetType;

        #endregion


        public bool HasPathOrObject
        {
            get
            {
                return !String.IsNullOrEmpty(AssetSubPath) || HasObject;
            }
        }

        #region IReferencable

        public IReference Reference
        {
            get { return AssetReference; }
        }

        public AssetReferenceNew AssetReference
        {
            get
            {
                return new AssetReferenceNew()
                {
                    Path = this.AssetSubPath,
                    Type = AssetType,
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

        public HAsset() { } // TODO - remove once JsonEx doesn't need this?

        public HAsset(object obj, Type assetType = null)
        {
            this.objectField = obj;
            if (assetType != null)
            {
                this.AssetType = assetType;
            }
            else if (obj != null){ this.AssetType = obj.GetType();}

        }
        public HAsset(object obj, string assetPath, Type assetType = null)
            : this(obj, assetType)
        {
            this.AssetSubPath = assetPath;
        }

        public HAsset(string assetPath)
        {
            this.AssetSubPath = assetPath;
            //if (anyLocation)
            //{
            //    VobHandle
            //}
        }

        public HAsset(string assetName, bool resolveHandleIgnoringContext)
            : this(assetName)
        {
#if !ASSETCACHE
            this.VobHandle = GetVobHandle(resolveHandleIgnoringContext); 
#endif

            //l.Trace("NEW: resolved asset " + assetName + "<" + typeof(AssetType).Name + "> to " + VobHandle);
        }

#if true // should get users to provide type?
		public static explicit operator HAsset(string assetName)
		{
		    return new HAsset(assetName);
		}
#endif

        // TODO VobHandle
//        private HAsset(string assetName, VobHandle<AssetType> vobHandle)
//        {
//            this.AssetPath = assetName;
//#if !ASSETCACHE
//            this.vobHandle = vobHandle;
//#endif
//        }
        // TODO VobHandle
//        private HAsset(VobHandle<AssetType> vobHandle)
//        {
//            l.Warn("HAsset<AssetType>(VobHandle<AssetType>) - Does not support subpaths in asset names, cannot detect failure: " + vobHandle);
//            string assetName = vobHandle.Name;
//            this.AssetPath = assetName;
//#if !ASSETCACHE
//            this.vobHandle = vobHandle;
//#endif
//        }

        
        
        //public static explicit operator HAsset(object obj)
        //{
        //    // TODO VobHandle
        //    //IHasHAsset<AssetType> hasha = obj as IHasHAsset<AssetType>;
        //    //if (hasha != null)
        //    //{
        //    //    //l.Trace("Avoided creating new HAsset in implicit operator AssetType => HAsset<>");
        //    //    if (hasha.HAsset == null)
        //    //    {
        //    //        hasha.HAsset = new HAsset<AssetType>(obj);
        //    //    }
        //    //    else if (!hasha.HAsset.HasObject)
        //    //    {
        //    //        l.Warn("operator HAsset<AssetType>(AssetType obj) using obj.HAsset.  Warning: HAsset's Object was not set to obj, so setting it: " + hasha.HAsset);
        //    //        hasha.HAsset.Object = obj;
        //    //    }
        //    //    return hasha.HAsset;
        //    //}

        //    return new HAsset(obj);
        //}

        #endregion

        #region Misc

        private static ILogger l = Log.Get();

        #endregion
        
        #region Implicit Conversion Operators

        // TODO VobHandle
        //public static implicit operator HAsset(IVobHandle vobHandle)
        //{
        //    return new HAsset(vobHandle);
        //}

        // TODO VobHandle
        //public static implicit operator VobHandle<AssetType>(HAsset<AssetType> assetHandle)
        //{
        //    if (assetHandle == null) return null;
        //    //return (VobHandle<AssetType>)assetHandle.VobHandle;
        //    return assetHandle.VobHandle;
        //}

        #endregion

        #region AssetPath

        public string AssetPathName
        {
            get
            {
                if (AssetSubPath == null) return null;
                return VosPath.GetName(AssetSubPath);
            }
        }

		public string FullPath{get{return AssetPath.PathFromSubPath(this.AssetSubPath, AssetType);}}

        public string AssetSubPath
        {
            get { return assetPath; }
            set
            {
                if (assetPath == value) return;
                if (assetPath != null) throw new NotSupportedException("AssetPath can only be set once.  Use Rename instead");
                assetPath = value;

                OnAssetPathChanged();
            }
        } private string assetPath;

        protected virtual void OnAssetPathChanged()
        {
            // TODO VobHandle
            //if (this.vobHandle != null)
            //{
            //    var oldVobHandle = vobHandle;
            //    vobHandle = null;

            //    oldVobHandle.OnRenamed(VobHandle);
            //}
        }

        public void Rename(string newAssetPath)
        {
            if (newAssetPath == AssetSubPath) return;

            l.Debug("RENAME - " + this.ToString() + " --> " + newAssetPath);

            this.assetPath = null;
            AssetSubPath = newAssetPath;

            // TODO VobHandle
            //if (this.HasObject)
            //{
                
            //    if (VobHandle.Object != this.Object)
            //    {
            //        l.Warn("RENAME - Overwriting existing object at " + VobHandle);
            //    }
            //    VobHandle.Object = this.Object;
            //}
            //else
            //{
            //    if (VobHandle.HasObject)
            //    {
            //        this.Object = VobHandle.Object;
            //    }
            //}
        }

        #endregion

        #region VobHandle

#if !AOT
        public VobHandle<AssetType> GetVobHandle<AssetType>(bool ignoreContext = false)
            where AssetType : class
        {
            return this.AssetSubPath.AssetNameToHandle<AssetType>(ignoreContext: ignoreContext);
        }

#endif

        public IVobHandle GetVobHandle(bool ignoreContext = false)
        {
            if (AssetType == null) throw new ArgumentNullException("this.AssetType");
            return this.AssetSubPath.AssetNameToHandle(ignoreContext: ignoreContext, T: AssetType);
        }


#if !ASSETCACHE
        public VobHandle<AssetType> ContextualVobHandle
        {
            get
            {
                return GetVobHandle(false);
            }
        }
#endif

        private const bool VobHandlePropertyIgnoreContext = false;

        //        public VobHandle<AssetType> VobHandle
        //        {
        //            get
        //            {
        //#if !ASSETCACHE
        //                if (vobHandle == null && this.AssetPath != null)
        //                {
        //                    this.VobHandle = GetVobHandle(VobHandlePropertyIgnoreContext);

        //                    //this.VobHandle = this.AssetPath.AssetNameToHandle<AssetType>(ignoreContext: IgnoreContext);
        //                    //l.Fatal("HAsset getting VobHandle from name: " + AssetName + " -- " + this.VobHandle.TryEnsureRetrieved());
        //                } 
        //#else
        //                if (vobHandle == null)
        //                {
        //                    l.Warn("Attempt to get vobHandle when ASSETCACHE is set, and VobHandle is null.  Asset may not be retrievable.");
        //                }
        //#endif
        //                return vobHandle;
        //            }
        //            private set
        //            {
        //#if ASSETCACHE
        //                l.Warn("Ignoring HAsset.set_VobHandle");
        //                return;
        //#endif
        //                if (vobHandle == value) return;
        //                if (vobHandle != null) throw new NotSupportedException("VobHandle can only be set once.");

        //                vobHandle = value;

        //                if (this.objectField != null && vobHandle != null)
        //                {
        //                    vobHandle.Object = this.objectField;
        //                }
        //            }
        //        } private VobHandle<AssetType> vobHandle;

        #endregion

        #region IReadHandle

        object IReadHandle.Object
        {
            get
            {
                return this.Object;
            }
        }

        [Ignore]
        public object Object
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
            set
            {
                if (objectField != value)
                {
                    objectField = value;
                }
                //if (vobHandle != null)
                //{
                //    vobHandle.Object = value;
                //}
            }
        }

        public bool HasObject
        {
            get
            {
                if (objectField != null) return true;
                //if (VobHandle == null)return false; return VobHandle.HasObject; 
                return false;
            }
        }

        [Ignore]
        public object ObjectField
        {
            get
            {
                //if (objectField == null)
                //{
                //    if (VobHandle != null)
                //    {
                //        objectField = VobHandle.ObjectField;
                //    }
                //}
                return objectField;
            }
        }

        [Ignore]
        private object objectField
        {
            get
            {
                return _objectField;
            }
            set
            {
                if (_objectField == value) return;

                _objectField = value;

                OnPropertyChanged("Object");
            }
        }  private object _objectField;

        #endregion

        #region Misc

        public override bool Equals(object obj)
        {
            HAsset other = obj as HAsset;
            if (other == null) return false;

            if (this.AssetType != other.AssetType) return false;
            if (AssetSubPath != other.AssetSubPath) return false;
            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (AssetType != null) hash ^= AssetType.GetHashCode();
            if (AssetSubPath != null) hash ^= AssetSubPath.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            if (HasObject)
            {
                return String.Concat("<", (Reference == null ? "null" : Reference.ToString()), ">");
            }
            else
            {
                return String.Concat(">", (Reference == null ? "null" : Reference.ToString()), "<");
            }

            //return AssetPath == null ? "(null HAssetNG<" + AssetType.Name + ">)" : VobHandle.ToString();
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var ev = PropertyChanged;
            if (ev != null) ev(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #endregion

        public bool TryEnsureLoaded()
        {
#if ASSETCACHE
            if (AssetType == null) { throw new ArgumentNullException("this.AssetType, for path " + AssetSubPath); }
			if (objectField != null) return true;

#if !ASSETCACHEFLAT
			Dictionary<string, object> dictionary;
            if (assetCache.TryGetValue(AssetType, out dictionary))
            {
                object obj = dictionary.TryGetValue(AssetPath);

				if(obj==null)
				{
                	l.Warn("[ASSETCACHE LOAD " + (obj != null).ToString() + " ] " + AssetType.Name + ":" + this.AssetPath);
				}

                this.Object = obj;
                return obj != null;
            }
            else
            {
                l.Warn("ASSETCACHE Failed to load: " + AssetType.Name + ":" + AssetPath);
                return false;
            }
#else
			var fullPath = AssetPath.PathFromSubPath(this.AssetSubPath, AssetType);

			object obj = assetCacheFlat.TryGetValue(fullPath);

			if(obj==null)
			{
				l.Warn("[ASSETCACHE LOAD " + (obj != null).ToString() + " ] " + AssetType.Name + ":" + this.AssetSubPath);
			}

			this.Object = obj;
			return obj != null;
#endif

#else
            if (VobHandle == null) return false;
           return VobHandle.TryEnsureRetrieved();
#endif
        }

        public void Save()
        {
#if ASSETCACHE
            if (AssetType == null) { throw new ArgumentNullException("this.AssetType"); }

#if ASSETCACHEFLAT
			var fullPath = FullPath;
			assetCacheFlat.Set(fullPath, this.Object);
//			l.Info("[ASSETCACHE SAVE NG] " + AssetType.Name +"  " + fullPath);

#else


            var dictionary = (IDictionary)assetCache.GetOrAddDefault(AssetType, () => new Dictionary<string, object>());

            //return (VobHandle<AssetType>)this.AssetPath.AssetNameToHandle(ignoreContext: ignoreContext, T: typeof(AssetType));

			dictionary.Set(this.AssetPath, this.Object);
            l.Warn("[ASSETCACHE NG] Saving: " + AssetType.Name + ":" + AssetPath);

            //if (dictionary.Contains(this.AssetPath)) dictionary[this.AssetPath] = this.Object;
            //else dictionary.Add(this.AssetPath, this.Object);
#endif

#else
            //l.Debug("TEMP - HAsset<>.Save()");
            //var xThis = Object;
            //HAsset.VobHandle.Object = xThis;
            var h = this.ContextualHandle;
            h.Save();
            //Log.Info("ZX Saved - " + this.GetType().Name + " " + h.Reference.ToString());
            //return xThis;
#endif
        }

        #region Asset Cache
#if ASSETCACHE

//        internal static Dictionary<Type, Dictionary<string, object>> assetCache
//            = new Dictionary<Type, Dictionary<string, object>>();

		internal static Dictionary<string, object> assetCacheFlat
			= new Dictionary<string, object>();
#endif
        #endregion


        public void Delete()
        {
#if ASSETCACHE
            throw new NotImplementedException();
#else
            //l.Debug("TEMP - HAsset<>.Delete()");
//            var xThis = Object;
            //HAsset.VobHandle.Object = xThis;
            var h = this.ContextualHandle;
            h.Delete();
#endif
        }

#if !ASSETCACHE
        public  IHandle<AssetType> ContextualHandle
        {
            get
            {
                var vh = ContextualVobHandle;
                EnsureHandleSetToThis(vh);
                return vh;
            }
        }
#endif

        // TODO VobHandle
        //private void EnsureHandleSetToThis(VobHandle<AssetType> vh)
        //{
        //    if (!vh.HasObject)
        //    {
        //        vh.Object = Object;
        //    }
        //    else if (!object.ReferenceEquals(vh.Object, Object))
        //    {
        //        l.Warn("EnsureHandleSetToThis: AssetBase<" + AssetType.Name + ">.get_Handle: vh.HasObject && !object.ReferenceEquals(vh.Object, AssetObject): " + vh + ", Existing object: " + vh.Object + Environment.StackTrace);
        //        vh.Object = Object;
        //    }
        //}
    }
#else
//#error NotImplemented: HAssetNG for !ASSETCACHE
#endif
}
