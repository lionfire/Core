using System;

namespace LionFire.Referencing;

public interface IChangeableReferenceable : IReferenceable // An idea: RENAME to pointer?
{
    new IReference Reference { get; set; }
    event Action<IChangeableReferenceable, IReference> ReferenceChangedForFrom;
}
