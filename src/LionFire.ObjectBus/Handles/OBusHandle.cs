//#define DEBUG_LOAD
//#define TRACE_LOAD_FAIL
//using LionFire.Input; REVIEW
//using LionFire.Extensions.DefaultValues; REVIEW

using System;
using System.Threading.Tasks;
using LionFire.ObjectBus.Resolution;
using LionFire.Referencing;

namespace LionFire.ObjectBus
{
    public class OBusHandle<TObject> : OBusHandle<TObject, object>
    {
        public OBusHandle() { }
        public OBusHandle(IReference reference, TObject handleObject = default(TObject)) : base(reference, handleObject) { }
    }

    //public class OBaseResolutionData
    //{        
    //}

    /// <summary>
    ///  TODO NEXT: A standard handle can contain: IReference, TObject, cached OBase, cached OBase resolution data
    ///  (e.g. sub-reference for VOS, or just path for FS).  If the obase or obase resolution data can change/expire, a
    ///  more sophisticated handle might be needed.  A genericized handle could be the same, but be strongly typed
    ///  for the OBase resolution data and/or IReference and/or OBase??
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public class OBusHandle<TObject, TOBaseData> : WBase<TObject>, H<TObject>
    //where TOBaseData : OBaseResolutionData
    {
        #region OBase


        public IOBase OBase
        {
            get => obase;
            set
            {
                if (obase == value)
                {
                    return;
                }

                if (obase != default(IOBase))
                {
                    throw new AlreadySetException();
                }

                obase = value;
            }
        }
        private IOBase obase;

        #endregion

        #region OBus

        public IOBus OBus
        {
            get => obus ?? OBase?.OBus;
            set
            {
                if (obus == value)
                {
                    return;
                }

                if (obus != default(IOBus))
                {
                    throw new AlreadySetException();
                }

                obus = value;
            }
        }
        private IOBus obus;

        #endregion

        #region Construction

        public OBusHandle() { }
        public OBusHandle(IReference reference, TObject handleObject = default(TObject))
        {
            Reference = reference;
            _object = handleObject;
        }

        #endregion

        public void ResetResolutionCache() => ResolutionInfo = null;
        public ResolutionInfo<TOBaseData> ResolutionInfo { get; set; }

        [SetOnce]
        private object OBaseData { get; }

        private void EnsureOBaseResolved()
        {
            if (OBase != null) return;

            IOBase obase = OBase;

            if (obase == null)
            {
                // REVIEW - keep OBase/OBus in less places?  Should ResolutionInfo not be transient?
                obase = ResolutionInfo?.OBase;
            }

            if (obase == null)
            {
                IOBus obaseProvider;
                (obaseProvider, obase) = Reference.TryResolve();
                if (obase == null)
                {
                    ResolutionInfo = null;
                }
                else
                {
                    if (ResolutionInfo == null)
                    {
                        ResolutionInfo = new ResolutionInfo<TOBaseData>
                        {
                            OBase = obase,
                        };
                    }
                }
            }

            OBase = obase ?? throw new ObjectBusException("Could not resolve OBase for OBusHandle.  Reference: " + this.Reference);
        }

        public override async Task<bool> TryRetrieveObject()
        {
            if (Reference == null) throw new ArgumentNullException(nameof(Reference));
            EnsureOBaseResolved();
            
            var result = await OBase.TryGet<TObject>(Reference).ConfigureAwait(false);

            if (result.IsSuccess)
            {
                OnRetrievedObject(result.Result);
            }
            else
            {
                OnRetrieveFailed(result);
            }
            return result.IsSuccess;
        }

        protected override async Task<bool?> DeleteObject(object persistenceContext = null)
        {
            if (Reference == null) throw new ArgumentNullException(nameof(Reference));
            EnsureOBaseResolved();

            var result = await OBase.TryDelete(Reference).ConfigureAwait(false);

            if (result != false)
            {
                OnDeletedObject();  
            }
            return result;
        }
        protected override async Task WriteObject(object persistenceContext = null)
        {
            if (Reference == null) throw new ArgumentNullException(nameof(Reference));
            EnsureOBaseResolved();

            await OBase.Set(Reference, this._object, typeof(TObject)).ConfigureAwait(false);

            //if (result.IsSuccess)
            {
                OnSavedObject(); 
            }
            //else
            //{
            //    OnRetrieveFailed(result);
            //}
            //return result.IsSuccess;
        }
    }

    public class OBusHandle<TObject, TReference, TOBase, TOBaseData>
    {

        [SetOnce]
        private TReference Reference { get; set; }

        [SetOnce]
        private TOBase OBase { get; }

        [SetOnce]
        private TOBaseData OBaseData { get; }
    }



#if TOPORT // OLD - see above
    // TODO: Use IOC, extension methods here instead of this class
    public class OBusHandle<ObjectType> : HandleBase<ObjectType>
        where ObjectType : class
    {
    #region Construction

        public OBusHandle(ObjectType obj = null, bool freezeObjectIfProvided = true)
            : base(obj, freezeObjectIfProvided)
        {
        }

        internal OBusHandle(string uri, ObjectType obj = null, bool freezeObjectIfProvided = true)
            : base(uri, obj, freezeObjectIfProvided)
        {
        }

        public OBusHandle(IReference reference, ObjectType obj = null, bool freezeObjectIfProvided = true)
            : base(reference, obj, freezeObjectIfProvided)
        {
        }

        public OBusHandle(IReferencable referencable, ObjectType obj = null, bool freezeObjectIfProvided = true)
            : base(referencable, obj, freezeObjectIfProvided)
        {
            IReference reference = referencable.Reference;
            this.Reference = referencable.Reference;
        }

    #endregion


    #region Reference

        [SetOnce]
        public override IReference Reference
        {
            get { return reference; }
            set
            {
                if (reference == value)
                {
                    return;
                }

                if (reference != default(IReference))
                {
                    throw new NotSupportedException("Reference can only be set once.");
                }

                if (!reference.IsValid())
                {
                    throw new ArgumentException("Reference is invalid");
                }

                var oldReference = reference;


                if (value != null)
                {
                    if (value.Type == null)
                    {
                        //IChangeableReferencable cr = reference as IChangeableReferencable;
                        //if (cr != null)
                        //{
                        //    cr.Type = typeof(T);
                        //}
                    }
                    else
                    {
                        if (value.Type != typeof(T))
                        {
                            if (!typeof(ObjectType).IsAssignableFrom(value.Type))
                            {
                                throw new ArgumentException("!typeof(ObjectType).IsAssignableFrom(value.Type)");
                            }
                        }
                    }
                }
                reference = value;
            }
        }
        private IReference reference;

    #endregion

    #region RetrieveInfo

        public RetrieveInfo RetrieveInfo { get; set; }
        public virtual bool IsRetrieveInfoEnabled
        {
            get { return RetrieveInfo != null; }
            set
            {
                if (value) { if (RetrieveInfo == null) { RetrieveInfo = new RetrieveInfo(); } }
                else
                {
                    RetrieveInfo = null;
                }
            }
        }

    #endregion

    #region Saving
      

        public virtual Task Save(object persistenceContext = null)
        {
            throw new NotImplementedException();
        }
        public virtual void Save(bool allowDelete = false, bool preview = false)
        {
            RaiseSaving();

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
        public virtual bool TryRetrieve(bool setToNullOnFail = true)
        {
            //			Log.Info("ZX HandleBase2.TryRetrieve");

            if (!setToNullOnFail)
            {
                throw new NotImplementedException("setToNullOnFail = false");
            }

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

        public override Task<bool> TryResolveObject(object persistenceContext = null)
        {
            throw new Exception("TODO: See also: TryRetrieve and refactor");
            ObjectType result;

            // TODO: async
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

            return Task.FromResult(result != null);
        }
}
#endif
}