using System;

namespace LionFire.Referencing;

// RENAME this interface to ReferenceWithType?  This is for References that don't internally support Types

/// <summary>
/// A Reference augmented with a Type
/// </summary>
/// <typeparam name="TValue"></typeparam>
public interface ITypedReference<TValue>
{
    Type Type { get; }
    IReference Reference { get; }
}

/// <summary>
/// A Reference augmented with a Type
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TReference"></typeparam>
public interface ITypedReference<TValue, TReference>
    where TReference : IReference
{
    // REVIEW and better document this interface:
    //  - shouldn't TReference be IReference<TValue>?  Or would IReference<TValue> simply be used instead of this if that were possible?
    // - shouldn't this inherit from IReference<TValue>?  Or would IReference<TValue> simply be used instead of this if that were possible?

    Type Type { get; }
    TReference Reference { get; }
}
