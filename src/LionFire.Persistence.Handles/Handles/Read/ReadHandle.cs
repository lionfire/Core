﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LionFire.Activation;
using LionFire.Events;
using LionFire.Extensions.DefaultValues;
using LionFire.Ontology;
using LionFire.Referencing;
using LionFire.Resolvables;
using LionFire.Data.Async.Gets;
using LionFire.Structures;
using LionFire.Threading;
using MorseCode.ITask;
using LionFire.Persistence.Persisters;
using System.Diagnostics;

namespace LionFire.Persistence.Handles
{
    /// <summary>
    /// Base class for read/write handles, implementing IReadHandleEx&lt;TValue&gt;
    /// </summary>
    /// <remarks>
    ///  - Backing identity field: IReference
    ///  - PersistenceState
    ///  - ObjectReferenceChanged 
    ///  - ObjectChanged 
    /// </remarks>
    /// <typeparam name="TValue"></typeparam>
    public abstract partial class ReadHandle<TReference, TValue>
        : ReadHandleBase<TReference, TValue>
        , IReadHandle<TValue>
        //IReadHandleInvariantEx<TValue>, 
        , INotifyPersists<TValue>
        , INotifyPropertyChanged
        , INotifyPersistsInternal<TValue>
        //, IRetrievableImpl<TValue>
        , IPersistenceAware<TValue>
    where TReference : IReference<TValue>
    {
        #region Identity

        string IKeyed<string>.Key => Reference.Key;

        #endregion

        #region Construction

        protected ReadHandle() { }

        /// <param name="reference">Can be null</param>
        protected ReadHandle(TReference reference) : base(reference) { }

#nullable enable
        /// <param name="reference">Must not be null</param>
        ///// <param name="reference">If null, it should be set before the reference is used.</param>
        /// <param name="preresolvedValue">Starting value for Object</param>
        protected ReadHandle(TReference reference, TValue? preresolvedValue) : base(reference, preresolvedValue)
        {
        }
#nullable disable

        #endregion

        #region State

        //private readonly object objectLock = new object();

        #region Value

        // OLD - Maybe I don't need to override Value or ReadCacheValue?
        //public TValue Value
        //{
        //    [Blocking(Alternative = nameof(GetValue))]
        //    get
        //    {
        //        // TODO: Determine
        //        if (!Flags.HasFlag(PersistenceFlags.UpToDate))
        //        {
        //            Resolve().ConfigureAwait(false).GetAwaiter().GetResult();
        //            //_ = Get().Result;
        //        }
        //        return _value;
        //    }
        //    set
        //    {
        //        if (object.ReferenceEquals(_value, value))
        //        {
        //            return;
        //        }
        //        var oldValue = _value;
        //        _value = value;
        //        ObjectReferenceChanged?.Invoke(this, oldValue, value);
        //        ObjectChanged?.Invoke(this);
        //    }
        //}
        //protected TValue _value;

        protected override void OnValueChanged(TValue newValue, TValue oldValue) { OnPropertyChanged(nameof(Value)); base.OnValueChanged(newValue, oldValue); }

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

        //public event Action<IReadHandleBase<TValue>, TValue /*oldValue*/ , TValue /*newValue*/> ObjectReferenceChanged;
        //public event Action<IReadHandleBase<TValue>> ObjectChanged;

        protected virtual void OnSavedObject() { }
        protected virtual void OnDeletedObject() { }

        protected TValue OnRetrievedObject(TValue obj)
        {
            using var _ = new PersistenceEventTransaction<TValue>(this);

            //var old = PersistenceSnapshot;

            ReadCacheValue = obj;
            this.Flags |= PersistenceFlags.UpToDate;

            //this.PersistenceStateChanged?.Invoke(new PersistenceEvent<TValue>
            //{
            //    Sender = this,
            //    Old = old,
            //    New = PersistenceSnapshot
            //}); 

            //RaiseRetrievedObject();
            return obj;
        }

        IPersistenceSnapshot<TValue> IPersists<TValue>.PersistenceState
             => new PersistenceSnapshot<TValue>
             {
                 Value = ReadCacheValue,
                 Flags = Flags,
                 HasValue = HasValue,
             };

        /// <summary>
        /// Reused existing Object instance, applied new retrieval results to it.
        /// </summary>
        protected void OnRetrievedObjectInPlace() => RaiseRetrievedObject();

        protected void RaiseRetrievedObject() { } // TODO

        protected void OnRetrieveFailed(IGetResult<TValue> retrieveResult)
        {
            // TODO: Events?
        }

        //protected override void OnValueChanged(TValue newValue, TValue oldValue) { 

        //}

        public override void DiscardValue()
        {
            base.DiscardValue();
            //TransferResultFlags = TransferResultFlags.None;
            this.Flags = Flags.AfterDiscard();
        }

        #endregion

        /// <summary>
        /// Default: false for reference types, true for value types
        /// Extract to wrapper interface?
        /// If false, set_Object(default) is the same as DiscardValue(), which sets PersistenceFlags to None, and if true, it sets PersistenceFlags to HasUnpersistedObject (unless it was already Retrieved, and Object already is set to default).  TODO - Implement this
        /// </summary>
        public virtual bool CanObjectBeDefault => canObjectBeDefault;
        private static readonly bool canObjectBeDefault = typeof(TValue).IsValueType;

        //public virtual bool HasObject => CanObjectBeDefault ? (!IsDefaultValue(_object) && !this.RetrievedNullOrDefault()) : (!IsDefaultValue(_object));
        //public virtual bool HasValue => State & (PersistenceState.UpToDate | PersistenceState.IncomingUpdateAvailable); // This looks wrong

        //public TransferResultFlags TransferResultFlags
        //{
        //    get => persistenceResultFlags;
        //    protected set
        //    {
        //        if (persistenceResultFlags == value) return;
        //        persistenceResultFlags = value;
        //    }
        //}
        //private TransferResultFlags persistenceResultFlags;

        #endregion

        #region Events

#nullable enable
        public event Action<PersistenceEvent<TValue>>? PersistenceStateChanged;
        void INotifyPersistsInternal<TValue>.RaisePersistenceEvent(PersistenceEvent<TValue> ev) => PersistenceStateChanged?.Invoke(ev);
#nullable disable


        //public event Action<IReadHandleBase<TValue>, HandleEvents> HandleEvents;

        //protected void RaiseHandleEvent(HandleEvents eventType) => HandleEvents?.Invoke(this, eventType);


        //protected void RaisePersistenceEvent(ValueChanged<PersistenceSnapshot> arg) => StateChanged?.Invoke(this, arg);

        //public event EventHandler<IValueChanged<IPersistenceSnapshot<TValue>>> StateChanged;

        #endregion

        #region (public) Event Handlers

        public void OnPreresolved(TValue? preresolvedValue)
        {
            if (EqualityComparer.Equals(preresolvedValue, default)) return;
            if (ReferenceEquals(ReadCacheValue, preresolvedValue)) return;
            if (EqualityComparer<TValue>.Default.Equals(ReadCacheValue, preresolvedValue))
            {
                Debug.WriteLine("ReadHandle.OnPreresolved: EqualityComparer<TValue>.Default.Equals(ReadCacheValue, preresolvedValue). ReadCacheValue: " + ReadCacheValue);
                return;
            }

            if (ReadCacheValue != null)
            {
                Debug.WriteLine("ReadHandle.OnPreresolved: ReadCacheValue != null && !ReferenceEquals(ReadCacheValue, preresolvedValue).  ReadCacheValue: " + ReadCacheValue + " PreresolvedValue: " + preresolvedValue);
            }
            ReadCacheValue = preresolvedValue;
        }

        #endregion

        #region Get

        //public async ITask<IGetResult<TValue>> RetrieveImpl() => (IGetResult<TValue>)await GetImpl().ConfigureAwait(false);


        //async Task<IGetResult<ObjectType>> IRetrievableImpl<ObjectType>.RetrieveObject() => await RetrieveObject().ConfigureAwait(false);
        //public async Task<bool> Get()
        //{
        //    var result = await RetrieveImpl().ConfigureAwait(false);

        //    //var retrievableState = result.ToRetrievableState<TValue>(CanObjectBeDefault);
        //    //this.RetrievableState = retrievableState;

        //    this.TransferResultFlags = result.Flags;

        //    if (result.IsSuccess())
        //    {
        //        OnRetrievedObject(result.Object);
        //    }

        //    return result.IsSuccess();
        //}

        //async ITask<IGetResult<TValue>> ILazilyGets<TValue>.GetValue()
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
        //public virtual async ITask<IGetResult<TValue>> GetValue()
        //{
        //    if (HasValue)
        //    {
        //        return (true, _object);
        //    }

        //    if (!IsPersisted)
        //    {
        //        //await DoTryRetrieve().ConfigureAwait(false);
        //        await Get().ConfigureAwait(false);
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
                return (await GetImpl().ConfigureAwait(false)).HasValue;
            }
            else if (HasValue && IsUpToDate)
            {
                // Note: if delete is pending, it should set IsPersisted to false after deleting
                return true;
            }
#if true
            throw new NotImplementedException("Figure out the logic on this");
#else
            else
            {
                var result = await Resolve().ConfigureAwait(false);
                return result.HasValue;
            }
#endif
        }

        #endregion

        #region Misc


        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        private static bool IsDefaultValue(TValue value) => EqualityComparer<TValue>.Default.Equals(value, default);

        #endregion
    }
}
