using Microsoft.Extensions.Hosting;
using MorseCode.ITask;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Data.Async.Gets;

public abstract class HostedGetterSlim<T> : GetterSlim<T>, IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => Get(cancellationToken).AsTask();
    public Task StopAsync(CancellationToken cancellationToken) { Discard(); return Task.CompletedTask; }
}
