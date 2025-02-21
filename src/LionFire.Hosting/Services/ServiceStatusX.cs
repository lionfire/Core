using LionFire.Hosting.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Hosting;

public static class ServiceStatusX
{
    public static IServiceCollection AddServiceStatus(this IServiceCollection services)
        => services.AddSingleton<ServiceTracker>();
}
