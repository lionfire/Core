using LionFire.StateMachines.Class;
using LionFire.Threading;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Trading.Feeds
{
    //public interface IWaitForHostedState
    //{
    //    Task WaitFor<TState>(TState state, CancellationToken cancellationToken = default);
    //}

    public abstract class HostedServiceMachine<TConcrete> : IHostedService
    {
        StateMachineState<HostedServiceState, HostedServiceTransitions, TConcrete> sm;

        public HostedServiceMachine()
        {
            sm = new StateMachineState<HostedServiceState, HostedServiceTransitions, TConcrete>((TConcrete)(object)this);
        }

        private readonly ManualResetEventAsync mre = new ManualResetEventAsync(false, false);

        #region WaitForStarted

        public async Task WaitForStarted() => await mre.WaitAsync();
        public async Task WaitForStarted(CancellationToken cancellationToken) => await mre.WaitAsync(cancellationToken);
        public async Task WaitForStarted(TimeSpan timeout, CancellationToken cancellationToken) => await mre.WaitAsync(timeout, cancellationToken);
        public async Task WaitForStarted(TimeSpan timeout) => await mre.WaitAsync(timeout);

        #endregion

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            mre.Reset();
            sm.Transition(HostedServiceTransitions.Start);
            await OnStartAsync(cancellationToken);
            mre.Set();
            sm.Transition(HostedServiceTransitions.Started);
        }

        protected abstract Task OnStartAsync(CancellationToken cancellationToken);
        protected abstract Task OnStopAsync(CancellationToken cancellationToken);

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            mre.Reset();
            sm.Transition(HostedServiceTransitions.Stop);
            await OnStopAsync(cancellationToken);
            sm.Transition(HostedServiceTransitions.Stopped);
        }
    }
}
