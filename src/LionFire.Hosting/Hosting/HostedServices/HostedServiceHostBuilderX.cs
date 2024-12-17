using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace LionFire.Hosting;

public static class HostedServiceHostBuilderX
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

    #region DynamicHostedService helpers

    public static IServiceCollection OnStart(this IServiceCollection services, Func<IServiceProvider, CancellationToken, ValueTask> action)
        => services.AddHostedService(sp => new DynamicHostedService(sp)
        {
            OnStart = action,
        });

    public static ILionFireHostBuilder OnStart(this ILionFireHostBuilder lionFireHostBuilder, Func<IServiceProvider, CancellationToken, ValueTask> action)
        => lionFireHostBuilder.ConfigureServices(s => s.OnStart(action));
    public static ILionFireHostBuilder OnStart(this ILionFireHostBuilder lionFireHostBuilder, Action<IServiceProvider> action)
        => lionFireHostBuilder.ConfigureServices(s => s.OnStart((sp, ct) => { action(sp); return ValueTask.CompletedTask; }));

    #endregion

}
