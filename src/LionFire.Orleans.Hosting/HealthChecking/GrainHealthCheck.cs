using System.Threading.Tasks;
using Orleans;
using System.Threading;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System;

namespace LionFire.Orleans_.AspNetCore_;


public class GrainHealthCheck : IHealthCheck
{
    private readonly IClusterClient client;
    private readonly IHostApplicationLifetime lifetime;

    public GrainHealthCheck(IClusterClient client, IHostApplicationLifetime lifetime)
    {
        this.client = client;
        this.lifetime = lifetime;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (lifetime.ApplicationStopping.IsCancellationRequested)
            return HealthCheckResult.Unhealthy("Application is shutting down.");

        try
        {
            await client.GetGrain<ILocalHealthCheckGrain>(Guid.Empty).PingAsync();
        }
        catch (Exception error)
        {
            return HealthCheckResult.Unhealthy("Failed to ping the local health check grain.", error);
        }
        return HealthCheckResult.Healthy();
    }
}
