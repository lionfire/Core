using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.ExtensionMethods
{
    public static class IServiceProviderExtensions
    {
        public static T GetRequiredService<T>(this IServiceProvider serviceProvider, Type serviceType) 
            => (T)serviceProvider.GetService(serviceType) 
            ?? throw new InvalidOperationException("There is no service of type serviceType.");
    }
}
