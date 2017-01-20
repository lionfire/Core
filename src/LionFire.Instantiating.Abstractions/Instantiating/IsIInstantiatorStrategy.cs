using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public class IsIInstantiatorStrategy : IInstantiationProvider
    {
        public IInstantiator TryProvide(object instance, InstantiationContext context = null)
        {
            var ii = instance as IInstantiator;
            if (ii != null) return ii;
            return null;
        }
    }
}
