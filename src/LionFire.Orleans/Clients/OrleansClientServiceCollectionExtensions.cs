using LionFire.Orleans_.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;

namespace LionFire.Hosting;

public static class OrleansClientServiceCollectionExtensions
{
    public static IServiceCollection AddOrleansClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ClusterClientHostedService>();
        services.AddHostedService<ClusterClientHostedService>(sp => sp.GetRequiredService<ClusterClientHostedService>());
        services.AddSingleton<IClusterClient>(sp => sp.GetRequiredService<ClusterClientHostedService>().Client);
        services.AddSingleton<IGrainFactory>(sp => sp.GetRequiredService<ClusterClientHostedService>().Client);

        services.Configure<OrleansClusterConfig>(configuration.GetSection("Orleans:Cluster"));
        services.Configure<OrleansConsulClusterConfig>(configuration.GetSection("Orleans:Cluster:Consul"));

        return services;
    }
}
