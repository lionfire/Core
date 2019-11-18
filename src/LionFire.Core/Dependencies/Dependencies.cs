using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Dependencies;

namespace LionFire
{
    // FUTURE: Add interface here for default implementation Type and trickle it through to ManualSingleton.GetGuaranteedInsance<CreateType>()
    public static class DependenciesConfig
    {
        private const bool CreateIfMissingDefault = false;

        public static object GetService(Type serviceType, IServiceProvider serviceProvider = null) => DependencyContext.Current.GetService(serviceType, serviceProvider);
        public static T GetService<T>(IServiceProvider serviceProvider = null) => (T)DependencyContext.Current.GetService(typeof(T), serviceProvider);

        //public static object GetServiceOrSingleton(Type serviceType, IServiceProvider serviceProvider = null, bool createIfMissing = CreateIfMissingDefault) 
        //    => DependencyContext.Current.GetServiceOrSingleton(serviceType, serviceProvider, createIfMissing);
        //public static T GetServiceOrSingleton<T>(IServiceProvider serviceProvider = null, bool createIfMissing = CreateIfMissingDefault) 
        //    => (T)DependencyContext.Current.GetServiceOrSingleton(typeof(T), serviceProvider, createIfMissing);
    }
}
