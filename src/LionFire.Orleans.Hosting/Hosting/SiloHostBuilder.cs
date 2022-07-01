
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

namespace LionFire.Hosting;

public static class SiloHostBuilder
{

    public static LionFireHostBuilder Silo(this LionFireHostBuilder lf, int port, Action<Microsoft.Extensions.Hosting.HostBuilderContext, ISiloBuilder>? configureSilo = null)
        => lf.Silo(new SiloProgramConfig(port), configureSilo);

    public static LionFireHostBuilder Silo(this LionFireHostBuilder lf, SiloProgramConfig config = null, Action<Microsoft.Extensions.Hosting.HostBuilderContext, ISiloBuilder>? configureSilo = null)
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



                builder
                    .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = clusterConfig.ClusterId ?? OrleansClusterConfig.DefaultClusterId;
                        options.ServiceId = clusterConfig.ServiceId ?? OrleansClusterConfig.DefaultServiceId;
                    })
                    .ConfigureApplicationParts(parts =>
                    {
                        parts.AddApplicationPart(typeof(LocalHealthCheckGrain).Assembly).WithReferences();
                        //parts.AddFromApplicationBaseDirectory();
                    })

                    .If(clusterConfig.Kind == ClusterDiscovery.Localhost, s =>
                        // NOTE - redundant specification of ClusterId and ServiceId
                        s.UseLocalhostClustering(config.SiloPort, config.GatewayPort, new IPEndPoint(IPAddress.Loopback, 99999), clusterConfig.ServiceId, clusterConfig.ClusterId
                        ))
                    .If(clusterConfig.Kind == ClusterDiscovery.Consul, s =>
                        s.UseConsulClustering(gatewayOptions =>
                        {
                            OrleansConsulClusterConfig clusterConsulConfig = new();
                            context.Configuration.Bind("Orleans:Cluster:Consul", clusterConsulConfig);

                            if (clusterConsulConfig.ServiceDiscoverEndPoint != null)
                            {
                                gatewayOptions.Address = new Uri(clusterConsulConfig.ServiceDiscoverEndPoint);
                            }
                            gatewayOptions.AclClientToken = clusterConsulConfig.ServiceDiscoveryToken;
                            gatewayOptions.KvRootFolder = clusterConsulConfig.KvFolderName ?? clusterConfig.ServiceId;
                        })
                    )

                    .ConfigureEndpoints(IPAddress.Parse(config.OrleansInterface), config.SiloPort, config.GatewayPort)
                    .ConfigureLogging(logging => logging.AddConsole())
                    .UsePerfCounterEnvironmentStatistics()
                    ;

                configureSilo?.Invoke(context, builder);

                if (!OperatingSystem.IsWindows()) { builder.UseLinuxEnvironmentStatistics(); }

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
