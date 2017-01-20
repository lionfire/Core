using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public interface IToInstantiator
    {
        IInstantiator ToInstantiator(InstantiationContext context = null);
    }
    public class IToInstantiatorStrategy : IInstantiationProvider
    {
        public IInstantiator TryProvide(object instance, InstantiationContext context = null)
        {
            return (instance as IToInstantiator)?.ToInstantiator(context);
        }
    }
}
