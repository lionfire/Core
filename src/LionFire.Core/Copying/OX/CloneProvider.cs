using System;
using System.Collections.Generic;

namespace LionFire
{
    /// <summary>
    /// Abstract class that implements the <see cref="ICloneProvider"/> interface,
    /// and can be used as a base class for an instance provider. The class simplifies
    /// implementation by partially implementing the interface, leaving the implementation
    /// of the <see cref="Clone"/> method to the concrete subclass.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CloneProvider<T> : ICloneProvider<T>
    {
        #region ICloneProvider Members

        public Type Provided
        {
            get { return typeof(T); }
        }
        public IEnumerable<Type> TypesSupported
        {
            get
            {
                yield return Provided;
            }
        }

        public object Clone(object toBeCopied)
        {
            return Clone((T)toBeCopied);
        }

        public abstract T Clone(T toBeCopied);

        #endregion
    }
}
