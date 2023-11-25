using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

namespace LionFire.Hosting;

public static class OrleansClientServiceCollectionExtensions
{
    public static IHostApplicationBuilder UseOrleansClient_LF(this IHostApplicationBuilder hostBuilder, string? clusterName = null)
    {
        //if (clusterName != null) throw new NotImplementedException("TODO: Support multiple clusters using .NET 8 Keyed Services, once Orleans supports it, or I implement it");

        hostBuilder.Services.AddOrleansClient(builder => {
            
            var configKey = clusterName == null ? "Orleans:Cluster" : "Orleans:Clusters:" + clusterName;

            builder.Configure<ClusterOptions>(hostBuilder.Configuration.GetSection(configKey));

            var ClusterConfig = new OrleansClusterConfig();
            hostBuilder.Configuration.GetSection(configKey).Bind(ClusterConfig);

            var ConsulClusterConfig = new OrleansConsulClusterConfig();
            hostBuilder.Configuration.GetSection(configKey + ":Consul").Bind(ConsulClusterConfig);

            switch (ClusterConfig.Kind)
            {
                case ClusterDiscovery.Unspecified:
                    throw new ArgumentException($"Missing Configuration: {configKey}:Kind");
                case ClusterDiscovery.Localhost:
                    builder.UseLocalhostClustering();
                    break;
                case ClusterDiscovery.Consul:
                    builder.UseConsulClientClustering(options =>
                    {
                        options.ConfigureConsulClient(new Uri(ConsulClusterConfig.ServiceDiscoverEndPoint ?? throw new ArgumentNullException("Missing config: Orleans:Cluster:Consul:ServiceDiscoverEndPoint")), ConsulClusterConfig.ServiceDiscoveryToken);
                        options.KvRootFolder = ConsulClusterConfig.KvFolderName ?? ClusterConfig.ServiceId;
                    });
                    break;
                case ClusterDiscovery.None: // FUTURE - support disabling the client here? For now fall through to default which is invalid.  
                default:
                    throw new NotSupportedException($"Unsupported Configuration value for: {configKey}:Kind: {ClusterConfig.Kind}");
            }
        });

        return hostBuilder;
    }
    //public static IServiceCollection AddOrleansClient(this IServiceCollection services, IConfiguration configuration)
    //{
    //    services
    //        .Configure<ClusterOptions>(o =>
    //        {
    //            o.ClusterId = ClusterConfig.ClusterId;
    //            o.ServiceId = ClusterConfig.ServiceId;
    //        })
    //    //    .Configure<OrleansClusterConfig>(configuration.GetSection("Orleans:Cluster"))
    //    //    .Configure<OrleansConsulClusterConfig>(configuration.GetSection("Orleans:Cluster:Consul"))
    //    //;
    //    // OLD - Orleans 3
    //    //.AddSingleton<ClusterClientHostedService>()
    //    //.AddHostedService<ClusterClientHostedService>(sp => sp.GetRequiredService<ClusterClientHostedService>())
    //    //.AddSingleton<IClusterClient>(sp => sp.GetRequiredService<ClusterClientHostedService>().Client);
    //    //.AddSingleton<IGrainFactory>(sp => sp.GetRequiredService<ClusterClientHostedService>().Client);
    //    //;


    //    return services;
    //}
}
