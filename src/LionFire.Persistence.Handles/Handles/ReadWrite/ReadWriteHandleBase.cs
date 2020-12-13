using LionFire.Persistence;
using LionFire.Persistence.Implementation;
using LionFire.Referencing;
using LionFire.Resolves;
using LionFire.Threading;
using MorseCode.ITask;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{
    public abstract class ReadWriteHandleBase<TReference, TValue> 
        : WriteHandleBase<TReference, TValue>
        , IReadWriteHandleBase<TValue>
        , IHandleInternal<TValue>
        where TReference : IReference
    {
        #region Construction

        public ReadWriteHandleBase() { }
        public ReadWriteHandleBase(TReference reference) : base(reference) { }
        public ReadWriteHandleBase(TReference reference, TValue preresolvedValue) : base(reference, preresolvedValue) { }

        #endregion

        #region Value

        public override TValue Value
        {
            [Blocking(Alternative = nameof(GetValue))]
            get => ProtectedValue ?? GetValue().Result.Value;
            [PublicOnly]
            set
            {
                if (object.Equals(value, protectedValue)) return;
                //if (System.Collections.Generic.Comparer<TValue>.Default.Compare(protectedValue, value) == 0) return; // Should use Equality instead of Compare?
                //if (value == ProtectedValue) return;
                HandleUtils.OnUserChangedValue_ReadWrite(this, value);
            }
        }
        TValue IHandleInternal<TValue>.ProtectedValue { get => ProtectedValue; set => ProtectedValue = value; }
        PersistenceFlags IHandleInternal<TValue>.Flags { set => Flags = value; }

        #endregion

        /// <summary>
        /// Returns GetValue().Value if HasValue is true, otherwise uses InstantiateDefault() to populate Value and returns that.
        /// </summary>
        /// <returns>A guaranteed Value, that may have been preexisting, lazily loaded, or just instantiated.</returns>
        [ThreadSafe]
        public async ITask<IResolveResult<TValue>> GetOrInstantiateValue()
        {
            var getResult = await GetValue().ConfigureAwait(false);
            if (getResult.HasValue) return getResult;

            //TrySetProtectedValueIfDefault(InstantiateDefault());
            //ProtectedValue = InstantiateDefault();
            var newValue = InstantiateDefault();
            this.OnUserChangedValue_Write(newValue);

            return RetrieveResult<TValue>.NotFoundButInstantiated(newValue);

            // Consider .NET's LazyInitializer
            //lock (objectLock)
            //{
            //    if (!HasValue)
            //    {
            //        ProtectedValue = InstantiateDefault();
            //    }
            //    return ProtectedValue;
            //}
        }

        async Task<bool?> IDeletable.Delete()
        {
            MarkDeleted();
            var putResult = await Put();
            return putResult.IsFound();
        }

        void IDeletable.MarkDeleted() => this.OnUserChangedValue_ReadWrite(default);

        ITask<IResolveResult<TValue>> IResolves<TValue>.Resolve() => Resolve().AsITask();
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
