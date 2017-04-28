using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    /// <summary>
    /// If the reference can change, expose IChangeableReference.
    /// </summary>
    public interface IReferencable
    {
        IReference Reference { get; }
    }

    public interface IChangeableReferencable : IReferencable
    {
        new IReference Reference { get; set; }
        event Action<IChangeableReferencable, IReference> ReferenceChangedForFrom;
    }

    
}
