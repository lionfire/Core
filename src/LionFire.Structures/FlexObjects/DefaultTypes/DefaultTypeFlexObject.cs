using System;

namespace LionFire.FlexObjects
{
    public class DefaultTypeFlexObject<T> : FlexObject, IFlexWithDefaultType<T>
    {
        public Type DefaultType => typeof(T);
        public T PrimaryValue { get; set; }
    }
}
