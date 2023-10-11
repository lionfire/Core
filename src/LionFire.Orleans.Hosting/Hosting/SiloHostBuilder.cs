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
using Microsoft.Extensions.DependencyInjection;
using LionFire.ExtensionMethods.Copying;
using LionFire.Consul_.Orleans_;

namespace LionFire.Hosting;

public static class SiloHostBuilder
{
    //public static ILionFireHostBuilder Silo(this ILionFireHostBuilder lf, int port, Action<HostBuilderContext, ISiloBuilder>? configureSilo = null)
    //    => lf.Silo(new SiloProgramOptions(port), configureSilo);

    //public static ILionFireHostBuilder Silo(this ILionFireHostBuilder lf, SiloProgramOptions? defaultOptions = null, Action<HostBuilderContext, ISiloBuilder>? configureSilo = null)
    //{
    //    return lf.Silo(configureOptions: o =>
    //    {
    //        defaultOptions?.AssignPropertiesTo(o);
    //    }, configureSilo);
    //}

    public static ILionFireHostBuilder Silo(this ILionFireHostBuilder lf, Action<HostBuilderContext, ISiloBuilder>? configureSilo = null, Action<SiloProgramOptions>? configureOptions = null)
    {
        OrleansStaticInitialization.InitOrleans();

        lf.HostBuilder
            .ConfigureAppConfiguration((context, b) =>
            {
                SiloProgramOptions options = new();
                context.Configuration.Bind(SiloProgramOptions.ConfigLocation, options);
                if (options.HttpPort.HasValue)
                {
                    b.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Kestrel:Endpoints:Http:Url"] = $"http://{options.HttpInterface}:{options.HttpPort}",
                    });
                }
                if (options.HttpsPort.HasValue)
                {
                    b.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Kestrel:Endpoints:HttpsDefaultCert:Url"] = $"https://{options.HttpsInterface}:{options.HttpsPort}",
                    });
                }
            })
            .ConfigureServices((context, services) =>
            {
                SiloProgramOptions options = new();
                context.Configuration.Bind(SiloProgramOptions.ConfigLocation, options);

                services
                    .If(configureOptions != null, s => s.Configure(configureOptions!))
                    .AddOrleans()
                    .Configure<SiloProgramOptions>(context.Configuration.GetSection(SiloProgramOptions.ConfigLocation))
                    .AddTransient<LionFireSiloConfigurator>()
                    .AddHostedService<OrleansHealthCheckHostedService>()
                    .Configure<OrleansCheckHostedServiceOptions>(c => { c.Port = options.OrleansHealthCheckPort; })


                    .Configure<OrleansClusterConfig>(o => context.Configuration.Bind("Orleans:Cluster", o))
                    .Configure<SiloOptions>(options => options.SiloName ??= $"LFSilo_{Guid.NewGuid().ToString("N").Substring(0, 5)}")

                #region Consul

                    .Configure<ConsulServiceOptions>(o =>
                    {
                        int tcpTimeout = 15;

                        var checks = new List<AgentServiceCheck>();
                        //checks.Add(new AgentServiceCheck
                        //{
                        //    Name = "Self-report",
                        //    TTL = TimeSpan.FromSeconds(30),
                        //    Status = HealthStatus.Passing,
                        //});
                        if (options.DashboardEnabled)
                        {
                            checks.Add(new AgentServiceCheck
                            {
                                Name = "Dashboard",
                                Notes = $"http://{options.DashboardInterface}:{options.DashboardPort}",
                                HTTP = $"http://{options.DashboardInterface}:{options.DashboardPort}",
                                Interval = TimeSpan.FromSeconds(60),
                            });
                        }
                        checks.Add(new AgentServiceCheck
                        {
                            Name = "HealthCheck",
                            Notes = $"http://{options.HttpInterface}:{options.HttpPort}/health",
                            HTTP = $"http://{options.HttpInterface}:{options.HttpPort}/health",
                            Interval = TimeSpan.FromSeconds(60),
                        });
                        checks.Add(new AgentServiceCheck
                        {
                            Name = $"Silo Port",
                            Notes = $"{options.OrleansInterface}:{options.SiloPort}",
                            TCP = $"{options.OrleansInterface}:{options.SiloPort}",
                            Interval = TimeSpan.FromSeconds(tcpTimeout),
                        });
                        checks.Add(new AgentServiceCheck
                        {
                            Name = "Gateway Port",
                            Notes = $"{options.OrleansInterface}:{options.GatewayPort}",
                            TCP = $"{options.OrleansInterface}:{options.GatewayPort}",
                            Interval = TimeSpan.FromSeconds(tcpTimeout),
                        });

                        o.Address = options.OrleansInterface;
                        o.Port = options.GatewayPort;
                        o.Checks = checks;
                    })
                ;
            })
            .RegisterSiloWithConsul() // Will only register if ConsulServiceOptions.Register == true and/or ConsulKVOptions.Register == true

        #endregion

            .UseLionFireOrleans(configureSilo)
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
