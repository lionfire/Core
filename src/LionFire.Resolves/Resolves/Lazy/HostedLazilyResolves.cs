using Microsoft.Extensions.Hosting;
using MorseCode.ITask;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Resolves
{
    public abstract class HostedLazilyResolves<T> : LazilyResolves<T>, IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken) => Resolve().AsTask();
        public Task StopAsync(CancellationToken cancellationToken) { value = default; HasValue = false; return Task.CompletedTask; }
    }
}
