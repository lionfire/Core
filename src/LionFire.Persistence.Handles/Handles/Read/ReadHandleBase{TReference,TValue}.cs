using System;
using LionFire.Referencing;
using LionFire.Resolves;

namespace LionFire.Persistence.Handles
{
    // TODO: Rebase on Resolves<TReference, T>?  Avoid IsAllowedReferenceType
#if false
    public abstract class ReadHandleBase2<TReference, TValue> : Resolves<TReference, TValue>, RH<TValue>, IReadHandleInvariant<TValue>
    {
#region Construction

        protected ReadHandleBase2() { }

        protected ReadHandleBase2(TReference input) : base(input) { }

#endregion

#region Reference

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

                reference = value;
            }
        }
        protected IReference reference;

#endregion

#region Persistence State

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
            }
        }
        private PersistenceState handleState;

#endregion
    }
#endif

    public abstract class ReadHandleBase<TReference, TValue> : ReadHandleBase<TValue>
        where TReference : IReference
        where TValue : class
    {
        protected override bool IsAllowedReferenceType(Type type) => type == typeof(TReference);

        // Skips the reference type check
        public new TReference Reference
        {
            get => (TReference)base.Reference;
            set
            {
                if (ReferenceEquals(reference, value)) { return; }
                if (reference != default(IReference)) { throw new AlreadySetException(); }
                reference = value;
            }
        }

#region Construction

        protected ReadHandleBase() { }

        protected ReadHandleBase(TReference reference) : base(reference) { }

        ///// <summary>
        ///// Do not use this in derived classes that are purely resolve-only and not intended to set an initial value.
        ///// </summary>
        ///// <param name="input"></param>
        ///// <param name="initialValue"></param>
        //protected ReadHandleBase(IReference input, TValue initialValue) : base(input, initialValue) { }

#endregion

    }
}
