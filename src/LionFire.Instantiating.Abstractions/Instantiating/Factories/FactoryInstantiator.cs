using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public class FactoryInstantiator : IInstantiator
    {
        public IInstantiationFactory Factory { get; set; }

        public object Affect(object obj, InstantiationContext context = null)
        {
#if SanityChecks
            if(obj!=null)
            {
                throw new ArgumentException("obj must be null for FactoryInstantiator.Affect");
            }
#endif
            return Factory.Create(context);
        }
    }
}
