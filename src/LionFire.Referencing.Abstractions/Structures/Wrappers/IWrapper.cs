using System;

namespace LionFire.Structures
{
    public interface IWrapper<T> : IReadWrapper<T>, IWriteWrapper<T>
    {
        new T Value { get; set; }
    }

    //public interface INotifyingWrapper<out T>
    //{
    //    event Action<INotifyingWrapper<T>, T /*oldValue*/ , T /*newValue*/> ValueChanged;
    //}
}
