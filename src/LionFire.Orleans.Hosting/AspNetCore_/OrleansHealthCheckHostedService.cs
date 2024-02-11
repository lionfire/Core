#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Orleans;
using Orleans.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Threading;
using Orleans.Runtime;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Orleans.Concurrency;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using LionFire.Hosting;
using Microsoft.Extensions.Configuration;
using Orleans.Configuration;

namespace LionFire.Orleans_.AspNetCore_;

public static class OrleansHealthCheckX
{
    public static IServiceCollection AddSiloHealthChecks(this IServiceCollection services, HostBuilderContext context, IEnumerable<IHealthCheckParticipant>? healthCheckParticipants = null)
    {
        services.Configure<ClusterHealthOptions>(context.Configuration.GetSection(ClusterHealthOptions.ConfigLocation));
        services.AddHealthChecks()
                        .AddCheck<GrainHealthCheck>("GrainHealth")
                        .AddCheck<SiloHealthCheck>("SiloHealth")
                        //.AddCheck<StorageHealthCheck>("StorageHealth")
                        .AddCheck<ClusterHealthCheck>("ClusterHealth");

        services.AddSingleton<IHealthCheckPublisher, LoggingHealthCheckPublisher>()
             .Configure<HealthCheckPublisherOptions>(options =>
             {
                 options.Period = TimeSpan.FromSeconds(60);
             });

        if (healthCheckParticipants != null) { services.AddSingleton(healthCheckParticipants); }

        return services;
    }

    // OLD
    //public static IWebHostBuilder UseSiloHealthChecks(this IWebHostBuilder builder, IOptions<OrleansCheckHostedServiceOptions> myOptions, IEnumerable<IHealthCheckParticipant>? healthCheckParticipants = null)
    //    => builder
    //        .ConfigureServices(services => services
    //                .AddSiloHealthChecks(healthCheckParticipants)
    //            )
    //        //.Configure(app => app
    //        //        .UseHealthChecks(myOptions.Value.PathString)
    //        //    )
    //        ;
}

#if OLD
public class OrleansHealthCheckHostedService : IHostedService
{
    private readonly IWebHost? host;

    public OrleansHealthCheckHostedService(IClusterClient client, IEnumerable<IHealthCheckParticipant> healthCheckParticipants, IOptions<OrleansCheckHostedServiceOptions> myOptions, IConfiguration configuration)
    {
        var o = new SiloProgramConfig(configuration);
        if (!o.SiloHealthCheckOnPrimaryWebHost)
        {

            host = new WebHostBuilder()
                .UseKestrel(options => options.ListenAnyIP(myOptions.Value.Port))
                .ConfigureServices(services => services
                    .AddSingleton(client)
                    .AddSiloHealthChecks(healthCheckParticipants)
                    .Configure<OrleansCheckHostedServiceOptions>(c => { c.Port = o.SiloHealthCheckPort??throw new ArgumentNullException(nameof(o.SiloHealthCheckPort)); })

                )
                //.UseSiloHealthChecks(myOptions, healthCheckParticipants)
                .Configure(app => app
                        .UseHealthChecks(myOptions.Value.PathString)
                    )
                .Build();
        }
    }

    public Task StartAsync(CancellationToken cancellationToken) => host?.StartAsync(cancellationToken) ?? Task.CompletedTask;
    public Task StopAsync(CancellationToken cancellationToken) => host?.StopAsync(cancellationToken) ?? Task.CompletedTask;
}
#endif