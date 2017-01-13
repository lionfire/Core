using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Instantiating
{
    public interface IInstantiator : IAffector<InstantiationContext> { }

    public static class IInstantiatorExtensions
    {
        public static object Instantiate(this IInstantiator instantiator, InstantiationContext context = null)
        {
            if (context == null) context = new InstantiationContext();
            return instantiator.Affect(null, context);
        }
        public static T Instantiate<T>(this IInstantiator instantiator, InstantiationContext context = null)
        {
            return (T)Instantiate(instantiator, context);
        }
    }
}
