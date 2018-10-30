using System;

namespace LionFire.Referencing
{
    public interface IVReference : IReference
    {
        string Package { get; }
        string TypeName { get; }
        Type Type { get; }
    }
}
