using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using LionFire.Referencing;
using Microsoft.Extensions.Logging;
using LionFire.Vos;
using LionFire.Persistence;
using LionFire.Persistence.Handles;

namespace LionFire.Assets
{

    // NEW EXPERIMENTAL - ConcreteType can be an interface. If this works, RENAME ConcreteType to AssetType
    [Asset(IsAbstract = true)]
    public class AssetBase<ConcreteType> : AssetBase
        , IHasReadHandle
#if !AOT
            <ConcreteType>
            , IHasHAsset<ConcreteType>
            , IAsset<ConcreteType>
            //, IHasHandle
#else
			, IHasHAsset
			, IAsset
        , IReadHandle
#endif
        where ConcreteType : class
    {
        #region Ontology

        public override Type Type { get { return typeof(ConcreteType); } }

        #region Self

        //object IHasHAsset.AssetObject { get { return this.AssetObject; } }
        internal virtual ConcreteType AssetObject
        {
            get
            {
                try
                {
                    object obj = this;
                    return (ConcreteType)obj;
                }
                catch (InvalidCastException ice)
                {
                    if (this.GetType() != typeof(ConcreteType)) throw new NotSupportedException("this.GetType() (" + this.GetType() + ") must equal ConcreteType generic parameter (" + typeof(ConcreteType) + ")", ice);
                    else throw;
                }
            }
        }

        #endregion

        #endregion

        #region Construction

        public AssetBase()
        {
        }

        // Maybe for AOT:
        //		public AssetBase(HAsset hAsset) : this() 
        //		{ 
        //			hAsset.Object = AssetObject;
        //			this.HAsset = hAsset; 
        //		}

        //#if !AOT

        public AssetBase(HAsset<ConcreteType> hAsset) : this()
        {
            hAsset.Object = AssetObject;
            this.HAsset = hAsset;
        }
        //#endif

        #endregion

        #region HAsset


        #region IReadHandle pass thru, 
        // only needed for AOT, for implicit assignment to HAsset???
#if AOT
		IReference IReferencable.Reference { get { return ReadHandle==null?null:ReadHandle.Reference; } }
        object IReadHandle.Object { get { return this; } } // or ReadHandle.Object
        bool IReadHandle.HasObject { get { return true; } }
#endif
        #endregion

#if true // !AOT
        public static implicit operator HAsset<ConcreteType>(AssetBase<ConcreteType> assetBase)
        {
            return assetBase.HAsset;
        }
#else
		public static implicit operator HAsset(AssetBase<ConcreteType> assetBase)
		{
			return assetBase.HAsset;
		}
#endif
        #region Handle Accessors

        //IReadHandle IHasReadHandle.ReadHandle { get { return this.ReadHandle; } }

        public RH
#if !AOT
            <ConcreteType>
#endif
                ReadHandle
        {
            get
            {
                //				return this.Handle; // RECENTCHANGE - this was the VobHandle 
                return this.HAsset;  // New: HAsset is a IReadHandle, so why not return it?
            }
        }

#if !AOT
#if !AOT
        //H IHasHandle.Handle => this.Handle;
        public H<ConcreteType> Handle
#else
		public H Handle
#endif
        {
            get
            {
                var vh = HAsset.VobHandle;
                EnsureHandleSetToThis(vh);
                return vh;
            }
        }
#endif
#if !ASSETCACHE
        [Obsolete("Use HAsset.ContextualHandle")]
        public H<ConcreteType> ContextualHandle => HAsset.ContextualHandle;
#endif

        private void EnsureHandleSetToThis(IVobHandle<ConcreteType> vh)
        {

            if (!vh.HasObject)
            {
                vh.Object = AssetObject;
            }
            else if (!object.ReferenceEquals(vh.Object, AssetObject))
            {
                l.Info("REVIEW - EnsureHandleSetToThis: AssetBase<" + typeof(ConcreteType).Name + ">.get_Handle: vh.HasObject && !object.ReferenceEquals(vh.Object, AssetObject): " + vh.ToStringSafe() + ", Existing object: " + vh.Object.ToStringSafe() + Environment.StackTrace);
                vh.Object = AssetObject;
            }
        }

        #endregion

#if false // AOT
		IHAsset IHasHAsset.HAsset {get {return this.HAsset; } }
		[Ignore] // REVIEW
		[SerializeDefaultValue(false)]
		public HAsset HAsset
		{
			get
			{
				if (hAsset == null)
				{
					if (Name != null)
					{
						hAsset = new HAsset(this.AssetObject, Name, typeof(ConcreteType));
					}
				}
#if SanityChecks
				else 
				{
					if(!hAsset.HasObject) throw new UnreachableCodeException("!hAsset.HasObject in get_HAsset");
				}
#endif
				return hAsset;
			}
			private set
			{
				if (hAsset == value) return;
				if (hAsset != null && hAsset.Equals(value))
				{
					if (hAsset.HasObject && value.HasObject
					    && hAsset.Object != value.Object)
					{
						l.Warn("Ignoring setting of HAsset because their AssetPaths are equal, but both have an Object and they are not equal.");
					}
					return;
				}
				if (value != null && hAsset != null)
				{
					throw new NotSupportedException("HAsset can only be set once unless it is first set back to null.  Current: " + hAsset + ", New: " + value);
				}
				if (value != null)
				{
					if (value.HasObject)
					{
						if (value.ObjectField != null && value.ObjectField != this)
						{
							throw new ArgumentException("Invalid value: value.ObjectField is set to an object other than this.");
						}

						if (value.Object != this)
						{
							l.Debug("Replacing AssetBase.VobHandle with this.  FUTURE: Either avoid deserialization, or do an AssignFrom into the existing object.  AssetPath: " + value.AssetSubPath);
							value.Object = AssetObject;
							//throw new ArgumentException("Invalid value: value.Object is set to an object other than this.");
						}
						// else ok
					}
					else
					{
						//l.Debug("Setting HAsset to HAsset with no Object specified.  Setting object to this. " + Environment.StackTrace);
						value.Object = AssetObject;
					}
				}

				hAsset = value;
				OnPropertyChanged("Name");
			}
		} private HAsset hAsset;
#else
#if TOPORT
        IHAsset IHasHAsset.HAsset
        {
            get { return this.HAsset; }
            set
            {

                try
                {
                    this.HAsset = (HAsset<ConcreteType>)value;
                }
                catch (InvalidCastException ice)
                {
                    l.Warn("TODO FIXME - Invalid cast in set_HAsset - (ITemplateAsset to generic type?)");
                }

            }
        }
#endif

        protected HAsset<ConcreteType> CreateHAsset()
        {
            return new AssetIdentifier<ConcreteType>(AssetTypePath).ToHAsset(this.AssetObject);
        }


        public const bool IgnoreAssignUnregisteredHAssetToRegisteredWithPotentialLoss = true; // TODO - turn this off?

        /// <summary>
        /// Setting HAsset:
        ///  - If applicable: this.AssetObject.HAsset = value
        ///  - Exception if this.HAsset.Object is not value.Object
        /// </summary>

        [Ignore] // REVIEW
        [SerializeDefaultValue(false)]
        public HAsset<ConcreteType> HAsset
        {
            get
            {
                if (hAsset == null)
                {
                    if (AssetTypePath != null)
                    {
                        hAsset = CreateHAsset();

                        //hAsset = new HAsset<ConcreteType>(this.AssetObject, AssetTypePath);
                    }
                }
                else if (!hAsset.HasObject)
                {
                    hAsset.Object = AssetObject;
                }
                //#if SanityChecks
                // #error not a valid check? This is ok, such as when loading an object?
                //                else
                //                {
                //                    if (!hAsset.HasObject) throw new UnreachableCodeException("!hAsset.HasObject in get_HAsset");
                //                }
                //#endif
                return hAsset;
            }
            set
            {
                if (object.ReferenceEquals(hAsset, value)) { return; }

                // If AssetObject is IHasHAsset, make sure its HAsset matches the value being assigned here.
                var hha = AssetObject as IHasHAsset<ConcreteType>;
                if (hha != null && !object.ReferenceEquals(this, hha) && (hha.HAsset != null && !hha.HAsset.Equals(value)))
                {
                    if (hha.HAsset != value) l.Warn("set_HAsset: hha.HAsset != value.  Attempting overwrite. " + value);
                    hha.HAsset = value;
                }

                if (value != null && hAsset != null)
                {
                    if (IgnoreAssignUnregisteredHAssetToRegisteredWithPotentialLoss && value.AssetPath == hAsset.AssetPath)
                    {
                        l.Warn("Ignoring reassignment to " + value.AssetPath + " with different HAsset instances.");
                        return;
                    }
                    else
                    {
                        throw new NotSupportedException("HAsset can only be set once unless it is first set back to null.  Current: " + hAsset.ToStringSafe() + ", New: " + value.ToStringSafe());
                    }
                }

#if SanityChecks
                if (value == null)
                {
                    l.Warn("set_HAsset(null) " + this);
                }
#endif

                if (hAsset != null && hAsset.HasObject && value != null && value.HasObject)
                {
                    if (!object.ReferenceEquals(hAsset.Object, value.Object)
                    //hAsset.Object != value.Object
                    )
                    {
                        var msg = this + " - set_HAsset: Ignoring setting of HAsset because their AssetPaths are equal, but both have an Object and they are not the same object reference.";
                        l.Warn(msg);
                        throw new Exception(msg);
                    }
                    else
                    {
                        l.TraceWarn("hAsset != null && hAsset.Equals(value) but objects match.");
                    }
                    return;
                }

                if (value != null)
                {
                    if (value.HasObject)
                    {
                        if (value.ObjectField != null && value.ObjectField != this)
                        {
                            // REVIEW FIXME - This happens with TheatreEngine (MultiAssetBase) because ContextualVobHandle.Object gets assigned to this.Objet or something, or one object gets loaded from disk, maybe
                            var msg = "AssetBase.set_HAsset: value.ObjectField is set to an object other than this. " + this;
                            l.Warn(msg);
                            //throw new ArgumentException(msg);
                        }

                        if (value.Object != this)
                        {
                            l.Debug("Replacing AssetBase.VobHandle with this.  FUTURE: Either avoid deserialization, or do an AssignFrom into the existing object.  AssetPath: " + value.AssetTypePath);
                            value.Object = AssetObject;
                            //throw new ArgumentException("Invalid value: value.Object is set to an object other than this.");
                        }
                        // else ok
                    }
                    else
                    {
                        //l.Debug("Setting HAsset to HAsset with no Object specified.  Setting object to this. " + Environment.StackTrace);
                        value.Object = AssetObject;
                    }
                }

                hAsset = value;
                OnPropertyChanged("Name");
                //if (value == null)
                //{
                //    base.ID = null;
                //}
            }
        }
        private HAsset<ConcreteType> hAsset;
#endif
#endregion

        #region Path

        public string Name { get { return LionPath.GetName(AssetTypePath); } set { this.AssetTypePath = value; } } // REVIEW

        public virtual bool AllowRename { get { return false; } }

        //[Ignore]
        //public override string AssetTypePath {
        //    get {
        //        return HAsset == null ? null : HAsset.AssetTypePath;
        //    }
        //    set {
        //        if (HAsset != null) HAsset.AssetTypePath = value;
        //        else HAsset = value;
        //    }
        //}


        [Ignore]
        public override string AssetTypePath
        {
            get
            {
                if (hAsset != null)
                {
                    return hAsset.AssetTypePath;
                }
                return base.AssetTypePath;
            }
            set
            {
                string oldName = AssetTypePath;

                //l.Warn("AssetBase<>.set_Name: " + Environment.StackTrace);
                if (hAsset != null && hAsset.AssetTypePath == value) { return; }

                if (value == null)
                {
                    HAsset = null;
                }
                else
                {
                    bool isRename = hAsset != null && hAsset.AssetTypePath != value;

                    if (isRename && !AllowRename)
                    {
                        throw new AlreadyException("HAsset is already set and cannot be changed unless HAsset is first set to null.");
                    }


                    if (hAsset != null)
                    {
                        hAsset.Rename(value);
                    }
                    else
                    {
                        //hAsset = value; // assetPath
                        //hAsset.Object = AssetObject;

                        hAsset = value.ToHAsset<ConcreteType>(AssetObject);

                    }
                }

                if (AssetTypePath != oldName)
                {
                    OnPropertyChanged("Name");
                    OnNameChanged();
                }

                //base.Name = value; // depend on this when HAsset not available (because AssetNamePath is not available yet
            }
        }

        public event Action NameChanged;

        protected virtual void OnNameChanged()
        {
            if (hAsset != null)
            {
                if (AssetTypePath != hAsset.AssetTypePath)
                {
                    hAsset = null;
                }
            }
            var ev = NameChanged; if (ev != null) ev();
            OnPropertyChanged("Name");
            OnPropertyChanged("DisplayName");
        }

        #endregion

        #region Methods

        // TEMP? - remove after old IAsset is gone?  IHasHandle extensions should pick up the save
        //[Obsolete("Use HAsset.Save")]
        //public ConcreteType Save()
        ////public void Save()
        //{
        //    this.HAsset.Save();
        //    return this.AssetObject;
        //}

        [Obsolete("Use HAsset.Delete")]
        public void Delete()
        {
            this.HAsset.Delete();
        }

        #endregion

        #region Misc

        [Ignore]
        public virtual string DisplayName
        {
            get { return AssetTypePath; }
            set
            {
                throw new NotSupportedException("Does not support setting DisplayName.  Set Name instead.");
            }
        }

        //public override bool Equals(object obj)
        //{
        //AssetBase<Concrete    
        //}

        public override object Clone()
        {
            object obj = base.Clone();
            AssetBase<ConcreteType> castedObj = (AssetBase<ConcreteType>)obj;
            castedObj.HAsset = null;

            return castedObj;
        }

        public override string ToString()
        {
            return this.GetType().Name + ":" + (AssetTypePath == null ? (this.GetType().Name + " (null AssetPath)") : AssetTypePath) + "";
        }

        private static ILogger l = Log.Get();

        #endregion

    }



}
