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
using HostBuilderContext = Microsoft.Extensions.Hosting.HostBuilderContext;
using Orleans.Serialization;
using Microsoft.Extensions.DependencyInjection;
using LionFire.ExtensionMethods.Copying;
using LionFire.Consul_.Orleans_;
using LionFire.Configuration;
using LionFire.AspNetCore;
using System.Linq;
using System.Diagnostics;
using Orleans.Runtime;
using System.IO;

namespace LionFire.Hosting;

public static class SiloHostBuilder
{
    public static ILionFireHostBuilder Silo(this ILionFireHostBuilder lf, Action<HostBuilderContext, ISiloBuilder>? configureSilo = null, Action<SiloProgramConfig>? configureOptions = null)
    {
        OrleansStaticInitialization.InitOrleans();

        var siloOptions2 = new SiloProgramConfig(lf.HostBuilder.Configuration);

        lf
            .ConfigureServices((context, services) =>
            {
                var siloOptions = new SiloProgramConfig(context.Configuration);

                var needWebHostOptions = siloOptions.SiloHealthCheckOnPrimaryWebHost;
                var webHostOptions = needWebHostOptions ? new WebHostConfig(context.Configuration) : null;

                services
                    .AddOrleans()
                    
                    .Configure<SiloProgramConfig>(context.Configuration.GetSection(siloOptions.ConfigLocation))
                    .AddTransient<LionFireSiloConfigurator>()


                    .If(siloOptions.SiloHealthCheckEnabled, s => s.AddSiloHealthChecks(context))
                   // .If(!siloOptions.SiloHealthCheckOnPrimaryWebHost, s => s.AddHostedService<OrleansHealthCheckHostedService>()) // OLD

                    .Configure<OrleansClusterConfig>(o => context.Configuration.Bind("Orleans:Cluster", o))
                    .Configure<SiloOptions>(options => options.SiloName ??= $"LFSilo_{Guid.NewGuid().ToString("N").Substring(0, 5)}")

                #region Consul

                    .Configure<ConsulServiceOptions>(o =>
                    {
                        int tcpTimeout = 15;

                        var checks = o.Checks;

                        //checks.Add(new AgentServiceCheck
                        //{
                        //    Name = "Self-report",
                        //    TTL = TimeSpan.FromSeconds(30),
                        //    Status = HealthStatus.Passing,
                        //});
                        if (siloOptions.DashboardEnabled)
                        {
                            checks.Add(new AgentServiceCheck
                            {
                                Name = "Dashboard",
                                Notes = $"http://{siloOptions.DashboardInterface}:{siloOptions.DashboardPort}",
                                HTTP = $"http://{siloOptions.DashboardInterface}:{siloOptions.DashboardPort}",
                                Interval = TimeSpan.FromSeconds(60),
                            });
                        }

                        // TODO: disable these if not set.  Default to HTTP on, HTTPS off. 

                        checks.Add(new AgentServiceCheck
                        {
                            Name = $"Silo Port",
                            Notes = $"{siloOptions.SiloInterface}:{siloOptions.SiloPort}",
                            TCP = $"{siloOptions.SiloInterface}:{siloOptions.SiloPort}",
                            Interval = TimeSpan.FromSeconds(tcpTimeout),
                        });
                        checks.Add(new AgentServiceCheck
                        {
                            Name = "Gateway Port",
                            Notes = $"{siloOptions.SiloInterface}:{siloOptions.GatewayPort}",
                            TCP = $"{siloOptions.SiloInterface}:{siloOptions.GatewayPort}",
                            Interval = TimeSpan.FromSeconds(tcpTimeout),
                        });

                        #region ENH: copy this to LionFire.AspNetCore.Consul  (and refactor)

                        string? siloHealthCheckUrlBase = null;

                        if (webHostOptions != null && siloOptions.SiloHealthCheckOnPrimaryWebHost)
                        {
                            if (siloOptions.SiloHealthCheckScheme == "http")
                            {
                                if (webHostOptions.Http && webHostOptions.HttpInterface != null && webHostOptions.HttpPort.HasValue)
                                {
                                    siloHealthCheckUrlBase = $"{siloOptions.SiloHealthCheckScheme}://{webHostOptions.HttpInterface}:{webHostOptions.HttpPort.Value}";
                                }
                            }
                            else if (siloOptions.SiloHealthCheckScheme == "https")
                            {
                                if (webHostOptions.Https && webHostOptions.HttpsInterface != null && webHostOptions.HttpsPort.HasValue)
                                {
                                    siloHealthCheckUrlBase = $"{siloOptions.SiloHealthCheckScheme}://{webHostOptions.HttpsInterface}:{webHostOptions.HttpsPort.Value}";
                                }
                            }

                            siloHealthCheckUrlBase ??= webHostOptions?.Urls?.Where(u => u.StartsWith(siloOptions.SiloHealthCheckScheme)).FirstOrDefault();

                            if (siloHealthCheckUrlBase == null)
                            {
                                Debug.WriteLine($"SiloHealthCheckOnPrimaryWebHost is true but could not find a URL configured for {siloOptions.SiloHealthCheckScheme}");
                            }
                        }
                        else
                        {
                            siloHealthCheckUrlBase = $"{siloOptions.SiloHealthCheckScheme}://{siloOptions.SiloHealthCheckInterface}:{siloOptions.SiloHealthCheckPort}";
                        }

                        if (siloHealthCheckUrlBase != null)
                        {
                            Debug.WriteLine($"siloHealthCheckUrlBase: {siloHealthCheckUrlBase}");
                            checks.Add(new AgentServiceCheck
                            {
                                Name = "HealthCheck",
                                Notes = $"{siloHealthCheckUrlBase}/health",
                                HTTP = $"{siloHealthCheckUrlBase}/health",
                                Interval = TimeSpan.FromSeconds(60),
                            });
                        }

                        #endregion

                        o.Address = siloOptions.SiloInterface; // TODO FIXME: LAN IP address
                        o.Port = siloOptions.GatewayPort ?? throw new ArgumentNullException(nameof(o.Port));
                    })
                    .If(configureOptions != null, s => s.Configure(configureOptions!))
                ;
            })
            .HostBuilder
            .If(!siloOptions2.SiloHealthCheckOnPrimaryWebHost, b => 
            throw new NotImplementedException("TODO: SiloHealthCheckOnPrimaryWebHost == false not implemented yet")
            //b.ConfigureWebHostDefaults(builder => builder.UseSiloHealthChecks(myOptions))
            )
            .RegisterSiloWithConsul() // Will only register if ConsulServiceOptions.Register == true and/or ConsulKVOptions.Register == true
                                      // MOVE to ILionFireHostBuilder extension method

        #endregion


                .UseLionFireOrleans(configureSilo) // MOVE to ILionFireHostBuilder extension method
            ;

        return lf;
    }
}
