using System;

namespace LionFire.FlexObjects
{
    public interface IFlexWithDefaultType
    {
        Type DefaultType { get; }
    }
    public interface IFlexWithDefaultType<T> : IFlexWithDefaultType
    {
        T PrimaryValue { get; set; }
    }
}
