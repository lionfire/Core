using System;
using LionFire.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting
{
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
    public static class DependencyContextWrapperExtensions
    {
        public static IHostBuilder UseDependencyContext(this IHostBuilder hostBuilder)
        {
            //hostBuilder.UseServiceProviderFactory(new DefaultServiceProviderFactory());
            hostBuilder.UseServiceProviderFactory(new DependencyContextServiceProviderFactoryWrapper(new DefaultServiceProviderFactory()));

            //new ServiceFactoryAdapter<IServiceCollection>(new DefaultServiceProviderFactory());
            return hostBuilder;
        }
    }
}
