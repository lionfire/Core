using System;

namespace LionFire.Dependencies
{
    public class CurrentDependencyContextServiceProviderProvider : IServiceProviderProvider
    {
        public IServiceProvider GetServiceProvider(object obj) => DependencyContext.Current.ServiceProvider;
    }


}
