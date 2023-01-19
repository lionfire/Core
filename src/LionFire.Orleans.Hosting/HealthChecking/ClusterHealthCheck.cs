using System.Threading.Tasks;
using Orleans;
using System.Threading;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using Orleans.Runtime;
using System.Linq;

namespace LionFire.Orleans_.AspNetCore_
{
    public class ClusterHealthCheck : IHealthCheck
    {
        private readonly IClusterClient client;

        public ClusterHealthCheck(IClusterClient client)
        {
            this.client = client;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("TODO"));

            //throw new NotImplementedException();//What happened to IsUnavailable?

            //var manager = client.GetGrain<IManagementGrain>(0);
            //try
            //{
            //    var hosts = await manager.GetHosts();
            //    var count = hosts.Values.Where(x => x.IsUnavailable()).Count();
            //    return count > 0 ? HealthCheckResult.Degraded($"{count} silo(s) unavailable") : HealthCheckResult.Healthy();
            //}
            //catch (Exception error)
            //{
            //    return HealthCheckResult.Unhealthy("Failed to get cluster status", error);
            //}
        }
    }
}
