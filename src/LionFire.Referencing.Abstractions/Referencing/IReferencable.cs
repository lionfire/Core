using System;

namespace LionFire.Referencing
{
    /// <summary>
    /// If the reference can change, expose IChangeableReference.
    /// </summary>
    public interface IReferencable
    {
        IReference Reference { get; }
    }

    public interface IReferencableValueType : IReferencable
    {
        Type ReferenceValueType { get; }

    }
}
