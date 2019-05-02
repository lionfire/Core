using System;

namespace LionFire.DependencyInjection
{
    public class CurrentInjectionContextServiceProviderProvider : IServiceProviderProvider
    {
        public IServiceProvider GetServiceProvider(object obj) => InjectionContext.Current.ServiceProvider;
    }


}
