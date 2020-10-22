using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Hosting
{
    /// <summary>
    /// Wraps any type in a HostedService&lt;T&gt; in IHostedService so it can be added via AddHostedService.
    /// Intended for use with the IServiceCollection.AddHostedSingleton extension method.
    /// IHostedService.StopAsync will dispose the instance if T implements IDisposable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HostedService<T> : IHostedService
    {
        T Service;

        public HostedService(T service)
        {
            Service = service;
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask; // No-op

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (Service is IDisposable d) d.Dispose();
            Service = default;
            return Task.CompletedTask;
        }
    }
}
