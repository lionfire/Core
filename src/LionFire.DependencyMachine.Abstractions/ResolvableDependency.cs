using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.DependencyMachine
{
    public abstract class ResolvableDependency : IDependencyMachineParticipant, IHostedService
    {
        public abstract string Key { get; }

        public InitializerFlags Flags { get; set; }

        #region Dependencies

        public IEnumerable<object> Dependencies
        {
            get => dependencies ?? Enumerable.Empty<object>();
            set => dependencies = value;
        }
        private IEnumerable<object> dependencies;

        #endregion
        #region Provides

        public IEnumerable<object> Provides
        {
            get => provides ?? Enumerable.Empty<object>();
            set => provides = value;
        }
        private IEnumerable<object> provides;

        #endregion

        #region Contributes

        public IEnumerable<object> Contributes
        {
            get => contributes ?? Enumerable.Empty<object>();
            set => contributes = value;
        }
        private IEnumerable<object> contributes;

        #endregion

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

        public override string ToString() => Key;

    }
}
