using System;

namespace LionFire.FlexObjects
{
    /// <summary>
    /// Represents a value of unspecified type that is known as specified type T.  This is used to register values as a base class or interface rather than their implementation type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TypedObject<T> : ITypedObject
    {
        public Type Type => typeof(T);
        public T Object { get; set; }

        object ITypedObject.Object => Object;
    }

}
