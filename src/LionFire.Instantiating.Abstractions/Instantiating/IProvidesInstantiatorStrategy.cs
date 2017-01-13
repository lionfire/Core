using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public class IProvidesInstantiatorStrategy : IInstantiationProvider
    {
        public IInstantiator TryProvide(object instance, InstantiationContext context = null)
        {
            var ipi = instance as IProvidesInstantiator;
            if (ipi != null) return ObjectInstantiationExtensions.ToInstantiator(ipi, context);

            return null;
        }
    }
}
