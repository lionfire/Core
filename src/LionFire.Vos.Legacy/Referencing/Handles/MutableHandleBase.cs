#if UNUSED // Not Recommended
using System;

namespace LionFire.ObjectBus
{

    /// <summary>
    /// Derive from this to implement Handles that can have a Reference that can be changed (not usually recommended)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MutableHandleBase<T> : HandleBase<T>
            , IChangeableReferencable
            , IFreezable
            where T : class
    {
#region Reference

        protected void OnReferenceChangedFrom(IReference oldReference)
        {
            ReferenceChangedForFrom?.Invoke(this, oldReference);
        }

        public event Action<IChangeableReferencable, IReference> ReferenceChangedForFrom;

#endregion

#region IFreezable

        public bool IsFrozen
        {
            get
            {
                return isFrozen;
            }
            set
            {
                if (isFrozen == value)
                {
                    return;
                }

                if (isFrozen && !value) { throw new NotSupportedException("Unfreeze not supported"); }

                isFrozen = value;
            }
        }
        private bool isFrozen;

#endregion
    }
}

#endif