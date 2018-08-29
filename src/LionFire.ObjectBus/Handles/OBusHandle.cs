#if TOPORT
//#define DEBUG_LOAD
//#define TRACE_LOAD_FAIL
//using LionFire.Input; REVIEW
//using LionFire.Extensions.DefaultValues; REVIEW

using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{

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
}
#endif