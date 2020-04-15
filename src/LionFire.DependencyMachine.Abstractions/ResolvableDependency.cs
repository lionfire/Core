using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.DependencyMachine
{
    public abstract class ResolvableDependency : IReactor, IHostedService
    {
        public abstract string Key { get; }

        public InitializerFlags Flags { get; set; }

        public IEnumerable<object> Conditions { get; set; }
        public IEnumerable<object> Provides { get; set; }
        public IEnumerable<object> Contributes { get; set; }

        public Func<DependendyMachineContext, CancellationToken, Task<object>> StartAction { get; set; }
        public Func<DependendyMachineContext, CancellationToken, Task<object>> StopAction { get; set; }

        public bool RerunOnChanges { get; set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (StartAction != null) return StartAction?.Invoke(null, cancellationToken);
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (StopAction != null) return StopAction?.Invoke(null, cancellationToken);
            return Task.CompletedTask;
        }
    }
}
