using Microsoft.Extensions.Hosting;
using MorseCode.ITask;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Data.Gets;

public abstract class HostedGetsSlim<T> : AsyncGetsSlim<T>, IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => Get(cancellationToken).AsTask();
    public Task StopAsync(CancellationToken cancellationToken) { Discard(); return Task.CompletedTask; }
}
