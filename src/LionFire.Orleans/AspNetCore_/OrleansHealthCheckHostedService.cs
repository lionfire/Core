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

namespace LionFire.Orleans_.AspNetCore_
{



    public class OrleansHealthCheckHostedService : IHostedService
    {
        private readonly IWebHost host;

        public OrleansHealthCheckHostedService(IClusterClient client, IEnumerable<IHealthCheckParticipant> healthCheckParticipants, IOptions<OrleansCheckHostedServiceOptions> myOptions)
        {
            host = new WebHostBuilder()
                .UseKestrel(options => options.ListenAnyIP(myOptions.Value.Port))
                .ConfigureServices(services =>
                {
                    services.AddHealthChecks()
                        .AddCheck<GrainHealthCheck>("GrainHealth")
                        .AddCheck<SiloHealthCheck>("SiloHealth")
                        .AddCheck<StorageHealthCheck>("StorageHealth")
                        .AddCheck<ClusterHealthCheck>("ClusterHealth");

                    services.AddSingleton<IHealthCheckPublisher, LoggingHealthCheckPublisher>()
                         .Configure<HealthCheckPublisherOptions>(options =>
                         {
                             options.Period = TimeSpan.FromSeconds(1);
                         });

                    services.AddSingleton(client);


                })
                .Configure(app =>
                {
                    app.UseHealthChecks(myOptions.Value.PathString);
                })
                .Build();


        }

        public Task StartAsync(CancellationToken cancellationToken) => host.StartAsync(cancellationToken);
        public Task StopAsync(CancellationToken cancellationToken) => host.StopAsync(cancellationToken);
    }
}
