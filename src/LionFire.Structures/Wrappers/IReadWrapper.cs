#nullable enable
using System;
using System.Data.SqlTypes;

namespace LionFire.Structures;

public interface IReadWrapper<out T>
{
    // REVIEW: Consider replacing T with language-ext's Option<>?
    T? Value { get; } 
    //event Action<IReadWrapper<T> /* Wrapper */ , T /*oldValue*/ , T /*newValue*/> ObjectChanged;
}
