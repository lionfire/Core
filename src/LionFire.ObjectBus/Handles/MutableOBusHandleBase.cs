#if UNUSED // Not Recommended
using System;

namespace LionFire.ObjectBus
{
    /// <summary>
    /// Reference can be changed (not usually recommended)
    /// </summary>
    /// <typeparam name="ObjectType"></typeparam>
    public abstract class MutableOBusHandleBase<ObjectType> : MutableHandleBase<ObjectType>
        where ObjectType : class
    {
        #region Reference

        public override IReference Reference
        {
            get { return reference; }
            set
            {
                if (reference == value) return;
                if (IsFrozen)
                {
                    if (reference != default(IReference)) throw new NotSupportedException("IsFrozen == true.  Reference can only be set once.");
                }

                if (!reference.IsValid())
                {
                    throw new ArgumentException("Reference is invalid");
                }

#if DEBUG
                //if (value as VosReference != null) throw new InvalidOperationException("vh not valid here");
#endif

                var oldReference = reference;

                if (value != null)
                {
                    if (value.Type == null)
                    {
                        //IChangeableReferenceable cr = reference as IChangeableReferenceable;
                        //if (cr != null)
                        //{
                        //    cr.Type = typeof(T);
                        //}
                    }
                    else
                    {
                        if (value.Type != typeof(T))
                        {
                            if (!typeof(T).IsAssignableFrom(value.Type))
                            {
                                throw new ArgumentException("!typeof(T).IsAssignableFrom(value.Type)");
                            }
                        }
                    }
                }
                reference = value;
                OnReferenceChangedFrom(oldReference);
            }
        }
        private IReference reference;

        #endregion
    }
}
#endif