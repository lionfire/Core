using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Referencing
{
    /// <summary>
    /// If the reference can change, expose IChangeableReference.
    /// </summary>
    public interface IReferencable
    {
        IReference Reference { get; }
    }

    public interface IReferencable<TReference>
        where TReference : IReference
    {
        TReference Reference { get; }
    }
}
