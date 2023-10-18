using System.Threading.Tasks;
using Orleans;
using System.Threading;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using Orleans.Runtime;
using System.Linq;
using Microsoft.Extensions.Options;

namespace LionFire.Orleans_.AspNetCore_;

public class ClusterHealthOptions
{
    public const string ConfigLocation = "Orleans:Health:Cluster";
    public int HealthySiloCount { get; set; } = 3;
}

public class ClusterHealthCheck : IHealthCheck
{
    private readonly IClusterClient client;
    private readonly IOptionsMonitor<ClusterHealthOptions> optionsMonitor;

    public ClusterHealthCheck(IClusterClient client, IOptionsMonitor<ClusterHealthOptions> optionsMonitor)
    {
        this.client = client;
        this.optionsMonitor = optionsMonitor;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        //return Task.FromResult(HealthCheckResult.Unhealthy("TODO"));

        ////throw new NotImplementedException();//What happened to IsUnavailable?

        var manager = client.GetGrain<IManagementGrain>(0);
        try
        {
            var hosts = await manager.GetHosts();
            var activeCount = hosts.Values.Where(x => x == SiloStatus.Active).Count();
            var deadCount = hosts.Values.Where(x => x == SiloStatus.Dead).Count();

            //if(deadCount > 0)
            //    return HealthCheckResult.Degraded($"{deadCount} dead silo(s)");

            var options = optionsMonitor.CurrentValue;

            return activeCount >= options.HealthySiloCount ? HealthCheckResult.Healthy() : HealthCheckResult.Degraded($"Only {activeCount} active silo(s). Configured to be healthy when there are at least {options.HealthySiloCount} active silos. ({deadCount} dead silo(s))") ;
        }
        catch (Exception error)
        {
            return HealthCheckResult.Unhealthy("Failed to get cluster status", error);
        }
    }
}
