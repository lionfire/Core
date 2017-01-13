using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.DependencyInjection;

namespace LionFire
{
    // FUTURE: Add interface here for default implementation Type and trickle it through to ManualSingleton.GetGuaranteedInsance<CreateType>()
    public static class Injection
    {
        public static object GetService(Type serviceType, IServiceProvider serviceProvider = null)
        {
            return InjectionContext.Current.GetService(serviceType, serviceProvider);
        }
        public static T GetService<T>(IServiceProvider serviceProvider = null)
        {
            return (T)InjectionContext.Current.GetService(typeof(T), serviceProvider);
        }
    }
}
