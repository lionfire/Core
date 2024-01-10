using System.Threading.Tasks;
using Orleans;
using System.Threading;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace LionFire.Orleans_.AspNetCore_;

public class StorageHealthCheck : IHealthCheck
{
    private readonly IClusterClient client;

    public StorageHealthCheck(IClusterClient client)
    {
        this.client = client;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await client.GetGrain<IStorageHealthCheckGrain>("0").CheckAsync();
        }
        catch (Exception error)
        {
            return HealthCheckResult.Unhealthy("Failed to ping the storage health check grain.", error);
        }
        return HealthCheckResult.Healthy();
    }
}
