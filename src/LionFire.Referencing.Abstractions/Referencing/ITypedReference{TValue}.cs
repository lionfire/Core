using System;

namespace LionFire.Referencing
{
    public interface ITypedReference<TValue>
    {
        Type Type { get; }
        IReference Reference { get; }
    }
    public interface ITypedReference<TValue, TReference>
        where TReference : IReference
    {
        Type Type { get; }
        TReference Reference { get; }
    }
}
