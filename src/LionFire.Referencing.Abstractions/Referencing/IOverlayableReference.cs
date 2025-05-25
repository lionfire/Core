using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{
    public enum ReferenceOverlayMode
    {
        None = 0,
        CanAddLeft = 1 << 0,
        CanAddRight = 1 << 1,

        /// <summary>
        /// If true, the left-most references provide defaults which following references can override
        /// </summary>
        AllowOverride = 1 << 2,
    }

    public interface IOverlayableReference<TReference> : IReferenceable<TReference>
        where TReference : IReference
    {
        ReferenceOverlayMode OverlayMode { get; }

        IReferenceable<TReference> AddLeft(IReference reference);
        IReferenceable<TReference> AddRight(IReference reference);
        IReferenceable<TReference> PopLeft(IReference reference);
        IReferenceable<TReference> PopRight(IReference reference);
        int Count { get; }
    }

}
