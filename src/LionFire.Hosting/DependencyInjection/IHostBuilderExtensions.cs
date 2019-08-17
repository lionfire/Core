using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting
{
    public static class IHostBuilderExtensions
    {
        public static IHostBuilder AddSingleton<TImplementation>(this IHostBuilder hostBuilder)
            where TImplementation : class
        {
            hostBuilder.ConfigureServices((c, sc) => sc.AddSingleton<TImplementation>());
            return hostBuilder;
        }

        public static IHostBuilder AddHostedService<TImplementation>(this IHostBuilder hostBuilder)
            where TImplementation : class, IHostedService
        {
            hostBuilder.ConfigureServices((c, sc) => sc.AddHostedService<TImplementation>());
            return hostBuilder;
        }

        public static IHostBuilder AddSingleton<TService, TImplementation>(this IHostBuilder hostBuilder)
            where TService : class
            where TImplementation : class, TService
        {
            hostBuilder.ConfigureServices((c, sc) => sc.AddSingleton<TService, TImplementation>());
            return hostBuilder;
        }
    }
}
