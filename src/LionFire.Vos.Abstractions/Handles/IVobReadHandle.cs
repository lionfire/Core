using LionFire.Persistence;
using LionFire.Referencing;
using System;

namespace LionFire.Vos
{
    public interface IVobReadHandle : IVobReadHandle<object> // RENAME to VR?
    {
    }
    public interface IVobReadHandle<out T> : IReadHandleBase<T>, IReferencable<IVosReference> // RENAME to VR?
    {
        IVob Vob { get; }
        Type Type { get; } // REVIEW
    }
}
