using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Structures;

namespace LionFire.Referencing
{
    /// <summary>
    /// Base class for read/write handles
    /// </summary>
    /// <typeparam name="ObjectType"></typeparam>
    public abstract class RBase<ObjectType> : R<ObjectType>, IKeyed<string>
        where ObjectType : class
    {
        /// <summary>
        /// Reference only allows types assignable to types in this Enumerable
        /// </summary>
        public virtual IEnumerable<Type> AllowedReferenceTypes => null;

        #region Identity

        #region Reference

        public IReference Reference
        {
            get => reference;
            protected set
            {
                if (reference == value)
                {
                    return;
                }

                if (reference != default(IReference))
                {
                    throw new AlreadySetException();
                }

                var art = AllowedReferenceTypes;
                if (art != null && value != null && !art.Where(type => type.IsAssignableFrom(value.GetType())).Any())
                {
                    throw new ArgumentException("This type does not support IReferences of that type.  See AllowedReferenceTypes for allowed types.");
                }

                reference = value;
            }
        }
        protected IReference reference;

        #endregion

        public string Key
        {
            get => Reference.Key;
            set => Reference = Injection.GetService<IReferenceFactory>().ToReference(value);
        }

        #endregion

        #region Construction

        protected RBase() { }
        protected RBase(IReference reference) { Reference = reference; }
        public RBase(IReference reference, ObjectType obj = null) : this(reference)
        {
            _object = obj;
        }

        #endregion

        #region State

        #region HandleState

        public PersistenceState HandleState
        {
            get => handleState;
            set
            {
                if (handleState == value)
                {
                    return;
                }

                var oldValue = handleState;
                handleState = value;

                RaiseEvent(LionFire.HandleEvents.StateChanged);
                HandleStateChangedFromTo?.Invoke(oldValue, value);
            }
        }
        private PersistenceState handleState;

        public event Action<PersistenceState, PersistenceState> HandleStateChangedFromTo;

        #endregion

        // REVIEW - is this useful?
        public bool IsPersisted
        {
            get => HandleState.HasFlag(PersistenceState.Persisted);
            set
            {
                if (value)
                {
                    HandleState |= PersistenceState.Persisted;
                }
                else
                {
                    HandleState &= ~PersistenceState.Persisted;
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

        protected void OnRetrievedObject(ObjectType obj) => Object = obj; // TODO FUTURE: Bypass events, or trigger different events (don't trigger "user changed", but instead "retrieved")
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
            get => isResolved;
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

        protected void RaiseEvent(HandleEvents eventType) => HandleEvents?.Invoke(this, eventType);

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
            if (HasObject)
            {
                return true;
            }

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
