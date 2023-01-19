using LionFire.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting;

public static class DependencyContextWrapperExtensions
{
    public static IHostBuilder UseDependencyContext(this IHostBuilder hostBuilder, bool useAsGuaranteedSingletonProvider = true, bool? multiHostEnvironment = null)
    {
        hostBuilder.RegisterServiceProviderWithDependencyContext(multiHostEnvironment: multiHostEnvironment, useAsGuaranteedSingletonProvider: useAsGuaranteedSingletonProvider);

        return hostBuilder;
    }

    public static IHostBuilder RegisterServiceProviderWithDependencyContext(this IHostBuilder hostBuilder, bool? multiHostEnvironment = null, bool useAsGuaranteedSingletonProvider = true)
        => hostBuilder.UseServiceProviderFactory(new CaptureServiceProviderIntoContextAndWrapFactory(new DefaultServiceProviderFactory(), multiHostEnvironment: multiHostEnvironment, useAsGuaranteedSingletonProvider));
}
