using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Referencing
{
    // If inheriting this, also consider inheriting from IReferencable
    public interface IReferencable<out TReference> : IReferencable // REVIEW - IReferencable?  Helps for Save().  Having it separate results in ambiguous resolution of Reference.
        where TReference : IReference
    {
        new TReference Reference { get; }
    }
    
}
