using System;

namespace LionFire.ObjectBus // MOVE to References
{
    public interface IChangeableReferencable : IReferencable // An idea: RENAME to pointer?
    {
        new IReference Reference { get; set; }
        event Action<IChangeableReferencable, IReference> ReferenceChangedForFrom;
    }
}
