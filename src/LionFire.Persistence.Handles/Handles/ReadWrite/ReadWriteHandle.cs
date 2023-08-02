using LionFire.Referencing;
using LionFire.Data.Gets;
using LionFire.Structures;
using MorseCode.ITask;
using System;
using System.Collections.Generic;

namespace LionFire.Persistence.Handles
{
    //public abstract class NotifyingReadWriteHandle<TReference, TValue> : ReadWriteHandle<TReference, TValue>
    //    , INotifyPersists<TValue>
    //    , INotifyingHandleInternal<TValue>
    //            where TReference : IReference

    //{
    //    public abstract event Action<PersistenceEvent<TValue>> PersistenceStateChanged;

    //    public override TValue Value
    //    {
    //        [Blocking(Alternative = nameof(GetValue))]
    //        get => ReadCacheValue ?? GetValue().Result.Value;
    //        [PublicOnly]
    //        set
    //        {
    //            if (EqualityComparer<TValue>.Default.Equals(readCacheValue, value)) return;
    //            this.MutatePersistenceStateAndNotify(() => HandleUtils.OnUserChangedValue_ReadWrite(this, value));
    //        }
    //    }

    //}

    /// <summary>
    /// TODO: Document here how this is different from ReadWriteHandleBase (and NotifyingReadWriteHandle if it exists).  Should parallel ReadHandle.
    /// </summary>
    /// <typeparam name="TReference"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class ReadWriteHandle<TReference, TValue>
        : ReadWriteHandleBase<TReference, TValue>
        , IReadWriteHandle<TValue>
        , IReferencable<TReference>
        , IReadWriteHandle // RECENTCHANGE - added, okay?
        , INotifyingHandleInternal<TValue>
        where TReference : IReference<TValue>
    {
        public new TReference Reference => Key;
        string IKeyed<string>.Key => Key?.ToString();

        #region Construction

        protected ReadWriteHandle() { }
        protected ReadWriteHandle(TReference reference) : base(reference) { }
        //protected ReadWriteHandle(IReference reference) : base(reference) { }
        protected ReadWriteHandle(TReference reference, TValue preresolvedValue) : base(reference, preresolvedValue) { }

        #endregion

        #region OLD (Resolves)

        //public abstract IGetResult<TValue> QueryValue();
        //public abstract ITask<IGetResult<TValue>> GetValue();
        //ITask<IGetResult<TValue>> ILazilyGets<TValue>.TryGetValue() => this.GetValue().AsITask();

        #endregion

        #region Value

        //public override TValue Value
        //{
        //    [Blocking(Alternative = nameof(GetValue))]
        //    get => ReadCacheValue ?? GetValue().Result.Value;
        //    [PublicOnly]
        //    set
        //    {
        //        if (EqualityComparer<TValue>.Default.Equals(readCacheValue, value)) return;
        //        ReadCacheValue = value; // REVIEW TOTEST
        //    }
        //}

        public IPersistenceSnapshot<TValue> PersistenceState => new PersistenceSnapshot<TValue>(Flags, ReadCacheValue, HasValue);
        public object PersistenceLock { get; } = new object();

        #endregion

        #region Event Raising

        //public abstract void RaisePersistenceEvent(PersistenceEvent<TValue> ev);

        public event Action<PersistenceEvent<TValue>>? PersistenceStateChanged;
        void INotifyPersistsInternal<TValue>.RaisePersistenceEvent(PersistenceEvent<TValue> ev) => PersistenceStateChanged?.Invoke(ev);

        protected TValue OnRetrievedObject(TValue obj)
        {
            throw new NotImplementedException();
            
            // From ReadHandle:
            //using var _ = new PersistenceEventTransaction<TValue>(this);

            ////var old = PersistenceSnapshot;

            //ReadCacheValue = obj;
            //this.Flags |= PersistenceFlags.UpToDate;

            ////this.PersistenceStateChanged?.Invoke(new PersistenceEvent<TValue>
            ////{
            ////    Sender = this,
            ////    Old = old,
            ////    New = PersistenceSnapshot
            ////}); 

            ////RaiseRetrievedObject();
            //return obj;
        }

        #endregion
    }

#if OLD
    public abstract class ReadWriteHandleBase<TValue> : Resolves<IReference, TValue>, IReadHandleBase<TValue>, IReadWriteHandleBase<TValue>
         //Resolves<IReference, TValue>, RH<TValue>, IReadHandleInvariant<TValue>
        //, ICommitableImpl, IDeletableImpl
    {
    #region Reference

        protected virtual bool IsAllowedReferenceType(Type type) => true;

        [SetOnce]
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

                // OLD: art != null && value != null && !art.Where(type => type.IsAssignableFrom(value.GetType())).Any()
                if (!IsAllowedReferenceType(value.GetType()))
                {
                    throw new ArgumentException("This type does not support IReference types of that type.  See protected IsAllowedReferenceType implementation for allowed types.");
                }

                reference = value;
            }
        }
        protected IReference reference;

    #endregion

    #region Persistence State

        public PersistenceFlags Flags
        {
            get => handleState;
            set
            {
                if (handleState == value) { return; }

                var oldValue = handleState;
                handleState = value;

                OnStateChanged(value, oldValue);
            }
        }

        TValue IWrapper<TValue>.Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        TValue IWriteWrapper<TValue>.Value { set => throw new NotImplementedException(); }

        private PersistenceFlags handleState;

        protected virtual void OnStateChanged(PersistenceFlags newValue, PersistenceFlags oldValue) { }
        
    #endregion

    #region Construction

        protected ReadWriteHandleBase() { }

        protected ReadWriteHandleBase(IReference input) : base(input) { }

        ///// <summary>
        ///// Do not use this in derived classes that are purely resolve-only and not intended to set an initial value.
        ///// </summary>
        ///// <param name="input"></param>
        ///// <param name="initialValue"></param>
        //protected ReadWriteHandleBase(IReference input, TValue initialValue) : base(input, initialValue)
        //{
        //}

    #endregion
    }
#endif
}
