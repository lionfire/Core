#nullable enable
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
//using Polly;
using HostBuilderContext = Microsoft.Extensions.Hosting.HostBuilderContext;
using Orleans.Serialization;

namespace LionFire.Hosting;

public static class SiloHostBuilder
{
    public static ILionFireHostBuilder Silo(this ILionFireHostBuilder lf, int port, Action<HostBuilderContext, ISiloBuilder>? configureSilo = null)
        => lf.Silo(new SiloProgramConfig(port), configureSilo);

    public static ILionFireHostBuilder Silo(this ILionFireHostBuilder lf, SiloProgramConfig? config = null, Action<HostBuilderContext, ISiloBuilder>? configureSilo = null)
    {
        config ??= new();

        OrleansStaticInitialization.InitOrleans();

        lf.HostBuilder
            .ConfigureHostConfiguration(b =>
            {
                if (config.HttpPort.HasValue)
                {
                    b.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Kestrel:Endpoints:Http:Url"] = $"http://{config.HttpInterface}:{config.HttpPort}",
                    });
                }
                if (config.HttpsPort.HasValue)
                {
                    b.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Kestrel:Endpoints:HttpsDefaultCert:Url"] = $"https://{config.HttpsInterface}:{config.HttpsPort}",
                    });
                }
            })
            .ConfigureServices((context, services) =>
            {
                services
                    .AddOrleans()
                    .AddTransient<LionFireSiloConfigurator>()
                    .AddHostedService<OrleansHealthCheckHostedService>()
                    .Configure<OrleansCheckHostedServiceOptions>(c => { c.Port = config.OrleansHealthCheckPort; })
                    .If(config.RegisterSiloWithConsul, s => s
                        .AddHostedService<ConsulKVRegistration>()
                    )

                    .Configure<OrleansClusterConfig>(o => context.Configuration.Bind("Orleans:Cluster", o))
                    .Configure<SiloOptions>(options => options.SiloName ??= $"LFSilo_{Guid.NewGuid().ToString("N").Substring(0, 5)}");
                ;
            })

            .UseLionFireOrleans(config, configureSilo)

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
                        if (config.DashboardEnabled)
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
