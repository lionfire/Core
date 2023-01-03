using LionFire.Dependencies;
using LionFire.FlexObjects;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LionFire.Hosting
{
    public static class IFlexDependencyContextExtensions
    {
        /// <summary>
        /// (Process-wide impact)
        /// </summary>
        /// <param name="lf"></param>
        /// <returns></returns>
        public static ILionFireHostBuilder UseDependencyContextToActivateFlexChildren(this ILionFireHostBuilder lf)
        {
            UseDependencyContextToActivateFlexChildren();
            return lf;
        }

        public static void UseDependencyContextToActivateFlexChildren()
        {
            FlexGlobalOptions.DefaultCreateFactory = type => ActivatorUtilities.CreateInstance(DependencyContext.Current.ServiceProvider, type);

            FlexGlobalOptions.DefaultCreateWithOptionsFactory = (type, args) => ActivatorUtilities.CreateInstance(DependencyContext.Current.ServiceProvider, type, args);
        }
    }

}
