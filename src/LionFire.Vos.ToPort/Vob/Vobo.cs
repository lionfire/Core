using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.Assets;
using LionFire.Vos;
using LionFire.Types;
using LionFire.Collections;
using LionFire.Overlays;
using System.Collections;
using LionFire.Persistence;
using LionFire.MultiTyping;
using Microsoft.Extensions.Logging;
using LionFire.Copying;

namespace LionFire.Vos
{
    
    public class LayeredMultiTypeVobo<ObjectType> : Vobo<ObjectType>, IMultiTyped
        , INotifyMultiTypeChanged
        where ObjectType : class, new()
    {

        #region Default Layer

        #region DefaultLayerPriority

        public int DefaultLayerPriority
        {
            get { return defaultLayerPriority; }
            set
            {
                if (defaultLayerPriority == value) return;
                int oldPriority = defaultLayerPriority;
                defaultLayerPriority = value;

                if (defaultLayer != null)
                {
                    DoModificationOperation(defaultLayer.SubTypeTypes, () =>
                        {
                            overlayStack.Objects.Remove(oldPriority);
                            overlayStack.Objects.Add(defaultLayerPriority, defaultLayer);
                        });
                }

            }
        } private int defaultLayerPriority = 1000;

        #endregion

        public MultiType DefaultLayer
        {
            get
            {
                return defaultLayer;
            }
            set
            {
                if (defaultLayer == value) return;
                if (defaultLayer != null)
                {
                    this.RemoveLayer(DefaultLayerPriority);
                }

                defaultLayer = value;

                if (defaultLayer != null)
                {
                    this.AddLayer(DefaultLayerPriority, defaultLayer);
                }
            }
        } private MultiType defaultLayer;

        #endregion

        #region MultiType Pass-through to Stack

        private object overlayStackLock = new object();
        private MultiTypeOverlay overlayStack
        {
            get { return _overlayStack; }
            set { _overlayStack = value; }
        }
        private MultiTypeOverlay _overlayStack = new MultiTypeOverlay();

#if !NoGenericMethods
        public T AsType<T>() where T : class { return overlayStack.AsType<T>(); }
        public IEnumerable<T> OfType<T>() where T : class { return overlayStack.OfType<T>(); }
#endif
        public object AsType(Type T) { return overlayStack.AsType(T); }
        public IEnumerable<object> OfType(Type T) { return overlayStack.OfType(T); }

        public IEnumerable<object> SubTypes { get { return overlayStack.SubTypes; } }

        public object this[Type type]
        {
            get => overlayStack[type];
            set
            {
#if true // Autocreate DefaultLayer
                if (DefaultLayer == null)
                {
                    DefaultLayer = new MultiType();
                }
                DefaultLayer[type] = value;
#else
                if (DefaultLayer != null)
                {
                    DefaultLayer[type] = value;
                }
                else
                {
                    throw new ArgumentException("Cannot use set methods when a DefaultLayer has not been set");
                }
#endif
            }
        }

        public void SetType(object obj, Type type)
        {
#if true // Autocreate DefaultLayer
            if (DefaultLayer == null)
            {
                DefaultLayer = new MultiType();
            }
            DefaultLayer.SetType(obj, type);
#else
            if (DefaultLayer != null)
            {
                DefaultLayer.SetType(obj, type);
            }
            else
            {
                throw new ArgumentException("Cannot use set methods when a DefaultLayer has not been set");
            }
#endif
        }

        public void ClearSubTypes()
        {
            overlayStack.Objects.Clear();
            OnCleared();
        }

#if !NoGenericMethods
        T IMultiTyped.AsTypeOrSetDefault<T>(Func<T> defaultValueFunc, Type slotType
            //= null
            )
        {
            throw new NotImplementedException();
        }
        T IMultiTyped.AsTypeOrSetDefault<T>(Func<IMultiTyped, T> defaultValueFunc, Type slotType
            //= null
            )
        {
            throw new NotImplementedException();
        }

        T IMultiTyped.AsTypeOrCreateDefault<T>(Type slotType
            //= null
            )
        {
            throw new NotImplementedException();
        }
#endif
        object IMultiTyped.AsTypeOrCreateDefault(Type slotType, Type type) { throw new NotImplementedException(); }
        #endregion

        #region Overlay Objects

        private object overlayObjectsLock = new object();

#if !NET35
        private Dictionary<Type, IOverlay> overlayObjects;

        public T GetOverlayObject<T>()
            where T : class, new()
        {
            l.Warn("UNTESTED, EXPERIMENTAL: GetOverlayObject");

            lock (overlayObjectsLock)
            {
                if (overlayObjects == null)
                {
                    overlayObjects = new Dictionary<Type, IOverlay>();
                }

                IOverlay overlayObj = overlayObjects.TryGetValue(typeof(T));

                if (overlayObj != null)
                {
                    return (T)overlayObj;
                }

                T overlay = OverlayFactory<T>.Create();
                IOverlay<T> overlayInterface;
                overlayInterface = (IOverlay<T>)overlay;
                overlayInterface.Disposing += OnDisposingOverlayObject;

                if (DefaultLayer != null)
                {
                    T defaultLayerObject = DefaultLayer.AsType<T>();
                    if (defaultLayerObject != null)
                    {
                        overlayInterface.Insert(defaultLayerObject, "__DefaultLayer");
                    }
                }

                overlayObjects.Add(typeof(T), overlayInterface);

                return overlay;
            }
        }
#endif

        void OnDisposingOverlayObject<T>(IOverlay<T> obj)
            where T : class, new()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Collection methods

        /// <summary>
        /// TODO REFACTOR: Use this method for all modification options
        /// Notifies 
        /// </summary>
        /// <param name="types"></param>
        /// <param name="action"></param>
        private void DoModificationOperation(IEnumerable<Type> types, Action action)
        {
            // Notifies

            lock (overlayStackLock)
            {
                Dictionary<Type, object> oldValues = new Dictionary<Type, object>();
                foreach (Type type in (IEnumerable)types)
                {
                    oldValues.Add(type, this[type]);
                }

                action();

                foreach (Type type in oldValues.Keys)
                {
                    object newValue = this[type];
                    if (newValue != oldValues[type])
                    {
                        OnChildChanged(type, newValue);
                    }
                }
            }
        }

        //public void AddLayer(int modPriority, object layer)
        //{
        //    DoModificationOperation(new Type[] { layer.GetType() }, () =>
        //    {
        //        MultiType mt = new MultiType(new object[] { layer });
        //        PROBLEM: Need corresponding remove method
        //        overlayStack.Objects.Add(modPriority, mt);
        //    });
        //}

        public void AddLayer(int modPriority, IReadOnlyMultiTyped layer)
        {
            DoModificationOperation(layer.SubTypes.Select(st => st.GetType()), () =>
            {
                overlayStack.Objects.Add(modPriority, layer);
            });

            //lock (overlayStackLock)
            //{
            //    Dictionary<Type, object> oldValues = new Dictionary<Type, object>();
            //    foreach (Type type in layer.SubTypes.Select(st => st.GetType()))
            //    {
            //        oldValues.Add(type, this[type]);
            //    }

            //    overlayStack.Objects.Add(modPriority, layer);

            //    foreach (Type type in oldValues.Keys)
            //    {
            //        object newValue = this[type];
            //        if (newValue != oldValues[type])
            //        {
            //            OnChildChanged(type, newValue);
            //        }
            //    }
            //}
        }

        public bool RemoveLayer(int key)
        {
            lock (overlayStackLock)
            {
                bool removedSomething;
                IReadOnlyMultiTyped layer =
#if AOT
					(IReadOnlyMultiTyped)
#endif
 overlayStack.Objects.TryGetValue(key);

                if (layer == null) return false;


                Dictionary<Type, object> oldValues = new Dictionary<Type, object>();
                foreach (object obj in layer.SubTypes)
                //.Select(st => st.GetType())
                {
                    Type type = obj.GetType();
                    oldValues.Add(type, this[type]);
                }

                removedSomething = overlayStack.Objects.Remove(key);

                foreach (Type type in oldValues.Keys)
                {
                    object newValue = this[type];
                    if (newValue != oldValues[type])
                    {
                        OnChildChanged(type, newValue);
                    }
                }

                return removedSomething;
            }
        }

#if !AOT
        public int RemoveLayer(IReadOnlyMultiTyped layer)
        {
            lock (overlayStackLock)
            {
                Dictionary<Type, object> oldValues = new Dictionary<Type, object>();
                foreach (Type type in layer.SubTypes.Select(st => st.GetType()))
                {
                    oldValues.Add(type, this[type]);
                }

                int removalCount = 0;
                var matches = overlayStack.Objects.Where(kvp => kvp.Value == layer);
                foreach (var key in matches.Select(x => x.Key))
                {
                    overlayStack.Objects.Remove(key);
                    removalCount++;
                }

                foreach (Type type in oldValues.Keys)
                {
                    object newValue = this[type];
                    if (newValue != oldValues[type])
                    {
                        OnChildChanged(type, newValue);
                    }
                }

                return removalCount;
            }
        }
#endif

        public void ClearLayers()
        {
            lock (overlayStackLock)
            {
                var oldSubTypes = this.SubTypes;

                overlayStack.Objects.Clear();
                OnCleared();

                OnSubTypesRemoved(oldSubTypes);
            }
        }

        protected virtual void OnCleared()
        {
        }

        #endregion

        #region INotifyMultiTypeChanged

        private void OnSubTypesRemoved(object[] oldSubTypes)
        {
#if AOT
			foreach (var x in oldSubTypes)
			{
				var type = x.GetType(); 
				// NOTE: Distinct is missing, may get multiple events per type
#else
            foreach (var type in oldSubTypes.Select(ost => ost.GetType()).Distinct())  // pre-AOT
            {
#endif
                OnChildChanged(type, null);
            }
        }

        #region Implementation

        private Dictionary<Type, Action<IReadOnlyMultiTyped, Type>> handlers = new Dictionary<Type, Action<IReadOnlyMultiTyped, Type>>();
        private object handlersLock = new object();


        public void AddTypeHandler(Type type, Action<IReadOnlyMultiTyped, Type> callback)
        //where T : class
        {
            lock (handlersLock)
            {
                // TODO FIXME REVIEW
                if (!handlers.ContainsKey(type)) handlers.Add(type, callback);
                else handlers[type] += callback;
            }
        }

        public void RemoveTypeHandler(Type type, Action<IReadOnlyMultiTyped, Type> callback)
        //public void RemoveTypeHandler<T>(Type type, MulticastDelegate callback)
        //where T : class
        {
            lock (handlersLock)
            {
                if (!handlers.ContainsKey(type)) return;

                handlers[type] -= callback;
            }
        }

        private void OnChildChanged(Type type, object newValue)
        {
            lock (handlersLock)
            {
                // TODO FIXME REVIEW
                if (!handlers.ContainsKey(type)) return;
                var ev = handlers[type];
                if (ev != null) ev.DynamicInvoke(this, type, newValue);
            }
        }

        #endregion

        #endregion

        private static ILogger l = Log.Get();

    }

    /// <summary>
    /// Provided for convenience for ObjectTypes that are to support IMultiTyped
    /// </summary>
    /// <typeparam name="ObjectType"></typeparam>
    [Asset(IsAbstract =true)]
    public class MultiTypeVobo<ObjectType> : Vobo<ObjectType>, IMultiTyped
        where ObjectType : class, new()
    {
        #region MultiType Pass-through

        public MultiType MultiType
        {
            get { return multiType; }
            set { multiType = value; } // For serialization
        } private MultiType multiType = new MultiType();

#if !NoGenericMethods
        public T AsType<T>() where T : class { return multiType.AsType<T>(); }
        public T[] OfType<T>() where T : class { return multiType.OfType<T>(); }
#endif
        public object AsType(Type T) { return multiType.AsType(T); }
        public IEnumerable<object> OfType(Type T) { return multiType.OfType(T); }
        public IEnumerable<object> SubTypes { get { return multiType.SubTypes; } }

        public object this[Type type] { get { return multiType[type]; } set { multiType[type] = value; } }
        public void SetType(object obj, Type type) { multiType.SetType(obj, type); }

        public void ClearSubTypes()
        {
            multiType.ClearSubTypes();
        }

#if !NoGenericMethods
        T IMultiTyped.AsTypeOrSetDefault<T>(Func<T> defaultValueFunc, Type slotType)
        {
            return multiType.AsTypeOrSetDefault<T>(defaultValueFunc, slotType);
        }
        T IMultiTyped.AsTypeOrSetDefault<T>(Func<IMultiTyped, T> defaultValueFunc, Type slotType)
        {
            return multiType.AsTypeOrSetDefault<T>(defaultValueFunc, slotType);
        }

        T IMultiTyped.AsTypeOrCreateDefault<T>(Type slotType)
        {
            return multiType.AsTypeOrCreateDefault<T>(slotType);
        }
#endif
        object IMultiTyped.AsTypeOrCreateDefault(Type slotType, Type type) { throw new NotImplementedException(); }

        #endregion

        #region Construction

        public MultiTypeVobo()
        {
        }

        public MultiTypeVobo(IVobHandle parent, string name)
            : base(parent, name)
        {
        }

        public MultiTypeVobo(VobHandle<ObjectType> handle) : base(handle)
        {
        }
        public MultiTypeVobo(IHasVobHandle parent, string name)
            : base(parent, name)
        {
        }

        #endregion
    }

    public abstract class Vobo<ObjectType> : Vobo<ObjectType, ObjectType>
        where ObjectType : class, new()
    {
        #region Construction

        public Vobo()
            : base()
        {
        }

        public Vobo(Vob parent, string subpath)
            : base(parent, subpath)
        {
        }

        public Vobo(IVobHandle parent, string name)
            : base(parent, name)
        {
        }

        public Vobo(VobHandle<ObjectType> handle)
            : base(handle)
        {
        }

        public Vobo(IHasVobHandle parent, string name)
            : base(parent, name)
        {
        }

        #endregion
    }

    /// <summary>
    /// A Vobo is an Object that holds its own VobHandle
    /// 
    /// (Old wording: A Vob is a node in a hierarchy with a reference to an object.  A Vobo is both in one.)
    /// 
    /// TODO REVIEW - I don't like how this works now.  How to resolve multiple addresses for the same object?  VosApp address, Package address, layer/physical address.
    /// </summary>
    /// <typeparam name="ObjectType">The type of the concrete object</typeparam>
    public abstract class Vobo<ObjectType, HandleType>
        :
#if AOT
			IHasVobHandle
#else
 IHasVobHandle<HandleType>
#endif
, ICanSetVobHandle
, ICommitable
        , IRetrievedListener
        //, IKeyed<string> // REVIEW
        where ObjectType : class, new()
        where HandleType : class // , new () // TODO - eliminate the new() requirement
    {
#if DEBUG
        static Vobo()
        {
            if (!typeof(HandleType).IsAssignableFrom(typeof(ObjectType))) throw new ArgumentException("HandleType (" + typeof(HandleType).Name + ") must be Assignable from ObjectType (" + typeof(ObjectType).Name + ")");
            // ENH: Also check that this is assignable to (same as?) ObjectType
        }
#endif

        #region Ontology

        //[Ignore] -- FUTURE:  OnLoaded(VobHandle<>) or (IReference) that is called by OBus?
        [Assignment(AssignmentMode.Ignore)]
        [Ignore]
        public VobHandle<HandleType> VobHandle
        {
            get
            {
                return vobHandle;
            }
            set
            {
                if (vobHandle != null && vobHandle != value) throw new AlreadyException("VobHandle can only be set once");
                // ENH: Allow setting again if moving/renaming?

                var oldVobHandle = vobHandle;
                vobHandle = value;

                if (vobHandle != null)
                {
                    if (vobHandle.HasObject)
                    {
                        if (!object.ReferenceEquals(vobHandle.Object, this))
                        {
                            // In general, it may be nice to reuse the same VobHandle: get the same VobHandle from a Vob, based on the type -- to avoid
                            // creating a lot of VobHandles, and to keep events together.
                            l.TraceWarn("Vobo.set_VobHandle: value already has Object set to an object other than this.  Replacing that object: " + value.Path);
                            vobHandle.Object = (HandleType)(object)this; // FORCECAST
                        }
                    }
                    else
                    {
                        vobHandle.Object = (HandleType)(object)this; // FORCECAST
                    }
                }

                var ev = VobHandleChangedForFrom;
                if(ev!=null)ev(this, oldVobHandle);

            }
        } private VobHandle<HandleType> vobHandle;

        IVobHandle ICanSetVobHandle.VobHandle
        {
            set
            {
                if (value == null) { VobHandle = null; return; }
                var typedVH = value as VobHandle<HandleType>;
                if (typedVH == null) throw new ArgumentException("Must be of type " + typeof(VobHandle<HandleType>));
                this.VobHandle = typedVH;
            }
        }

        public event Action<IHasVobHandle, IVobHandle> VobHandleChangedForFrom;

        #region Derived

        IVobHandle IHasVobHandle.VobHandle => VobHandle;

        #region Vob

        public Vob Vob => VobHandle?.Vob;

        #endregion

        #region Name

        //string IKeyed<string>.Key { get { return Name; } }

        /// <summary>
        /// Setting the name here is EXPERIMENTAL
        /// </summary>
        [Ignore]
        [Assignment(AssignmentMode.Ignore)]
        public string Name
        {
            get
            {
                if (VobHandle != null)
                {
                    return VobHandle.Name;
                }
                else return null;
            }
            set
            {
                // REVIEW - decouple from Asset subsystem
                Rename(value);
            }
        }

        public void SaveAs(string newName, bool saveNewIfExists = true, bool createIfNeeded = false) => Rename(newName, deleteOld: false, saveNewIfAlreadyHadVobHandle: true, createIfNeeded: true);
        public void Rename(string newName, bool deleteOld = true, bool saveNewIfAlreadyHadVobHandle = true, bool createIfNeeded = false)
        {
            if (VobHandle == null) // Set name for first time
            {
                var newHandle = AssetReferenceResolver.AssetNameToHandle<HandleType>(newName);
                newHandle.Value = (HandleType)(object)this; // FORCECAST

                VobHandle = newHandle;
                
                if (createIfNeeded)
                {
                    Save();
                }
            }
            else
            {
                var oldHandle = VobHandle;

                string currentPackage = VobHandle._(vh => vh.EffectivePackage);
                string currentLocation = VobHandle._(vh => vh.EffectiveStore);

                var newHandle =
#if AOT
                    (VobHandle<ObjectType>)AssetReferenceResolver.AssetNameToHandle(value, T: typeof(ObjectType));
#else
 AssetReferenceResolver.AssetNameToHandle<HandleType>(newName, package: currentPackage, location: currentLocation);
#endif
                newHandle.Value = (HandleType)(object)this; // FORCECAST

                VobHandle = newHandle;
                if (saveNewIfAlreadyHadVobHandle)
                {
                    VobHandle.Save();
                }
                if (deleteOld)
                {
                    oldHandle.Delete();
                }

                //string path = VosAssetsSettings.DefaultPathFromNameForType(value, typeof(ObjectType));
                //if (VobHandle == null)
                //{
                //    //VobHandle = new VobHandle<ObjectType>(path);
                //    VobHandle = newHandle;
                //}
                //else
                //{
                //    l.Warn("UNTESTED Moving " + VobHandle.Path + " to " + newHandle.Path);
                //    //VobHandle.Path = path;
                //VobHandle.Path = newHandle.Path; // throws?
                //}
            }
        }

        #endregion

        #endregion

        #endregion

        #region Construction

        public Vobo()
        {
            if (this.GetType() != typeof(ObjectType))
            {
                throw new ArgumentException("For Vobo<ObjectType>, ObjectType must be the name of the concrete class.");
            }
            if (!typeof(HandleType).IsAssignableFrom(typeof(ObjectType)))
            {
                throw new ArgumentException("HandleType must be assignable from ObjectType");
            }
        }

        public Vobo(Vob parent, string subpath)
            : this()
        {
            this.VobHandle = new VobHandle<HandleType>(parent, subpath);
        }

        public Vobo(IVobHandle parent, string name)
            : this()
        {
            // REVIEW - Vobo's shouldn't be linked to assets?!
            this.VobHandle = new VobHandle<HandleType>(parent, AssetPaths.GetAssetTypeFolder(typeof(ObjectType)), name);
        }
        public Vobo(VobHandle<HandleType> handle)
            : this()
        {
            this.VobHandle = handle;
        }

        public Vobo(IHasVobHandle parent, string name)
            : this(parent.VobHandle, name)
        {
        }

        #endregion

        #region Derived Methods

        #region Persistence Methods

        public void Save(bool allowDelete = false, bool preview = false)
        {
            if (VobHandle != null)
            {
                VobHandle.Save(allowDelete, preview);
            }
            else
            {
                throw new VosException("Cannot save Vobo: not associated to any Vob");
            }
        }

        #endregion

        #endregion

        #region Persistence Event Handling

        /// <summary>
        /// When a Vobo is retrieved, the VobHandle it was retrieved from should match the one reported by Vos.
        /// </summary>
        /// <param name="vh"></param>
        void IRetrievedListener.OnRetrieved(IVobHandle vh)
        {
            #region Validate matches this

            if (this.VobHandle != null)
            {
                if (this.VobHandle != vh)
                {
                    // This is expected
                    //l.Trace("IRetrievedEvent.OnRetrieved: this.VobHandle != vh.  this.VobHandle = " + this.VobHandle.ToStringSafe() + ", vh = " + vh.ToStringSafe() );
                }
                return;
            }

            #endregion

            var castedVH = vh as VobHandle<HandleType>;
            if (castedVH != null)
            {
                this.VobHandle = castedVH;
            }
            else
            {
                l.Debug("Could not reuse VobHandle because it is the wrong type: " + vh.GetType().FullName + " (needs to be: " + typeof(VobHandle<HandleType>).Name);
                this.VobHandle = new VobHandle<HandleType>(vh.Path);
            }
        }

        #endregion

        #region Misc

        private static ILogger l = Log.Get();

        #endregion
    }

    //public static class Vobo 
    //    //: IHasVobHandle
    //{

    //    //public string Name
    //    //{
    //    //    get
    //    //    {
    //    //        if (VobHandle != null)
    //    //        {
    //    //            return VobHandle.Name;
    //    //        }
    //    //        else return null;
    //    //    }
    //    //}

    //    //public Vob Vob
    //    //{
    //    //    get
    //    //    {
    //    //        if (vob == null)
    //    //        {
    //    //            if (VobHandle != null && VobHandle.VosReference != null)
    //    //            {
    //    //                vob = VobHandle.Vob;
    //    //            }
    //    //        }
    //    //        return vob;
    //    //    }
    //    //} private Vob vob;

    //    //#region IHasVobHandle Implementation

    //    //[Ignore]
    //    //public VobHandle VobHandle
    //    //{
    //    //    get
    //    //    {
    //    //        if (vobHandle == null && this.Vob != null)
    //    //        {
    //    //            vobHandle = new VobHandle(this.Vob);
    //    //            //// REVIEW TODO: Relative path!?
    //    //            //vobHandle = new VobHandle<Map>(VosReference.Path, this);
    //    //        }
    //    //        return vobHandle;
    //    //    }
    //    //} private VobHandle vobHandle;

    //    //IVobHandle IHasVobHandle.VobHandle
    //    //{
    //    //    get { return VobHandle; }
    //    //}

    //    //#endregion

    //}

    //public class MapBase : Vobo<Map>
    //, IReferencable, IHasHandle
    //{
    //AssetBase assetBase; // UNNEEDED?
    //public AssetID ID { get { return assetBase.ID; } set { assetBase.ID = value; } } // UNNEEDED?

    //public Dictionary<string, ValorEntityBase> Prototypes = new Dictionary<string, ValorEntityBase>();

    //[Ignore]
    //public IReference Reference
    //{
    //    get
    //    {
    //        return VosReference;
    //        {
    //            //Layer=map.ID.PackageName,
    //        };
    //    }
    //    //set { Handle.Reference = value; }
    //}

    //public VosReference VosReference
    //{
    //    get
    //    {
    //        if (this.ID == null || this.ID.Path == null) return null;
    //        return new VosReference(VosPaths.ActiveDataPath, this.ID.Path)
    //            {
    //                Layer = ID.PackageName,
    //            };
    //    }
    //}

    //public string Path
    //{
    //    get
    //    {
    //        var vr = VosReference; if (vr == null) return null;
    //        return vr.Path;
    //    }
    //}

    //public IReference Reference
    //{
    //    get
    //    {
    //        if (this.ID == null || this.ID.Path == null) return null;
    //        return new VosReference(this.ID.Path);
    //    }
    //}

    //[Ignore]
    //public IHandle Handle
    //{
    //    get
    //    {
    //        if (handle == null)
    //        {
    //            this.handle = this.Reference.ToHandle(this);
    //        }
    //        return handle;
    //    }
    //    set
    //    {
    //        if (handle != null) throw new AlreadyException();
    //        handle = value;
    //    }
    //} protected IHandle handle;



    //}
}
