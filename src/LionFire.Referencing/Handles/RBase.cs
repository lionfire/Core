using LionFire.Referencing.Persistence;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LionFire.Referencing
{

    /// <summary>
    /// Base class for read/write handles
    /// </summary>
    /// <typeparam name="ObjectType"></typeparam>
    public abstract class RBase<ObjectType> : R<ObjectType>
        where ObjectType : class
    {
        #region Identity

        #region Reference

        public IReference Reference
        {
            get { return reference; }
            protected set
            {
                if (reference == value) return;
                if (reference != default(IReference)) throw new AlreadySetException();
                reference = value;
            }
        }
        private IReference reference;

        #endregion

        public string Key
        {
            get=> Reference.Key;
            
            set => this.Reference = Injection.GetService<IReferenceFactory>().ToReference(value);
            
        }
        
        #endregion

        #region Construction

        protected RBase() { }
        protected RBase(IReference reference) { this.Reference = reference; }
        public RBase(IReference reference, ObjectType obj = null) : this(reference) {
            this._object = obj;
        }

        #endregion

        #region State

        #region HandleState

        public HandleState HandleState
        {
            get { return handleState; }
            set
            {
                if (handleState == value) return;
                var oldValue = handleState;
                handleState = value;

                RaiseEvent(LionFire.HandleEvents.StateChanged);
                HandleStateChangedFromTo?.Invoke(oldValue, value);
            }
        }
        private HandleState handleState;

        public event Action<HandleState,HandleState> HandleStateChangedFromTo;

        #endregion

        public bool IsPersisted
        {
            get { return HandleState.HasFlag(HandleState.Persisted); }
            set {
                if(value)
                {
                    HandleState |= HandleState.Persisted;
                }
                else
                {
                    HandleState &= ~HandleState.Persisted;
                }
            }
        }


        #region Object

        public ObjectType Object
        {
            [Blocking]
            get
            {
                if (!isResolved)
                {
                    DoTryRetrieve().Wait();
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
                ObjectReferenceChanged?.Invoke(this, oldValue, value);
                ObjectChanged?.Invoke(this);
            }
        }

        protected virtual async Task<bool> DoTryRetrieve()
        {
            bool result;
            if (!(result = (await TryRetrieveObject().ConfigureAwait(false))))
            {
                OnRetrieveFailed();
            }
            return result;
        }

        protected ObjectType _object;
        public event Action<R<ObjectType>, ObjectType /*oldValue*/ , ObjectType /*newValue*/> ObjectReferenceChanged;
        public event Action<R<ObjectType>> ObjectChanged;

        protected void OnRetrievedObject(ObjectType obj)
        {
            this.Object = obj; // TODO FUTURE: Bypass events, or trigger different events (don't trigger "user changed", but instead "retrieved")
        }
        protected void OnRetrieveFailed()
        {
            // TODO: Events?
        }

        public bool HasObject => _object != null;

        public void ForgetObject()
        {
            Object = null;
            IsRetrieved = false;
        }

        #endregion

        #region IsResolved

        public bool IsRetrieved
        {
            get { return isResolved; }
            protected set
            {
                if (isResolved == value)
                {
                    return;
                }

                isResolved = value;
                IsRetrievedChanged?.Invoke(value);
            }
        }
        private bool isResolved;

        public event Action<bool> IsRetrievedChanged;

        #endregion

        #endregion

        #region Events

        public event Action<R<ObjectType>, HandleEvents> HandleEvents;

        protected void RaiseEvent(HandleEvents eventType)
        {
            HandleEvents?.Invoke(this, eventType);
        }

        #endregion

        #region Retrieve

        public abstract Task<bool> TryRetrieveObject();

        /// <summary>
        /// Invokes get_Object, forcing a lazy retrieve if it was null.  Override to avoid this.
        /// If the user has set the object, then this will return true even if the object is not committed back to the source yet.
        /// </summary>
        /// <seealso cref="Exists"/>
        /// <returns>True if an object was found after a retrieval or was manually set on the handle, false otherwise.</returns>
        public virtual async Task<bool> TryGetObject()
        {
            if (HasObject) return true;

            if (!IsRetrieved)
            {
                await DoTryRetrieve().ConfigureAwait(false);
            }

            return HasObject;
        }

        /// <summary>
        /// Invokes get_Object, forcing a lazy retrieve if it was null.  Override to avoid this.
        /// </summary>
        /// <seealso cref="TryGetObject"/>
        /// <returns>True if an object was found after a retrieval, false otherwise.</returns>
        public virtual async Task<bool> Exists(bool forceCheck = false)
        {
            if (forceCheck)
            {
                return await TryRetrieveObject().ConfigureAwait(false);
            }
            else if (IsRetrieved)
            {
                return HasObject;
            }
            else
            {
                await TryRetrieveObject().ConfigureAwait(false);
                return HasObject;
            }
        }

        #endregion

    }
}
