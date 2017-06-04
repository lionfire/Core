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
        private const bool CreateIfMissingDefault = false;

        public static object GetService(Type serviceType, IServiceProvider serviceProvider = null, bool createIfMissing = CreateIfMissingDefault)
        {
            return InjectionContext.Current.GetService(serviceType, serviceProvider, createIfMissing);
        }
        public static T GetService<T>(IServiceProvider serviceProvider = null, bool createIfMissing = CreateIfMissingDefault)
        {
            return (T)InjectionContext.Current.GetService(typeof(T), serviceProvider, createIfMissing);
        }
    }
}
