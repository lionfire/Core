using System;

namespace LionFire.DependencyInjection
{
    public class DefaultDependencyContextServiceProviderProvider : IServiceProviderProvider
    {
        public IServiceProvider GetServiceProvider(object obj) => DependencyContext.Default.ServiceProvider;
    }


}
