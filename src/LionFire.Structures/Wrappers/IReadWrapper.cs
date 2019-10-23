using System;

namespace LionFire.Structures
{
    public interface IReadWrapper<out T>
    {
        T Value { get; } // RENAME to Value
        //event Action<IReadWrapper<T> /* Wrapper */ , T /*oldValue*/ , T /*newValue*/> ObjectChanged;
    }
}
