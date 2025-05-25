using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Referencing
{
    // If inheriting this, also consider inheriting from IReferenceable
    public interface IReferenceable<out TReference> : IReferenceable // REVIEW - IReferenceable?  Helps for Save().  Having it separate results in ambiguous resolution of Reference.
        where TReference : IReference
    {
        new TReference Reference { get; }
    }
}
