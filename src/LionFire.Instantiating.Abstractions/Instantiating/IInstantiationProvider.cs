using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public interface IInstantiationProvider
    {
        IInstantiator TryProvide(object instance, InstantiationContext context = null);
    }
    
}
