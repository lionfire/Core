using System;
using LionFire.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting
{
    /// <summary>
    /// Grabs the ServiceProvider from the wrapped IServiceProviderFactory and stores it in DependencyContext.Current.ServiceProvider
    /// </summary>
    public class DependencyContextServiceProviderFactoryWrapper : IServiceProviderFactory<IServiceCollection>
    {
        IServiceProviderFactory<IServiceCollection> child;

        public DependencyContextServiceProviderFactoryWrapper(IServiceProviderFactory<IServiceCollection> child)
        {
            this.child = child;
        }

        public IServiceCollection CreateBuilder(IServiceCollection services) => child.CreateBuilder(services);
        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            var result = child.CreateServiceProvider(containerBuilder);
            DependencyContext.Current.ServiceProvider = result;
            return result;
        }
    }
}
