using LionFire.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting
{
    public static class DependencyContextWrapperExtensions
    {
        public static IHostBuilder UseDependencyContext(this IHostBuilder hostBuilder, bool useAsGuaranteedSingletonProvider = true)
        {
            if (useAsGuaranteedSingletonProvider)
            {
                DependencyContext.Default.UseAsGuaranteedSingletonProvider();
            }

            hostBuilder.UseServiceProviderFactory(new DependencyContextServiceProviderFactoryWrapper(new DefaultServiceProviderFactory()));

            return hostBuilder;
        }
    }
}
