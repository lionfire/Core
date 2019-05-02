using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.DependencyInjection
{

    public static class ServiceProviderProvider
    {
        public static IServiceProvider GetServiceProvider(this object obj) => Provider.GetServiceProvider(obj);
        public static IServiceProviderProvider Provider { get; set; } = new CurrentInjectionContextServiceProviderProvider();
    }


}
