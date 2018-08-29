using System;

namespace LionFire.Referencing
{
    public interface IChangeableReferencable : IReferencable // An idea: RENAME to pointer?
    {
        new IReference Reference { get; set; }
        event Action<IChangeableReferencable, IReference> ReferenceChangedForFrom;
    }
}
