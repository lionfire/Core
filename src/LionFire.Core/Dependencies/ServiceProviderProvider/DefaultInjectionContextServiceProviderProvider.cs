using System;

namespace LionFire.Dependencies
{
    public class DefaultDependencyContextServiceProviderProvider : IServiceProviderProvider
    {
        public IServiceProvider GetServiceProvider(object obj) => DependencyContext.Default.ServiceProvider;
    }


}
