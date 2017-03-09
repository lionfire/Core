using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public class SerializerInstantiator : IInstantiator
    {
        public object Object { get; set; }
        public object Affect(object obj, InstantiationContext context = null)
        {
            return Object;
        }
    }

    public class SerializeObjectStrategy : IInstantiationProvider
    {
        public IInstantiator TryProvide(object instance, InstantiationContext context = null)
        {
            return new SerializerInstantiator { Object = instance };
        }
    }

}
