using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace LionFire.Hosting;

public static class OrleansClientServiceCollectionExtensions
{
    public static IHostBuilder UseOrleansClient_LF(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        //hostBuilder.Cont

        return hostBuilder
            //.ConfigureServices(services =>
            //    services
            //        .Configure<ConsulClientConfiguration>(c =>
            //        {
            //            c.Address = ;
            //            c.HttpAuth = ;
            //        })

            //    )
            .UseOrleansClient((context, builder) =>
            {
                builder
                    .Configure<ClusterOptions>(context.Configuration.GetSection("Orleans:Cluster"))
                ;

                var ClusterConfig = new OrleansClusterConfig();
                context.Configuration.GetSection("Orleans:Cluster").Bind(ClusterConfig);

                var ConsulClusterConfig = new OrleansConsulClusterConfig();
                context.Configuration.GetSection("Orleans:Cluster:Consul").Bind(ConsulClusterConfig);

                switch (ClusterConfig.Kind)
                {
                    case ClusterDiscovery.Unspecified:
                        break;
                    case ClusterDiscovery.None:
                        break;
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
                    default:
                        break;
                }
            });
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
