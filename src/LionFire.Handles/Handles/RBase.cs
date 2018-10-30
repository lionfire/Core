using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.DependencyInjection;
using LionFire.ObjectBus;
using LionFire.Persistence;
using LionFire.Structures;

namespace LionFire.Referencing
{
    /// <summary>
    /// Base class for read/write handles
    /// </summary>
    /// <remarks>
    ///  - Backing identity field: IReference
    ///  - PersistenceState
    ///  - ObjectReferenceChanged 
    ///  - ObjectChanged 
    /// </remarks>
    /// <typeparam name="ObjectType"></typeparam>
    public abstract class RBase<ObjectType> : R<ObjectType>, IKeyed<string>
        //where ObjectType : class
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
            set => Reference = value.ToReference();
        }

        #endregion

        #region Construction

        protected RBase() { }
        protected RBase(IReference reference) { Reference = reference; }
        public RBase(IReference reference, ObjectType obj = default(ObjectType)) : this(reference)
        {
            _object = obj;
        }

        #endregion

        #region State

        #region State

        public PersistenceState State
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
                StateChanged?.Invoke(oldValue, value);
            }
        }
        private PersistenceState handleState;

        public event PersistenceStateChangeHandler StateChanged;

        #region Derived - Convenience

        public bool IsPersisted
        {
            get => State.HasFlag(PersistenceState.Persisted);
            set
            {
                if (value)
                {
                    State |= PersistenceState.Persisted;
                }
                else
                {
                    State &= ~PersistenceState.Persisted;
                }
            }
        }

        #region Reachable

        public bool? IsReachable
        {
            get => State.HasFlag(PersistenceState.Reachable) ? true : (State.HasFlag(PersistenceState.Reachable) ? false : (bool?)null);
            set
            {
                if (value.HasValue)
                {
                    if (value.Value)
                    {
                        State |= PersistenceState.Reachable;
                        State &= ~PersistenceState.Unreachable;
                    }
                    else
                    {
                        State |= PersistenceState.Unreachable;
                        State &= ~PersistenceState.Reachable;
                    }
                }
                else
                {
                    State &= ~(PersistenceState.Reachable | PersistenceState.Unreachable);
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region Object

        public ObjectType Object
        {
            [Blocking]
            get
            {
                if (!IsPersisted)
                {
                    TryRetrieveObject().ConfigureAwait(false).GetAwaiter().GetResult();
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
        protected ObjectType _object;

        public async Task<ObjectType> GetObject()
        {
            if (!IsPersisted)
            {
                await TryRetrieveObject().ConfigureAwait(false);
            }
            return _object;
        }

        //protected virtual async Task<bool> DoTryRetrieve()
        //{
        //    return 
        //    bool result;
        //    if (!(result = (await TryRetrieveObject().ConfigureAwait(false))))
        //    {
        //        OnRetrieveFailed();
        //    }
        //    return result;
        //}
        
        public event Action<R<ObjectType>, ObjectType /*oldValue*/ , ObjectType /*newValue*/> ObjectReferenceChanged;
        public event Action<R<ObjectType>> ObjectChanged;

        protected virtual void OnSavedObject() { }

        protected void OnRetrievedObject(ObjectType obj) => Object = obj; // TODO FUTURE: Bypass events, or trigger different events (don't trigger "user changed", but instead "retrieved")
        protected void OnRetrieveFailed(IRetrieveResult<ObjectType> retrieveResult)
        {
            // TODO: Events?
        }

        public bool HasObject => EqualityComparer<ObjectType>.Default.Equals(_object, default(ObjectType)); 

        public void ForgetObject()
        {
            Object = default(ObjectType);
            IsReachable = false;
        }

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

            if (!IsPersisted)
            {
                //await DoTryRetrieve().ConfigureAwait(false);
                await TryRetrieveObject().ConfigureAwait(false);
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
            else if (IsPersisted)
            {
                // Note: if delete is pending, it should set IsPersisted to false after deleting
                return true;
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
