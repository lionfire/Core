using LionFire.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace LionFire.Instantiating
{
    public class InstantiatorsFromAttributesStrategy : IInstantiationProvider
    {
        public IInstantiator TryProvide(object instance, InstantiationContext context = null)
        {
            var pipe = new InstantiationPipeline();

            {
                var attr = instance.GetType().GetTypeInfo().GetCustomAttribute<InstantiatorTypeAttribute>();
                if (attr != null)
                {
                    var instantiator = (IInstantiator)Activator.CreateInstance(attr.Type, instance);
                    pipe.Add(instantiator);
                }
            }

            {
                var attr = instance.GetType().GetTypeInfo().GetCustomAttribute<StateAttribute>();
                if (attr != null)
                {
                    pipe.Add(new StateRestorer(instance));
                }
            }

            if (pipe.Count > 0)
            {
                return pipe;
            }
            else
            {
                return null;
            }
        }
    }
}
