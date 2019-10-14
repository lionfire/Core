#if false // MOVED to Referencing -- REVIEW the changes I made a while ago if any
//#define DEBUG_LOAD
//#define TRACE_LOAD_FAIL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;
using Microsoft.Extensions.Logging;

namespace LionFire.ObjectBus
{
    /// Minimalistic Handle Base
    //   - Abstract Reference
    public abstract class HandleBase2<ObjectType>
        : IHandle
#if !AOT
<ObjectType>
#endif
        // IHasHandle?
        where ObjectType : class//, new()
    {
#region Construction

        public HandleBase2(ObjectType obj = null, bool freezeObjectIfProvided

#if AOT
		                   = false
#else
 = true
#endif
)
        {

            this._object = obj;
            if (obj != null && freezeObjectIfProvided)
            {
                IsObjectReferenceFrozen = true;
            }

            if (typeof(ObjectType) == typeof(object)) { l.Trace("HandleBase2<object>: " + this.ToString()); }
        }

        internal HandleBase2(string uri, ObjectType obj = null, bool freezeObjectIfProvided = true)
            : this(obj)
        {
            this.Reference = uri.ToReference();
            this._object = obj;

            if (typeof(ObjectType) == typeof(object)) { l.Trace("HandleBase2<object>: " + this.ToString()); }
        }

        public HandleBase2(IReference reference, ObjectType obj = null, bool freezeObjectIfProvided = true)
            : this(obj, freezeObjectIfProvided)
        {
            this.Reference = reference;


            //if (LoadOnDemandByDefault)
            //{
            //    RetrieOnDemandByDefault = true;
            //}
            if (typeof(ObjectType) == typeof(object)) { l.Trace("HandleBase2<object>: " + this.ToString()); }
        }

        public HandleBase2(IReferencable referencable, ObjectType obj = null, bool freezeObjectIfProvided = true)
            : this(obj, freezeObjectIfProvided)
        {
            IReference reference = referencable.Reference;

            if (!reference.IsValid())
            {
                throw new ArgumentException("referencable.Reference must be valid");
            }

            this.Reference = referencable.Reference;
            if (typeof(ObjectType) == typeof(object)) { l.Trace("HandleBase2<object>: " + this.ToString()); }
        }

#endregion

#region Reference

        public abstract IReference Reference {
            get;
            set;
        }

        //#if AOT
        object IROKeyed.Key { get { return Key; } }
        //#endif
        public string Key {
            get {
                return Reference.Key;
            }
        }

        protected void OnReferenceChangedFrom(IReference oldReference)
        {
            var ev = ReferenceChangedForFrom;
            if (ev != null) ev(this, oldReference);
        }

        public event Action<IChangeableReferencable, IReference> ReferenceChangedForFrom;

#endregion

#region INotifyPropertyChanged

#region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler ObjectPropertyChanged {
            add {
                if (propertyChanged == null)
                {
                    if (this._object != null)
                    {
                        INotifyPropertyChanged inpc = _object as INotifyPropertyChanged;
                        if (inpc != null)
                        {
                            inpc.PropertyChanged += inpc_PropertyChanged;
                        }
                    }
                }
                propertyChanged += value;
            }
            remove {
                propertyChanged -= value;
                if (propertyChanged == null)
                {
                    if (this._object != null)
                    {
                        INotifyPropertyChanged inpc = _object as INotifyPropertyChanged;
                        if (inpc != null)
                        {
                            inpc.PropertyChanged += inpc_PropertyChanged;
                        }
                    }
                }
            }
        }
        event PropertyChangedEventHandler propertyChanged;

        //void inpc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{

        //}


        private void OnPropertyChanged(string propertyName)
        {
            var ev = propertyChanged;
            if (ev != null) ev(this, new PropertyChangedEventArgs(propertyName));
        }

#endregion

#endregion


#region Object

#region IsObjectFrozen

        [Ignore]
        public bool IsObjectReferenceFrozen {
            get { return isObjectReferenceFrozen; }
            set {
                if (isObjectReferenceFrozen == value) return;
                if (value == false && isObjectReferenceFrozen)
                {
                    throw new NotSupportedException("IsObjectReferenceFrozen cannot be set to false after setting to true.");
                }
#if AOT
				if(value)
				{
					throw new NotSupportedException("IsObjectReferenceFrozen on AOT build");
				}
#endif
                isObjectReferenceFrozen = value;
            }
        }
        private bool isObjectReferenceFrozen;

#endregion

        //protected bool loadObjectOnDemand { get { return SpecialObject.LoadOnDemand.Equals(_object); } }

        //public bool? LoadObjectOnDemand
        //{
        //    get
        //    {
        //        if (SpecialObject.LoadOnDemand.Equals(_object)) return true;
        //        else if (_object == null) return false;
        //        else return null;
        //    }
        //    set
        //    {
        //        if (value == true)
        //        {
        //            SetObjectToNull();
        //            if (_object == null)
        //            {
        //                _object = SpecialObject.LoadOnDemand;
        //            }
        //            // else ?
        //        }
        //        else if (value == false)
        //        {
        //            if (SpecialObject.LoadOnDemand.Equals(_object))
        //            {
        //                _object = null;
        //            }
        //            // else ?
        //        }
        //        // else ?
        //    }
        //}

        //public static bool LoadOnDemandByDefault = true;

        object IReadHandle.Object {
            get {
                //l.Trace("ZX HandleBase2.get_Object, iReadHandle");
                return this.Object;
            }
        }


        public virtual void ForgetObject()
        {
            forgotObject = true;
        }


#if AOT && true // AOTTEMP
#else
        object IHandle.Object {
            get {
                //l.Trace("ZX HandleBase2.get_Object, IHandle");

                return this.Object;
            }
            set {
                this.Object = (ObjectType)value;
            }
        }
#endif

        public ObjectType ObjectOrThrow {
            get {
                var result = Object;
                if (result == null)
                {

                    throw new VosException($"Failed to load object of type {typeof(ObjectType).Name} at ${this.Reference.Path}");
                }
                return result;
            }
        }
        [Ignore]
        public ObjectType Object {
            get {
                //System.Threading.Thread.MemoryBarrier();
                //				l.Info("ZX HandleBase2.get_Object");
                //System.Threading.Thread.MemoryBarrier();

                if (_object == null
                    //&& RetrieveOnDemand
                    )
                {
                    TryRetrieve();
                }
                else if (forgotObject)
                {
                    //forgotObject = false;
                    TryRetrieve(setToNullOnFail: true);
                }

#if AOT
//				l.Info("ZX HandleBase2.get_Object got " + (_object== null ? "null" : _object.GetType().Name));

				return (ObjectType)_object;
#else
                return _object;
#endif
            }
            set {
                forgotObject = false;
                if (System.Object.ReferenceEquals(_object, value)) return;

                if (IsObjectReferenceFrozen)
                {
                    AssignFrom(value);

                    //throw new InvalidOperationException("Object is frozen.");
                }
                else
                {
                    //if (value == null) 
                    //{
                    //    if(SpecialObject.LoadOnDemand.Equals(_object))
                    //    {
                    //        return;
                    //    }

                    //    SetObjectToNull();
                    //    return;
                    //}

                    if (_object != null && EffectiveAutoSave)
                    {
                        AutoSaveManager.Instance.Unregister(this);
                    }
                    var wasPropertyChangedEventsAttached = IsPropertyChangedEventsAttached || IsObjectChangedEnabled;

                    IsPropertyChangedEventsAttached = false;

                    _object = value;

                    if (_object != null && wasPropertyChangedEventsAttached)
                    {
                        IsPropertyChangedEventsAttached = wasPropertyChangedEventsAttached;
                    }
                    if (_object != null && EffectiveAutoSave)
                    {
                        AutoSaveManager.Instance.Register(this);
                    }
                    OnObjectChanged();
                }
            }
        }
#if AOT
		protected object _object;
#else
        protected ObjectType _object;
#endif

        /// <summary>
        /// Subsequent call to get_Object will re-retrieve the Object.
        /// We don't forget the _object by setting it to null in case 
        /// the reference is pinned.
        /// </summary>
        protected bool forgotObject;


        public ObjectType ObjectOrCreate {
            get {
                if (Object == null)
                {
                    Object = CreateDefault();
                }
                return Object;
            }
        }

#region ObjectField

        public ObjectType ObjectField {
            get {
#if AOT
				return (ObjectType)_object;
#else
                return _object;
#endif
            }
            set { _object = value; }
        }

#endregion

        private void RegisterForAutoSave(object obj)
        {

        }

        public bool IsObjectChangedEnabled { get { return objectChanged != null; } }
        public event ObjectChanged ObjectChanged {
            add {
                objectChanged += value;
                IsPropertyChangedEventsAttached = true;
            }
            remove {
                objectChanged -= value;
                IsPropertyChangedEventsAttached = objectChanged == null;
            }
        }
        private event ObjectChanged objectChanged;

#region Propety Changed Events

#region IsPropertyChangedEventsAttached

        public bool IsPropertyChangedEventsAttached {
            get { return isPropertyChangedEventsAttached; }
            set {
                if (isPropertyChangedEventsAttached == value) return;

                isPropertyChangedEventsAttached = value;
                if (_object == null)
                {
                    //l.Warn("_object == null during change to IsPropertyChangedEventsAttached.  This should only be set when _object is set.");
                    return;
                }
                //l.LogCritical("(TEMP) IsPropertyChangedEventsAttached: " + value + " " + this.ToString());

                if (isPropertyChangedEventsAttached)
                {
                    INotifyPropertyChanged inpc = _object as INotifyPropertyChanged;
                    if (inpc != null)
                    {
                        inpc.PropertyChanged += inpc_PropertyChanged;
                    }
                    IPropertyChanged ipc = _object as IPropertyChanged;
                    if (ipc != null)
                    {
                        ipc.PropertyValueChanged += ipc_PropertyValueChanged;
                    }
                    INotifyCollectionChanged inc = _object as INotifyCollectionChanged;
                    if (inc != null)
                    {
                        inc.CollectionChanged += inc_CollectionChanged;
                    }
                }
                else
                {
                    INotifyPropertyChanged inpc = _object as INotifyPropertyChanged;
                    if (inpc != null)
                    {
                        inpc.PropertyChanged -= inpc_PropertyChanged;
                    }
                    IPropertyChanged ipc = _object as IPropertyChanged;
                    if (ipc != null)
                    {
                        ipc.PropertyValueChanged -= ipc_PropertyValueChanged;
                    }
                    INotifyCollectionChanged inc = _object as INotifyCollectionChanged;
                    if (inc != null)
                    {
                        inc.CollectionChanged -= inc_CollectionChanged;
                    }
                }

            }
        }
        private bool isPropertyChangedEventsAttached;

#endregion

        void inc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var ev = objectChanged;
            if (ev != null) ev(this, "(CollectionChanged)");
        }

        void ipc_PropertyValueChanged(string propertyName)
        {
            var ev = objectChanged;
            if (ev != null) ev(this, propertyName);
        }

        void inpc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var ev = objectChanged;
            if (ev != null) ev(this, e.PropertyName);
        }

#endregion


        protected IMultiTyped ObjectMT { get { return _object as IMultiTyped; } }

        protected IExtendableMultiTyped ObjectEMT { get { return _object as IExtendableMultiTyped; } }

        //private void set_Object_Internal(T value)
        //{
        //    if (System.Object.ReferenceEquals(_object, value)) return;

        //    _object = value;
        //    OnObjectChanged();
        //}

        public bool HasObject {
            get {
                return _object != null;
                //&& !loadObjectOnDemand;
            }
        }


#endregion

#region Persistence

#region Create / Update

        private const bool saveCreatedObject = true; // HACK Hackish to save?  Or does "Create" include saving?
        public void RetrieveOrCreate()
        {
            if (!TryEnsureRetrieved())
            {
                ConstructDefault();
                if (saveCreatedObject) { Save(); }
            }
        }

        bool IHandlePersistence.RetrieveOrCreateDefault(object defaultValue)
        {
            return RetrieveOrCreateDefault((ObjectType)defaultValue);
        }
        bool IHandlePersistence.RetrieveOrCreateDefault(Func<object> defaultValue)
        {
            return RetrieveOrCreateDefault_Object(defaultValue);
        }

        public bool RetrieveOrCreateDefault(ObjectType defaultValue)
        {
            if (!TryEnsureRetrieved())
            {
                Object = defaultValue;
                return false;
            }
            return true;
        }

        public bool RetrieveOrCreateDefault(Func<ObjectType> defaultValue)
        {
            if (!TryEnsureRetrieved())
            {
                Object = defaultValue();
                return false;
            }
            return true;
        }

        public bool RetrieveOrCreateDefault_Object(Func<object> defaultValue)
        {
            if (!TryEnsureRetrieved())
            {
                Object = (ObjectType)defaultValue();
                return false;
            }
            return true;
        }

#endregion

#region Create

        public void Create()
        {
            EnsureConstructed();
            OBus.Create(this.Reference, this.Object); // Throws if already exists

            //OBus.CreateOrOverwrite(this.Reference, this.Object);
        }

        public void CreateOrOverwrite()
        {
            EnsureConstructed();
            Save();
        }

#endregion

#region Construction (No persistence)

        private ObjectType CreateDefault(bool applyDefaultValues = true)
        {
            ObjectType result;
            if (typeof(ObjectType) == typeof(object))
            {
                if (Reference == null) throw new ArgumentException("Reference.Type must be set when using non-generic Handle, or when the generic type is object.");

                if (Reference.Type == null)
                {
                    throw new ArgumentException("Reference.Type must be set when using non-generic Handle, or when the generic type is object.");
                }
                result = (ObjectType)Activator.CreateInstance(Reference.Type);
            }
            else
            {
                result = (ObjectType)Activator.CreateInstance(typeof(ObjectType));
            }
            if (applyDefaultValues) { DefaultValueUtils.ApplyDefaultValues(result); }
            return result;
        }
        public void ConstructDefault(bool applyDefaultValues = true)
        {
            Object = CreateDefault(applyDefaultValues);
        }
        private void ConstructDefaultNoEvents(bool applyDefaultValues = true)
        {
            _object = CreateDefault(applyDefaultValues);
        }

        public void EnsureConstructed() // REVIEW: What should be done here?
        {
            //RetrieveOrCreateDefault(); ??

            if (Object == null)
            {
                ConstructDefault();
            }
        }
        private void EnsureConstructedNoEvents() // REVIEW: What should be done here?
        {
            if (_object == null)
            {
                ConstructDefaultNoEvents();
            }
        }

#endregion

#region Delete

        public virtual bool? CanDelete()
        {
            bool? result = OBus.CanDelete(this.Reference);
            return result;
        }
        public virtual bool TryDelete(bool preview = false)
        {
            bool result = OBus.TryDelete(this.Reference, preview);
            if (result && !preview)
            {
                OnDeleted();
            }
            return result;
        }

        public virtual void Delete()
        {
            OBus.Delete(this.Reference); // Throws if doesn't exist

            // OLD:
            //Object = null;
            //Object = SpecialObject.NullObject;

            OnDeleted();
        }

#endregion

#region Move / Copy

        public void Copy(IReference newReference)
        {
            throw new NotImplementedException();
        }
        public void Move(IReference newReference)
        {
            throw new NotImplementedException();
        }

#endregion

#region AssignFrom

        void IHandlePersistence.AssignFrom(object other)
        {
            AssignFrom((ObjectType)other);
        }

#region (Static) ObjectTypeAssignmentMode

        const AssignmentMode DefaultAssignmentMode = AssignmentMode.DeepCopy;
        internal static AssignmentMode ObjectTypeAssignmentMode // MOVE?
        {
            get {
                if (assignmentMode == LionFire.AssignmentMode.Unspecified)
                {
                    var attr = typeof(ObjectType).GetCustomAttribute<AssignmentAttribute>();
                    if (attr != null)
                    {
                        assignmentMode = attr.AssignmentMode;
                    }
                    else
                    {
                        assignmentMode = DefaultAssignmentMode;
                    }
                }
                return assignmentMode;
            }
        }
        static AssignmentMode assignmentMode = AssignmentMode.Unspecified;

#endregion

        public void AssignFrom(ObjectType other, AssignmentMode assignmentMode = AssignmentMode.Unspecified) // Move to generic extension method?
        {
#if AOT
			throw new NotSupportedException("AssignFrom not supported in AOT");
#else

            if (other == null) return; // Empty out this?

            if (assignmentMode == AssignmentMode.Unspecified)
            {
                assignmentMode = ObjectTypeAssignmentMode;
            }

            EnsureConstructedNoEvents();

            // FUTURE: AssignIgnore attribute
            //#if AOT
            //            LionFire.Extensions.AssignFrom.AssignFromUtils.AssignFrom(this._object, other);
            //#else
            LionFire.Extensions.AssignFrom.AssignFromUtils.AssignFrom<ObjectType>(this._object, other, assignmentMode);
            //#endif
#endif
        }
#endregion

#region Retrieve

        //public bool RetrieveOnDemand { get; set; }

        public void Retrieve()
        {
            //if (this.Reference == null) throw new ArgumentException("this.Reference is null");

            if (!TryRetrieve()) throw new ObjectNotFoundException();

            //    ObjectType result;
            //    if (typeof(ObjectType) == typeof(object))
            //    {
            //        result = (ObjectType)OBus.Get(this.Reference);
            //    }
            //    else
            //    {
            //        result = OBus.GetAs<ObjectType>(this.Reference);
            //    }
            //    OnRetrieved(result);
        }

        public virtual ObjectType TryGetOrCreate()
        {
            if (!HasObject)
            {
                TryRetrieve();
                if (!HasObject) { ConstructDefault(); }
            }
            return Object;
        }

        //public virtual bool TryReload(bool setToNullOnFail = true)
        //{
        //}

        public virtual bool QueryExistance()
        {
            return Object != null;
        }

        public abstract RetrieveInfo RetrieveInfo { get; set; }
        public virtual bool IsRetrieveInfoEnabled {
            get { return RetrieveInfo != null; }
            set {
                if (value) { if (RetrieveInfo == null) { RetrieveInfo = new RetrieveInfo(); } }
                else
                {
                    RetrieveInfo = null;
                }
            }
        }

        public virtual bool TryRetrieve(bool setToNullOnFail = true)
        {
            //			Log.Info("ZX HandleBase2.TryRetrieve");

            if (!setToNullOnFail) throw new NotImplementedException("setToNullOnFail = false");

            ObjectType result;
            //IHandle resolvedTo = null;
#if !AOT
            if (typeof(ObjectType) != typeof(object))
            {
                result = OBus.TryGetAs<ObjectType>(this.Reference, RetrieveInfo);
            }
            else
#endif
            {
                result = (ObjectType)OBus.TryGet(this.Reference, RetrieveInfo);
            }

            //T obj = OBus.TryGetAs<T>(this.Reference);

            if (result == null)
            {
#if TRACE_LOAD_FAIL
                lFailLoad.Trace("Failed to retrieve " + this.Reference);
#endif
            }
            else
            {
#if DEBUG_LOAD
                lLoad.Debug("HandleBase2 Retrieved " + this.Reference);
#endif
            }

            OnRetrieved(result);

            return result != null;

        }


        public void EnsureRetrieved()
        {
            bool result = TryEnsureRetrieved();
            if (!result) throw new ObjectNotFoundException();
        }

        /// <summary>
        /// If Object is null, attempts to retrieve.  Returns true if Object != null
        /// </summary>
        /// <returns></returns>
        public bool TryEnsureRetrieved()
        {
            if (_object != null) return true;

            if (Reference != null)
            {
                TryRetrieve();
            }

            return _object != null;
        }

#endregion

#region Save

#region AutoSave

        public bool? AutoSave {
            get { return autoSave; }
            set {
                if (autoSave == value) return;
                var oldEffectiveAutoSave = EffectiveAutoSave;
                autoSave = value;
                //l.Info("TEMP - AutoSave = " + value + " for " + this);
                if (oldEffectiveAutoSave != EffectiveAutoSave)
                {
                    if (EffectiveAutoSave)
                    {
                        AutoSaveManager.Instance.Register(this);
                    }
                    else
                    {
                        AutoSaveManager.Instance.Unregister(this);
                    }
                }
                OnPropertyChanged("AutoSave");
            }
        }
        private bool? autoSave;

        public bool EffectiveAutoSave {
            get {
                if (AutoSave.HasValue) return AutoSave.Value;
                return InheritedAutoSave;
            }
        }

        protected bool InheritedAutoSave {
            get {
                if (AutoSaveManager.AutoSaveTypes.Contains(typeof(ObjectType))) return true;
                if (_object != null && AutoSaveManager.AutoSaveTypes.Contains(_object.GetType())) return true;
                return false;
            }
        } // FUTURE: Actually inherit



#endregion

        private bool AllowOverwriteOnSave = true;

        public event Action<IHandle> Saving;
        public virtual void Save(bool allowDelete = false, bool preview = false)
        {
            var ev = Saving; if (ev != null) ev(this);
            if (!HasObject)
            {
                if (allowDelete)
                {
                    TryDelete(preview);
                }
                else
                {
                    throw new ArgumentException("Attempt to save null when allowDelete == false");
                }
                return;
            }

            if (AllowOverwriteOnSave)
            {
                OBus.Set(this.Reference, this._object);
            }
            else
            {
                if (IsPersisted)
                {
                    OBus.Set(this.Reference, this._object); // TODO: Use update instead to make sure it wasn't deleted out from under us

                    // FUTURE: Some kind of threadsafe versioning logic to make sure the object wasn't updated from under us.
                }
                else
                {
                    OBus.Create(this.Reference, this._object); // Throws if already exists
                }
            }

            OnSaved(); // Sets IsPersisted = true
        }

#endregion

#region Children Names

        public virtual IEnumerable<string> GetChildrenNames(bool includeHidden = false)
        {
            return OBus.GetChildrenNames(this.Reference).Where(n => includeHidden || !VosPath.IsHidden(n));
        }

        public virtual IEnumerable<string> GetChildrenNamesOfType<ChildType>() where ChildType : class, new()
        {
            return OBus.GetChildrenNamesOfType<ChildType>(this.Reference);
        }
        public virtual IEnumerable<string> GetChildrenNamesOfType(Type childType)
        {
            return OBus.GetChildrenNamesOfType(childType, this.Reference);
        }

        public virtual IEnumerable<IHandle> GetChildren()
        {
            return OBus.GetChildren(this.Reference);
        }

#if !AOT
        public virtual IEnumerable<IHandle<ChildType>> GetChildrenOfType<ChildType>() where ChildType : class//,new()
        {
            return OBus.GetChildrenOfType<ChildType>(this.Reference);
        }
#endif
        public virtual IEnumerable<IHandle> GetChildrenOfType(Type childType)
        {
            return OBus.GetChildrenOfType(this.Reference, childType);
        }

#endregion

#endregion

#region Change Events

        protected virtual void OnObjectChanged()
        {
            if (_object != null)
            {
                IReferencable referenceable = _object as IReferencable;
                if (referenceable != null)
                {
                    if (Reference == null)
                    {
                        this.Reference = referenceable.Reference;
                    }
                    else
                    {
                        if (!Reference.Equals(referenceable.Reference))
                        {
                            l.Warn("HandleBase Object changed to an object with refernce: " + referenceable.Reference + " but this.Reference is: " + this.Reference + " (MountReference: " + MountReference + ")");
                        }
                    }
                }
            }
        }

        protected virtual IReference MountReference {
            get { return null; }
        }

#endregion

#region Persistence Events

#region IsPersisted

        public bool IsPersisted {
            get { return isPersisted; }
            set {
                if (isPersisted == value) return;
                isPersisted = value;

                //var ev = IsPersistedChanged;
                //if (ev != null) ev();
            }
        }
        private bool isPersisted;

        //public event Action IsPersistedChanged;

#endregion

        protected void OnDeleted()
        {
            IsPersisted = false;
        }

        //public IEnumerable<IHandle> ResolvedTo { get { return ResolvedTo; } }
        //private SortedSet<IHandle> resolvedTo;

        //private virtual void OnResolvedTo(IHandle handle)
        //{
        //    if (resolvedTo == null) resolvedTo = new SortedSet<IHandle>();
        //    if (!resolvedTo.Contains(handle))
        //    {
        //        resolvedTo.Add(handle);
        //    }
        //}

        protected virtual void OnRetrieved(ObjectType retrievedObject)
        {
            //Log.Info("ZX HandleBase2.OnRetrieved");

            if (retrievedObject == null)
            {
                IsPersisted = false;
                return;
            }

            IHasHandleSetter hasHandle = retrievedObject as IHasHandleSetter;
            if (hasHandle != null)
            {
                hasHandle.Handle = this;
            }

            IsPersisted = true;

            if (IsObjectReferenceFrozen)
            {
#if AOT
				throw new NotSupportedException("IsObjectReferenceFrozen on AOT build");
#else
                AssignFrom(retrievedObject);
#endif
            }
            else
            {
                this.Object = retrievedObject;
            }
        }

        protected void OnSaved()
        {
            IsPersisted = true;
        }

#endregion

#region IHasHandle

        [Ignore]
        public IHandle Handle {
            get { return this; }
            set { throw new NotSupportedException("this is a handle.  Cannot set this.Handle"); }
        }

#endregion

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
        }

#region IMultiTyped

        public object this[Type type] {
            get {
                if (!TryEnsureRetrieved()) return null;

                if (type.IsAssignableFrom(_object.GetType()))
                {
                    return _object;
                }

                IMultiTyped mtObj = _object as IMultiTyped;

                if (mtObj != null)
                {
                    object mtChild = mtObj[type];
                    if (mtChild != null)
                    {
                        return mtChild;
                    }
                }
                return null;
            }
        }

        public bool IsPersistable<T>()
        {
            var attr = typeof(T).GetCustomAttribute<IgnoreAttribute>();
            if (attr != null && attr.Ignore.HasFlag(LionSerializeContext.Persistence)) return false;

            return true;
        }
        public bool IsPersistable(Type T)
        {
            var attr = T.GetCustomAttribute<IgnoreAttribute>();
            if (attr != null && attr.Ignore.HasFlag(LionSerializeContext.Persistence)) return false;

            return true;
        }

#if !NoGenericMethods
        public T AsType<T>() where T : class
        {
            if (!HasObject)
            {
                if (!IsPersistable<T>() && !IsPersistable<ObjectType>()) return null;
                if (!TryEnsureRetrieved()) return null;
            }

            T result = _object as T;
            if (result != null) return result;

            IMultiTyped mtObj = ObjectMT;

            if (mtObj != null)
            {
                result = mtObj.AsType<T>();
            }

            return result;
        }
#endif
        [AotReplacement]
        public object AsType(Type T)
        {
            if (!HasObject)
            {
                if (!IsPersistable(T) && !IsPersistable(typeof(ObjectType))) return null;
                if (!TryEnsureRetrieved()) return null;
            }

            if (_object != null && T.IsAssignableFrom(_object.GetType()))
            {
                return _object;
            }

            IMultiTyped mtObj = ObjectMT;

            object result = null;
            if (mtObj != null)
            {
                result = mtObj.AsType(T);
            }

            return result;
        }

#if !NoGenericMethods
        public T AsTypeOrCreate<T>(bool allowMerge = true, Func<T> factory = null) where T : class
        {
            T obj = this.AsType<T>();
            if (obj == null)
            {
                if (HasObject)
                {
                    if (allowMerge && ObjectEMT != null)
                    {
                        // Ok, merge
                    }
                    else
                    {
                        if (ObjectEMT != null && !allowMerge)
                        {
                            throw new VosException("AsTypeOrCreate: allowMerge is false but object is a IMultiType");
                        }
                        else
                        {
                            throw new VosException("AsTypeOrCreate: already has object of another type");
                        }
                    }
                }

                if (ObjectEMT != null && !typeof(ObjectType).IsAssignableFrom(typeof(T)))
                {
                    throw new Exception("This handle supports objects of type " + typeof(ObjectType).FullName + " but requested type is not assignable: " + typeof(T).FullName);
                }

                if (factory != null) obj = factory();
                else obj = (T)Activator.CreateInstance(typeof(T));

                if (ObjectEMT != null)
                {
                    ObjectEMT.Set<T>(obj);
                }
                else
                {
                    var castedObj = obj as ObjectType;
#if SanityChecks
                    if (castedObj == null)
                    {
                        throw new UnreachableCodeException("castedObj == null");
                    }
#endif
                    this.Object = castedObj;
                }
            }
            return obj;
        }

        public void SetType<T>(T obj) where T : class
        {
            // Pass-thru to a Multitype object

            if (typeof(T) == typeof(ObjectType))
            {
                this.Object = obj as ObjectType;
                return;
            }

            if (typeof(ObjectType).IsAssignableFrom(typeof(T)))
            {
                ObjectType ot = obj as ObjectType;
                this.Object = ot;
                return;
            }

            var mtObj = _object as IExtendableMultiTyped;

            if (mtObj != null)
            {
                mtObj.SetType(obj, typeof(T));
                return;
            }

            throw new ArgumentException("Type not supported by this handle: " + typeof(T).FullName);
        }

        // REVIEW: Non array version with optional array version (for network)?
        public T[] OfType<T>() where T : class
        {
            if (!TryEnsureRetrieved()) return null;

            IEnumerable<T> result = _object as IEnumerable<T>;
            if (result != null) return result.ToArray();

            T resultItem = _object as T;
            if (result != null) return new T[] { resultItem };

            IMultiTyped mtObj = _object as IMultiTyped;
            if (mtObj != null)
            {
                result = mtObj.OfType<T>();
            }

            if (result == null)
            {
                return null;
            }
            else
            {
                return result.ToArray();
            }
        }
#endif
        [AotReplacement] // TODO - this Func<object> won't be found by current version of Rewriter... need to search for generic args with Func<T> and replace with Func<object>!
        public object AsTypeOrCreate(bool allowMerge, Func<object> factory, Type type) { throw new NotImplementedException("HandleBase2.AsTypeOrCreate"); }
        [AotReplacement]
        public void SetType(object obj, Type type) { throw new NotImplementedException("HandleBase2.SetType"); }
        [AotReplacement]
        public IEnumerable<object> OfType(Type T)
        {
            if (!TryEnsureRetrieved()) return null;

            IEnumerable result = _object as IEnumerable;
            IEnumerable<object> objArray = result.Cast<object>();
            if (result != null) return objArray.ToArray();

            if (_object != null && T.IsAssignableFrom(_object.GetType()))
            {
                return new object[] { _object };
            }

            object resultObj = null;

            IMultiTyped mtObj = _object as IMultiTyped;
            if (mtObj != null)
            {
                resultObj = mtObj.OfType(T);
            }

            if (resultObj == null)
            {
                return null;
            }
            else
            {
                return new object[] { resultObj };
            }
        }


        public IEnumerable<object> SubTypes {
            get {
                if (!TryEnsureRetrieved()) return null;

                //if (_object == null) return new object[] { };

                IMultiTyped mtObj = _object as IMultiTyped;
                if (mtObj != null)
                {
                    return mtObj.SubTypes;
                }

                return new object[] { _object };
            }
        }

#endregion


#region Child Accessors

        IHandle ITreeHandle.this[string subpath] {
            get {
                if (Reference == null) throw new ArgumentNullException("Reference == null");
                if (string.IsNullOrWhiteSpace(subpath)) return this;
                //return this.Reference.GetChild(VosPath.Combine(this.Reference.Path, subpath)).ToHandle(); BUG
                return this.Reference.GetChild(subpath).GetHandle<Folder>();
            }
        }


        IHandle ITreeHandle.this[IEnumerable<string> subpathChunks] {
            get {
                if (Reference == null) throw new ArgumentNullException("Reference == null");
                if (!subpathChunks.Any()) return this;
                return ((IHandle)this)[subpathChunks.ToSubPath()]; // OPTIMIZE
            }
        }

        IHandle ITreeHandle.this[IEnumerator<string> subpathChunks] {
            get {
                if (Reference == null) throw new ArgumentNullException("Reference == null");
                return ((IHandle)this)[subpathChunks.ToSubPath()]; // OPTIMIZE
            }
        }

        IHandle ITreeHandle.this[int index, string[] subpathChunks] {
            get {
                if (Reference == null) throw new ArgumentNullException("Reference == null");
                if (subpathChunks == null) return this;
                if (index == subpathChunks.Length) return this;
                return ((IHandle)this)[subpathChunks.ToSubPath()];
            }
        }

        IHandle ITreeHandle.this[params string[] subpathChunks] {
            get {
                return ((IHandle)this)[0, subpathChunks];
            }
        }

        public virtual IHandle<T1> GetHandle<T1>(params string[] subpathChunks)
            where T1 : class
        {
            return GetHandle<T1>((IEnumerable<string>)subpathChunks);
        }
        public virtual IHandle<T1> GetHandle<T1>(IEnumerable<string> subpathChunks)
            where T1 : class
        {
            //var parent = subpathChunks.Take(subpathChunks.Length - 1);

            // Uses handle factory, creates a Handle<>.
            return this.Reference.GetChildSubpath(subpathChunks).GetHandle<T1>();
        }

#endregion

#region CommandMap

        public CommandMap Commands {
            get {
                if (commands == null)
                {
                    commands = new CommandMap();
                    commands.AddCommand("Delete", obj => Delete());
                    commands.AddCommand("Save", obj => Save());
                    //commands.AddCommand("SaveAs", obj => Save());
                    //commands.AddCommand("Rename", obj => Rename());
                    OnInitializingCommands(commands);
                }
                return commands;
            }
        }
        private CommandMap commands;

        protected virtual void OnInitializingCommands(CommandMap commands)
        {
        }

#endregion

#region Misc

        private static ILogger l = Log.Get();
        private static ILogger lLoad = Log.Get("LionFire.OBus.Load");
        private static ILogger lFailLoad = Log.Get("LionFire.OBus.Fail.Load");

#endregion

    }
}
#endif