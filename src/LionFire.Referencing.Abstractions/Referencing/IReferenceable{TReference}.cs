using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Referencing
{
    // If inheriting this, also consider inheriting from IReferencable
    public interface IReferencable<out TReference>
        where TReference : IReference
    {
        TReference Reference { get; }
    }
    
}
