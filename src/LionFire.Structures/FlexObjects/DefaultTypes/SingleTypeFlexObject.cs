using System;

namespace LionFire.FlexObjects
{
    public interface ISingleTypeFlex
    {

    }

    /// <summary>
    /// Bringing a flex object back to being inflexible
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleTypeFlexObject<T> : IFlex, IFlexWithDefaultType<T>, ISingleTypeFlex
    {
        public SingleTypeFlexObject() { }
        public SingleTypeFlexObject(T value)
        {
            PrimaryValue = value;
        }

        public Type DefaultType => typeof(T);
        public T PrimaryValue { get; set; }
        public object FlexData { get => PrimaryValue; set => PrimaryValue = (T)value; }
    }
}
