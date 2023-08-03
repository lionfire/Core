using Microsoft.Extensions.Hosting;

namespace LionFire.Data.Async.Gets;

// ENH: OneShot: Get once and freeze
public abstract class HostedGetter<T> : GetterRxO<T>, IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => Get(cancellationToken).AsTask();
    public Task StopAsync(CancellationToken cancellationToken) { Discard(); return Task.CompletedTask; }
}
