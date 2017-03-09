using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.MultiTyping;
using LionFire.Structures;

namespace LionFire.Instantiating
{

    
    public static class ObjectInstantiationExtensions
    {
        /// <summary>
        /// Uses strategies in (context.AsType<InstantiationStrategyContext>() ?? InstantiationStrategyContext.Default) to create an IInstantiator
        /// </summary>
        public static IInstantiator ToInstantiator(this object obj, InstantiationContext context = null)
        {
            if (obj == null) return null;

            if (context == null)
            {
                context = new InstantiationContext();
            }

            var instantiationStrategyContext = context.AsType<InstantiationStrategyContext>() ?? InstantiationStrategyContext.Default;

            foreach (var strategy in instantiationStrategyContext.Strategies.Values)
            {
                var result = strategy.TryProvide(obj, context);
                if (result != null) return result;
            }

            return null;
        }
        public static object ToInstantiatorOrObject(this object obj, InstantiationContext context = null)
        {
            return ToInstantiator(obj,context) ?? obj;
        }
    }
}
