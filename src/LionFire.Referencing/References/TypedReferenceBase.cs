using System;

namespace LionFire.Referencing
{
    public abstract class TypedReferenceBase<ConcreteType> : ReferenceBase<ConcreteType>
        where ConcreteType : ReferenceBase<ConcreteType>
    {

        #region Type

        [SetOnce]
        public Type Type
        {
            get => type;
            set
            {
                if (type == value) return;
                if (type != default) throw new AlreadySetException();
                type = value;
            }
        }
        private Type type;

        #endregion
    }
}
