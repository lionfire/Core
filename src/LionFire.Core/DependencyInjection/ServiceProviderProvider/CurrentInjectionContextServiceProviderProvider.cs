using System;

namespace LionFire.DependencyInjection
{
    public class CurrentDependencyContextServiceProviderProvider : IServiceProviderProvider
    {
        public IServiceProvider GetServiceProvider(object obj) => DependencyContext.Current.ServiceProvider;
    }


}
