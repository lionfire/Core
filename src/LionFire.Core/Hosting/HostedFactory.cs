using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Hosting
{
    public class HostedFactory<T> : IHostedService
    {
        T Service;

        #region Factory

        public Func<T> Factory
        {
            get => factory ?? (() => Activator.CreateInstance<T>());
            set => factory = value;
        }
        private Func<T> factory;

        #endregion

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // ENH: Ghetto dependency sequencing idea: Keep retrying Factory() while it returns default(T) to give other dependencies a chance to become available
            Service = Factory();
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (Service is IDisposable d) d.Dispose();
            Service = default;
            return Task.CompletedTask;
        }
    }
}
