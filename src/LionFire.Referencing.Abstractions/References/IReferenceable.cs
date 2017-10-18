using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus // MOVE to References
{
    /// <summary>
    /// If the reference can change, expose IChangeableReference.
    /// </summary>
    public interface IReferencable
    {
        IReference Reference { get; }
    }
}
