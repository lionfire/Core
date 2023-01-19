
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Statistics;
using Orleans.Runtime.Host;
using LionFire.Orleans_.AspNetCore_;
using System.Net;
using LionFire.Consul_;
using Consul;
using System.Collections.Generic;
using LionFire.Orleans_;
using Microsoft.Extensions.Configuration;
using LionFire.Deployment;
using Polly;
using HostBuilderContext = Microsoft.Extensions.Hosting.HostBuilderContext;

namespace LionFire.Hosting;


public static class SiloHostBuilder
{

    public static ILionFireHostBuilder Silo(this ILionFireHostBuilder lf, int port, Action<HostBuilderContext, ISiloBuilder>? configureSilo = null)
        => lf.Silo(new SiloProgramConfig(port), configureSilo);

    public static ILionFireHostBuilder Silo(this ILionFireHostBuilder lf, SiloProgramConfig config = null, Action<HostBuilderContext, ISiloBuilder>? configureSilo = null)
    {
        config ??= new();

        lf.HostBuilder
            .ConfigureHostConfiguration(b =>
            {
                if (config.HttpPort.HasValue)
                {
                    b.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Kestrel:Endpoints:Http:Url"] = $"http://{config.HttpInterface}:{config.HttpPort}",
                    });
                }
                if (config.HttpsPort.HasValue)
                {
                    b.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Kestrel:Endpoints:HttpsDefaultCert:Url"] = $"https://{config.HttpsInterface}:{config.HttpsPort}",
                    });
                }
            })
            .ConfigureServices((context, services) =>
            {
                services
                    .AddOrleans()
                    .AddHostedService<OrleansHealthCheckHostedService>()
                    .Configure<OrleansCheckHostedServiceOptions>(c => { c.Port = config.OrleansHealthCheckPort; })
                    .AddHostedService<ConsulKVRegistration>()

                    .Configure<OrleansClusterConfig>(o => context.Configuration.Bind("Orleans:Cluster", o))
                ;
            })

            .If(config.RegisterSiloWithConsul, builder => builder
                    .RegisterSiloWithConsul(o =>
                    {
                        int tcpTimeout = 15;

                        var checks = new List<AgentServiceCheck>();
                        //checks.Add(new AgentServiceCheck
                        //{
                        //    Name = "Self-report",
                        //    TTL = TimeSpan.FromSeconds(30),
                        //    Status = HealthStatus.Passing,
                        //});
                        if (config.OrleansDashboard)
                        {
                            checks.Add(new AgentServiceCheck
                            {
                                Name = "Dashboard",
                                Notes = $"http://{config.DashboardInterface}:{config.DashboardPort}",
                                HTTP = $"http://{config.DashboardInterface}:{config.DashboardPort}",
                                Interval = TimeSpan.FromSeconds(60),
                            });
                        }
                        checks.Add(new AgentServiceCheck
                        {
                            Name = "HealthCheck",
                            Notes = $"http://{config.HttpInterface}:{config.HttpPort}/health",
                            HTTP = $"http://{config.HttpInterface}:{config.HttpPort}/health",
                            Interval = TimeSpan.FromSeconds(60),
                        });
                        checks.Add(new AgentServiceCheck
                        {
                            Name = $"Silo Port",
                            Notes = $"{config.OrleansInterface}:{config.SiloPort}",
                            TCP = $"{config.OrleansInterface}:{config.SiloPort}",
                            Interval = TimeSpan.FromSeconds(tcpTimeout),
                        });
                        checks.Add(new AgentServiceCheck
                        {
                            Name = "Gateway Port",
                            Notes = $"{config.OrleansInterface}:{config.GatewayPort}",
                            TCP = $"{config.OrleansInterface}:{config.GatewayPort}",
                            Interval = TimeSpan.FromSeconds(tcpTimeout),
                        });

                        o.Address = config.OrleansInterface;
                        o.Port = config.GatewayPort;
                        o.Checks = checks;
                    })
            )
            .UseOrleans((context, builder) =>
            {
                OrleansClusterConfig clusterConfig = new();
                context.Configuration.Bind("Orleans:Cluster", clusterConfig);

                // TODO: better way of confguring ClusterId/ServiceId.  Also see LionFire.Consul.Orleans RegisterSiloWithConsulHostedService
                var clusterId = clusterConfig.ClusterId ?? OrleansClusterConfig.DefaultClusterId;
                var serviceId = config?.ServiceId ?? clusterConfig.ServiceId ?? OrleansClusterConfig.DefaultServiceId;

                var deploymentId = clusterId;
                if (deploymentId == "blue" || deploymentId == "green") { deploymentId = "prod"; }
                if (deploymentId == "beta.blue" || deploymentId == "beta.green") { deploymentId = "beta"; }

                builder
                    .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = deploymentId;
                        options.ServiceId = serviceId;
                    })
                    //.ConfigureApplicationParts(parts => // OLD - Orleans 3
                    //{
                    //    parts.AddApplicationPart(typeof(LocalHealthCheckGrain).Assembly).WithReferences();
                    //    //parts.AddFromApplicationBaseDirectory();
                    //})

                    .If(clusterConfig.Kind == ClusterDiscovery.Localhost, s =>
                        // NOTE - redundant specification of ClusterId and ServiceId
                        s.UseLocalhostClustering(config.SiloPort, config.GatewayPort, config.LocalhostPrimaryClusterEndpoint, serviceId, deploymentId
                        ))
                    .If(clusterConfig.Kind == ClusterDiscovery.Consul, s =>
                        s.UseConsulSiloClustering(gatewayOptions =>
                        {
                            OrleansConsulClusterConfig clusterConsulConfig = new();
                            context.Configuration.Bind("Orleans:Cluster:Consul", clusterConsulConfig);

                            if (clusterConsulConfig.ServiceDiscoverEndPoint != null)
                            {
                                gatewayOptions.ConfigureConsulClient(new Uri(clusterConsulConfig.ServiceDiscoverEndPoint), clusterConsulConfig.ServiceDiscoveryToken);
                            }

                            gatewayOptions.KvRootFolder = clusterConsulConfig.KvFolderName ?? $"{serviceId}";
                        })
                    )
                    .If(clusterConfig.Kind == ClusterDiscovery.Redis, s =>
                        s.UseRedisClustering(gatewayOptions =>
                        {
                            OrleansRedisClusterConfig clusterRedisConfig = new();
                            context.Configuration.Bind("Orleans:Cluster:Redis", clusterRedisConfig);

                            gatewayOptions.Database = clusterRedisConfig.Database ?? 3;
                            gatewayOptions.ConnectionString = clusterRedisConfig.ConnectionString ?? "localhost:6379";
                        })
                    )
                    .If(clusterConfig.Kind != ClusterDiscovery.Localhost && clusterConfig.Kind != ClusterDiscovery.Consul && clusterConfig.Kind != ClusterDiscovery.Redis, s =>
                        throw new NotSupportedException($"Unknown clusterConfig.Kind: {clusterConfig.Kind}"))

                    .ConfigureEndpoints(IPAddress.Parse(config.OrleansInterface), config.SiloPort, config.GatewayPort)
                    .ConfigureLogging(logging => logging.AddConsole())
                    //.UsePerfCounterEnvironmentStatistics() // TODO TOPORT from Orleans 3.x
                    ;

                configureSilo?.Invoke(context, builder);

                //if (!OperatingSystem.IsWindows()) { builder.UseLinuxEnvironmentStatistics(); }  // TODO TOPORT from Orleans 3.x

                if (config.OrleansDashboard) builder.UseDashboard(options => { options.Port = config.DashboardPort; options.Host = config.DashboardInterface; });
            })
            ;

        return lf;
    }

}
public static class ISiloBuilderExtensions
{
    public static ISiloBuilder If(this ISiloBuilder siloBuilder, Func<bool> predicate, Action<ISiloBuilder> a)
    {
        if (predicate()) a(siloBuilder);
        return siloBuilder;
    }
    public static ISiloBuilder If(this ISiloBuilder siloBuilder, bool predicate, Action<ISiloBuilder> a)
    {
        if (predicate) a(siloBuilder);
        return siloBuilder;
    }
}
