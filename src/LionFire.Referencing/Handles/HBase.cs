using LionFire.Referencing.Resolution;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LionFire.Referencing
{
    // REVIEW - Deletion logic

    /// <summary>
    /// Base class for read/write handles
    /// </summary>
    /// <typeparam name="ObjectType"></typeparam>
    public abstract class RBase<ObjectType> : R<ObjectType>
    where ObjectType : class
    {
        public abstract string Key { get; set; }
        public abstract IReference Reference { get; set; }

        #region Construction

        protected RBase() { }
        protected RBase(ObjectType obj) { this._object = obj; }

        #endregion

        #region Object

        public ObjectType Object
        {
            [Blocking]
            get
            {
                if (!isResolved)
                {
                    if(!TryResolveObject().Result)
                    {
                        OnRetrieveFailed();
                    }
                }
                return _object;
            }
            set
            {
                if (object.ReferenceEquals(_object, value))
                {
                    return;
                }

                var oldValue = _object;
                _object = value;
                OnObjectChanged();
                ObjectChanged?.Invoke(this, oldValue, value);
            }
        }

        protected virtual void OnObjectChanged()
        {
        }

        protected ObjectType _object;
        public event Action<R<ObjectType>, ObjectType /*oldValue*/ , ObjectType /*newValue*/> ObjectChanged;

        protected void OnRetrievedObject(ObjectType obj)
        {
            this.Object = obj; // TODO FUTURE: Bypass events, or trigger different events (don't trigger "user changed", but instead "retrieved")
        }
        protected void OnRetrieveFailed()
        {
            // TODO: Events?
        }

        public bool HasObject => _object != null;

        #region IsResolved

        public bool IsResolved
        {
            get { return isResolved; }
            protected set
            {
                if (isResolved == value)
                {
                    return;
                }

                isResolved = value;
                IsResolvedChanged?.Invoke(value);
            }
        }
        private bool isResolved;

        public event Action<bool> IsResolvedChanged;

        #endregion

        public void ForgetObject()
        {
            Object = null;
            IsResolved = false;
        }

        #endregion

        #region Resolution

        public abstract Task<bool> TryResolveObject();

        #endregion

        //#region PersistenceContext

        //// TODO: Make this MultiTyped and allow OBases to add to it for different reasons?
        //// Or use ConditionalWeakTable?
        //public object PersistenceContext
        //{
        //    get { return persistenceContext; }
        //    set
        //    {
        //        if (persistenceContext == value)
        //        {
        //            return;
        //        }

        //        if (persistenceContext != default(object))
        //        {
        //            throw new AlreadySetException();
        //        }

        //        persistenceContext = value;
        //    }
        //}
        //private object persistenceContext;

        //#endregion

        

        //public void SetObject(T value)
        //{
        //    this.Object = value;
        //    if (value == null)
        //    {
        //        DeletePending = true;
        //    }
        //}

        
    }
}
