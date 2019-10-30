using LionFire.Persistence;
using LionFire.Persistence.Implementation;
using LionFire.Referencing;
using LionFire.Resolves;
using LionFire.Structures;
using System;
using System.Threading.Tasks;

namespace LionFire.Persistence.Handles
{
    // REVIEW: Can/should I use ReadHandleBase, or a separate class?  I could write it from scratch and merge if it is the same


    public abstract class ReadWriteHandleBase<TValue> : Resolves<IReference, TValue>, RH<TValue>, W<TValue>
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

        public PersistenceState State
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

        private PersistenceState handleState;

        protected virtual void OnStateChanged(PersistenceState newValue, PersistenceState oldValue) { }
        
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


        public Task<IPutResult> Put() => throw new NotImplementedException();
        public Task<bool> Delete() => throw new NotImplementedException();
        public void MarkDeleted() => throw new NotImplementedException();

    }
}
