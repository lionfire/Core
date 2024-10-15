#nullable enable
using System;
using System.Data.SqlTypes;

namespace LionFire.Structures;

// REVIEW ENH: non-null version for Value

public interface IReadWrapper<out T>
{
    // REVIEW: Consider replacing T with language-ext's Option<>?
    T? Value { get; }  // RENAME to Value, or something
    //event Action<IReadWrapper<T> /* Wrapper */ , T /*oldValue*/ , T /*newValue*/> ObjectChanged;
}
