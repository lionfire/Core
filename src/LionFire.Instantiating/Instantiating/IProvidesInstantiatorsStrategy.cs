using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public class IProvidesInstantiatorsStrategy : IInstantiationProvider
    {
        public IInstantiator TryProvide(object instance, InstantiationContext context = null)
        {
            var ipip = instance as IProvidesInstantiators;
            if (ipip?.Instantiators == null) return null;

            var pipeline = new InstantiationPipeline();

            foreach (var childProvider in ipip.Instantiators)
            {
                var inst = childProvider.ToInstantiator(context);
                if (inst != null)
                {
                    pipeline.Add(inst);
                }
            }

            return pipeline;
        }
    }
}
