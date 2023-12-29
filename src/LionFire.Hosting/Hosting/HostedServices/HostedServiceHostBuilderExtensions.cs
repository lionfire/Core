using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

public static class HostedServiceHostBuilderExtensions
{
    public static IHostBuilder UseHostedService<T>(this IHostBuilder hostBuilder)
        where T : class, IHostedService
    {
        return hostBuilder.ConfigureServices((context, services) =>
            services.AddHostedService<T>());
    }
    public static IHostApplicationBuilder UseHostedService<T>(this IHostApplicationBuilder hostApplicationBuilder)
        where T : class, IHostedService
    {
        hostApplicationBuilder.Services.AddHostedService<T>();
        return hostApplicationBuilder;
    }
}
