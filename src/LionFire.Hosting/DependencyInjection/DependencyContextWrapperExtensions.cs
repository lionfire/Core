using LionFire.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting
{
    public static class DependencyContextWrapperExtensions
    {
        public static IHostBuilder UseDependencyContext(this IHostBuilder hostBuilder, bool useAsGuaranteedSingletonProvider = true, bool allowMultipleRootServiceProviders = false)
        {
            if (useAsGuaranteedSingletonProvider)
            {
                DependencyContext.Default.UseAsGuaranteedSingletonProvider();
            }
            
            hostBuilder.RegisterServiceProviderWithDependencyContext(allowMultipleRootServiceProviders);

            return hostBuilder;
        }

        public static IHostBuilder RegisterServiceProviderWithDependencyContext(this IHostBuilder hostBuilder, bool allowMultipleRootServiceProviders = false)
            => hostBuilder.UseServiceProviderFactory(new DependencyContextServiceProviderFactoryWrapper(new DefaultServiceProviderFactory(), allowMultipleRootServiceProviders));
    }
}
