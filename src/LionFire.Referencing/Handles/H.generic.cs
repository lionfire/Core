using System;

namespace LionFire.Referencing
{
    public class H<ObjectType> : HDynamicBase<ObjectType>, IH<ObjectType>
        where ObjectType : class
    {
        #region Reference

        #region Key

        public override string Key
        {
            get => Reference?.ToString();
            set => throw new NotSupportedException("Set Reference instead");
        }

        #endregion

        [SetOnce]
        public override IReference Reference
        {
            get { return reference; }
            set
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
        private IReference reference;

        #endregion

    }
}
