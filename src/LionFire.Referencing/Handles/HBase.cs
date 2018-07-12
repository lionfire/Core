using LionFire.Referencing.Resolution;
using System;
using System.Threading.Tasks;

namespace LionFire.Referencing
{    public abstract class HBase<ObjectType> : IH<ObjectType>
    where ObjectType : class
    {
        public abstract string Key { get; set; }
        public abstract IReference Reference { get; set; }

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
            protected set
            {
                if (object.ReferenceEquals(_object, value))
                {
                    return;
                }

                var oldValue = _object;
                _object = value;
                ObjectChanged?.Invoke(this, oldValue, value);
            }
        }
        private ObjectType _object;
        public event Action<IReadHandle<ObjectType>, ObjectType /*oldValue*/ , ObjectType /*newValue*/> ObjectChanged;

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

    }
}
