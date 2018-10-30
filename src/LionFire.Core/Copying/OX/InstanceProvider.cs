using System;
using System.Collections.Generic;

namespace OX.Copyable
{
    /// <summary>
    /// Abstract class that implements the <see cref="IInstanceProvider"/> interface,
    /// and can be used as a base class for an instance provider. The class simplifies
    /// implementation by partially implementing the interface, leaving the implementation
    /// of the <see cref="CreateTypedInstance"/> method to the concrete subclass.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class InstanceProvider<T> : IInstanceProvider<T>
    {
        #region IInstanceProvider Members

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

        public object CreateInstance(object toBeCopied)
        {
            return CreateTypedInstance((T)toBeCopied);
        }

        public abstract T CreateTypedInstance(T toBeCopied);

        #endregion
    }
}
