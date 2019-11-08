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
    // REVIEW: Can/should I use ReadHandleBase, or a separate class?  I could write it from scratch and merge if it is the same

    public abstract class ReadWriteHandleBase<TValue> : WriteHandleBase<IReference, TValue>, IReadWriteHandleBase<TValue>
        where TValue : class
    {

        #region Value

        public override TValue Value
        {
            [Blocking(Alternative = nameof(GetValue))]
            get => ProtectedValue ?? GetValue().Result.Value;
            [PublicOnly]
            set
            {
                if (System.Collections.Generic.Comparer<TValue>.Default.Compare(protectedValue, value) == 0) return; // Should use Equality instead of Compare?
                //if (value == ProtectedValue) return;
                this.MutatePersistenceState(() => HandleUtils.OnUserChangedValue_ReadWrite(this, value));
            }
        }

        #endregion

        /// <summary>
        /// Returns GetValue().Value if HasValue is true, otherwise uses InstantiateDefault() to populate Value and returns that.
        /// </summary>
        /// <returns>A guaranteed Value, that may have been preexisting, lazily loaded, or just instantiated.</returns>
        [ThreadSafe]
        public async Task<TValue> GetOrInstantiate()
        {
            var getResult = await GetValue().ConfigureAwait(false);
            if (getResult.HasValue) return getResult.Value;

            TrySetProtectedValueIfDefault(InstantiateDefault());

            return Value;

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

        Task<bool> IDeletable.Delete()
        {
            //MulticastDelegate
            throw new NotImplementedException();
        }
        void IDeletable.MarkDeleted() => throw new NotImplementedException();
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
