using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persisters.Handles
{

    //public class NoopReadWriteHandle2<TValue> : SingletonProviderReadWriteHandle<TValue, NoopHandlePersistenceProvider>
    //{
    //}

    //public abstract class SingletonProviderReadWriteHandle<TValue, THandlePersistenceProvider>
    //    where THandlePersistenceProvider : class, IHandlePersistenceProvider
    //{
    //    public override IHandlePersistenceProvider PersistenceProvider => ManualSingleton<THandlePersistenceProvider>.GuaranteedInstance;
    //}
    //public abstract class ProviderInstanceReadWriteHandle<TValue, THandlePersistenceProvider>
    //    where THandlePersistenceProvider : class, IHandlePersistenceProvider
    //{
    //    public override IHandlePersistenceProvider PersistenceProvider { get; set; }
    //}


    //public class NoopReadWriteHandle<TValue> : ReadWriteHandle<TValue>
    //{
    //    //public override Task<IResolveResult<TValue>> ResolveImpl() => Task.FromResult((IResolveResult<TValue>)NoopFailResolveResult<TValue>.Instance);

    //    //public override Task<IPutResult> Put(TValue value) => Task.FromResult((IPutResult)NoopFailPutResult<TValue>.Instance);
    //    //protected override Task<IDeleteResult> DeleteImpl() => Task.FromResult((IDeleteResult)NoopFailDeleteResult<TValue>.Instance);

    //    //#region Implement all this in base class?

    //    //public override PersistenceSnapshot<TValue> PersistenceState => throw new NotImplementedException();

    //    //public override object PersistenceLock => throw new NotImplementedException();

    //    //public override event Action<PersistenceEvent<TValue>> PersistenceStateChanged;

    //    //public override ILazyResolveResult<TValue> QueryValue() => throw new NotImplementedException();
    //    //public override void RaisePersistenceEvent(PersistenceEvent<TValue> ev) => throw new NotImplementedException();

    //    //#endregion

    //    //public override Task<IPersistenceResult> UpsertImpl() => NoopPutPersistenceResult.Instance;
    //}

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
