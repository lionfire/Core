using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Events;
using LionFire.Extensions.DefaultValues;
using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Resolvables;
using LionFire.Resolves;
using LionFire.Structures;
using LionFire.Threading;
using MorseCode.ITask;

namespace LionFire.Persistence.Handles
{

    //public abstract class ReadHandleBase<TValue> : ResolvesBase<IReference, TValue>, IReadHandleEx<TValue>, IRetrievableImpl<TValue>, IHas<PersistenceResultFlags>, IPersisted
    //{
    //    #region Reference

    //    public IReference Reference
    //    {
    //        get => reference;
    //        set
    //        {
    //            if (reference == value) return;
    //            if (reference != default) throw new AlreadySetException();
    //            reference = value;
    //        }
    //    }
    //    private IReference reference;

    //    #endregion

    //    public PersistenceResultFlags Object => throw new NotImplementedException();

    //}

    //public  class ReadHandleBaseCovariant<TValue> : ResolvesBaseCovariant<IReference, TValue>, IReadHandleEx<TValue>, IKeyed<string>, IRetrievableImpl<TValue>, IHas<PersistenceResultFlags>
    //{
    //    public override Task<IResolveResult<TValue>> ResolveImpl() => throw new NotImplementedException();
    //}


    /// <summary>
    /// Base class for read/write handles
    /// </summary>
    /// <remarks>
    ///  - Backing identity field: IReference
    ///  - PersistenceState
    ///  - ObjectReferenceChanged 
    ///  - ObjectChanged 
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public abstract class RBaseEx<T> : ResolvesBase<IReference, T>, IReadHandleEx<T>, IReadHandleInvariant<T>

    //, IRetrievableImpl<TValue>
    //where TValue : class
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
        public ITypedReference TypedReference => Reference as ITypedReference;

        #endregion

        public string Key
        {
            get => Reference.Key;
            set => throw new NotImplementedException("TODO");
            //set => Reference = value.GetReference(); // TODO
        }

        #endregion

        #region Construction

        protected RBaseEx() { }

        /// <param name="reference">Can be null</param>
        protected RBaseEx(IReference reference)
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));
            Reference = reference;
        }

        /// <param name="reference">Must not be null</param>
        ///// <param name="reference">If null, it should be set before the reference is used.</param>
        /// <param name="obj">Starting value for Object</param>
        protected RBaseEx(IReference reference, T obj = default) : this(reference)
        {
            SetObjectFromConstructor(obj);
        }

        protected void SetObjectFromConstructor(T initialObject)
        {
            _object = initialObject;
            // FUTURE: In the future, we may want to do something special here, like set something along the lines of PersistenceFlags.SetByUser
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

                RaiseHandleEvent(LionFire.HandleEvents.StateChanged);
                StateChanged?.Invoke(oldValue, value);
            }
        }
        private PersistenceState handleState;

        #region Derived - Convenience

        public bool IsPersisted
        {
            get => State.HasFlag(PersistenceState.UpToDate);
            set
            {
                if (value)
                {
                    State |= PersistenceState.UpToDate;
                }
                else
                {
                    State &= ~PersistenceState.UpToDate;
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

        public T Value
        {
            [Blocking]
            get
            {
                if (!IsPersisted)
                {
                    Retrieve().ConfigureAwait(false).GetAwaiter().GetResult();
                    //_ = Retrieve().Result;
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
        protected T _object;
        private object objectLock = new object();

        #region Instantiation 

        [ThreadSafe]
        public async Task<T> GetOrInstantiate()
        {
            await Get().ConfigureAwait(false);
            lock (objectLock)
            {
                if (!HasValue)
                {
                    Value = InstantiateDefault();
                }
                return _object;
            }
        }

        // No persistence, just instantiating an ObjectType

        /// <summary>
        /// Returns null if ObjectType is object or interface and TypedReference?.Type is null
        /// TODO: If ObjectType is Interface, get create type from attribute on Interface type.
        /// </summary>
        public Type GetInstantiationType()
        {
            if (typeof(T) == typeof(object))
            {
                if (TypedReference?.Type == null)
                {
                    return null;
                }
                return TypedReference.Type;
            }
            else
            {
                return typeof(T);
            }
        }

        private T InstantiateDefault(bool applyDefaultValues = true)
        {
            T result = (T)Activator.CreateInstance(GetInstantiationType() ?? throw new ArgumentNullException("Reference.Type must be set when using non-generic Handle, or when the generic type is object."));

            if (applyDefaultValues) { DefaultValueUtils.ApplyDefaultValues(result); }

            return result;
        }

        public void InstantiateAndSet(bool applyDefaultValues = true) => Value = InstantiateDefault(applyDefaultValues);
        private void InstantiateAndSetWithoutEvents(bool applyDefaultValues = true) => _object = InstantiateDefault(applyDefaultValues);

        public void EnsureInstantiated() // REVIEW: What should be done here?
        {
            //RetrieveOrCreateDefault(); ??

            if (Value == null)
            {
                InstantiateAndSet();
            }
        }
        private void EnsureInstantiatedWithoutEvents() // REVIEW: What should be done here?
        {
            if (_object == null)
            {
                InstantiateAndSetWithoutEvents();
            }
        }

        #endregion

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

        public event Action<RH<T>, T /*oldValue*/ , T /*newValue*/> ObjectReferenceChanged;
        public event Action<RH<T>> ObjectChanged;

        protected virtual void OnSavedObject() { }
        protected virtual void OnDeletedObject() { }

        protected T OnRetrievedObject(T obj)
        {
            Value = obj;
            RaiseRetrievedObject();
            this.State |= PersistenceState.UpToDate;
            return obj;
        }

        /// <summary>
        /// Reused existing Object instance, applied new retrieval results to it.
        /// </summary>
        protected void OnRetrievedObjectInPlace() => RaiseRetrievedObject();

        protected void RaiseRetrievedObject() { } // TODO

        protected void OnRetrieveFailed(IRetrieveResult<T> retrieveResult)
        {
            // TODO: Events?
        }

        public void DiscardValue()
        {
            Value = default;
            IsReachable = false;
            PersistenceResultFlags = PersistenceResultFlags.None;
            this.State &= ~PersistenceState.UpToDate;
        }

        #endregion

        /// <summary>
        /// Default: false for reference types, true for value types
        /// Extract to wrapper interface?
        /// If false, set_Object(default) is the same as DiscardValue(), which sets PersistenceFlags to None, and if true, it sets PersistenceFlags to HasUnpersistedObject (unless it was already Retrieved, and Object already is set to default).  TODO - Implement this
        /// </summary>
        public virtual bool CanObjectBeDefault => canObjectBeDefault;
        private static readonly bool canObjectBeDefault = typeof(T).IsValueType;

        //public virtual bool HasObject => CanObjectBeDefault ? (!IsDefaultValue(_object) && !this.RetrievedNullOrDefault()) : (!IsDefaultValue(_object));
        public virtual bool HasValue => State & (PersistenceState.UpToDate | PersistenceState.IncomingUpdateAvailable);

        public PersistenceResultFlags PersistenceResultFlags
        {
            get => persistenceResultFlags;
            protected set
            {
                if (persistenceResultFlags == value) return;
                persistenceResultFlags = value;
            }
        }
        private PersistenceResultFlags persistenceResultFlags;
        PersistenceResultFlags IHas<PersistenceResultFlags>.Object => PersistenceResultFlags;

        #endregion

        #region Events

        public event Action<RH<T>, HandleEvents> HandleEvents;

        protected void RaiseHandleEvent(HandleEvents eventType) => HandleEvents?.Invoke(this, eventType);


        public event EventHandler<IValueChanged<IPersistenceSnapshot<T>>> StateChanged;

        protected void RaisePersistenceEvent(ValueChanged<PersistenceSnapshot> arg) => StateChanged?.Invoke(this, arg);

        #endregion



        #region Get

        //public async ITask<IRetrieveResult<TValue>> RetrieveImpl() => (IRetrieveResult<TValue>)await ResolveImpl().ConfigureAwait(false);


        //async Task<IRetrieveResult<ObjectType>> IRetrievableImpl<ObjectType>.RetrieveObject() => await RetrieveObject().ConfigureAwait(false);
        //public async Task<bool> Retrieve()
        //{
        //    var result = await RetrieveImpl().ConfigureAwait(false);

        //    //var retrievableState = result.ToRetrievableState<TValue>(CanObjectBeDefault);
        //    //this.RetrievableState = retrievableState;

        //    this.PersistenceResultFlags = result.Flags;

        //    if (result.IsSuccess())
        //    {
        //        OnRetrievedObject(result.Object);
        //    }

        //    return result.IsSuccess();
        //}

        //async ITask<ILazyResolveResult<TValue>> ILazilyResolves<TValue>.GetValue()
        //{
        //    var result = await GetValue();
        //    return (result.HasObject, (TValue)(object)result.Object); // HARDCAST
        //}

        ///// <summary>
        ///// Invokes get_Object, forcing a lazy retrieve if it was null.  Override to avoid this.
        ///// If the user has set the object, then this will return true even if the object is not committed back to the source yet.
        ///// </summary>
        ///// <seealso cref="Exists"/>
        ///// <returns>True if an object was found after a retrieval or was manually set on the handle, false otherwise.</returns>
        //public virtual async ITask<ILazyResolveResult<TValue>> GetValue()
        //{
        //    if (HasValue)
        //    {
        //        return (true, _object);
        //    }

        //    if (!IsPersisted)
        //    {
        //        //await DoTryRetrieve().ConfigureAwait(false);
        //        await Retrieve().ConfigureAwait(false);
        //    }

        //    return (HasValue, _object); ;
        //}

        /// <summary>
        /// Invokes get_Object, forcing a lazy retrieve if it was null.  Override to avoid this.
        /// </summary>
        /// <seealso cref="Get"/>
        /// <returns>True if an object was found after a retrieval, false otherwise.</returns>
        public virtual async Task<bool> Exists(bool forceCheck = false)
        {
            if (forceCheck)
            {
                return (await RetrieveImpl().ConfigureAwait(false)).IsFound();
            }
            else if (IsPersisted)
            {
                // Note: if delete is pending, it should set IsPersisted to false after deleting
                return true;
            }
            else
            {
                await Retrieve().ConfigureAwait(false);
                return HasValue;
            }
        }

        #endregion

        #region Misc
        private static bool IsDefaultValue(T value) => EqualityComparer<T>.Default.Equals(value, default);

        #endregion
    }
}
