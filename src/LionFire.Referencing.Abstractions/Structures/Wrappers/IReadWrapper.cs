using System;

namespace LionFire.Structures
{
    public interface IReadWrapper<out T> 
    {
        T Object { get; }
        //event Action<IReadWrapper<T> /* Wrapper */ , T /*oldValue*/ , T /*newValue*/> ObjectChanged;
    }
}
