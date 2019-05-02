using System;

namespace LionFire.DependencyInjection
{
    public class DefaultInjectionContextServiceProviderProvider : IServiceProviderProvider
    {
        public IServiceProvider GetServiceProvider(object obj) => InjectionContext.Default.ServiceProvider;
    }


}
