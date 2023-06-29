using Microsoft.Extensions.Hosting;

namespace LionFire.Data;

public abstract class HostedAsyncGets<T> : AsyncGets<T>, IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => Get(cancellationToken).AsTask();
    public Task StopAsync(CancellationToken cancellationToken) { Discard(); return Task.CompletedTask; }
}
