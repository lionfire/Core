using System;

namespace LionFire.Referencing
{
    public class H : HDynamicBase<object>
    {
        public override string Key { get => Reference?.Key; set => throw new NotImplementedException("TODO: Create an IReference from Key, perhaps via a UrlReference class "); }


        #region Reference

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
