using System;

namespace LionFire.Referencing
{
    /// <summary>
    /// If the reference can change, expose IChangeableReference.
    /// </summary>
    public interface IReferenceable
    {
        IReference Reference { get; }
    }

    public interface IReferenceableValueType : IReferenceable
    {
        Type ReferenceValueType { get; }

    }
}
