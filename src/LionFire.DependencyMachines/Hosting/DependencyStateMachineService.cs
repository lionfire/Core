using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.DependencyMachines
{
    /// <summary>
    /// This is just the IHostedService (registered as a Transient) that is used to start or stop the IDependencyStateMachine
    /// which needs to be registered as a singleton).
    /// 
    /// Add this via .AddHostedService&lt;DependencyStateMachineService&gt;().  
    /// Also add .AddSingleton&lt;IDependencyStateMachine, DependencyStateMachine&gt;()
    /// </summary>
    public class DependencyStateMachineService : IHostedService
    {
        public DependencyStateMachineService(IDependencyStateMachine dependencyStateMachine, IEnumerable<IParticipant> participants)
        {
            DependencyStateMachine = (IDependencyStateMachineInternal)dependencyStateMachine;
        }

        internal  IDependencyStateMachineInternal DependencyStateMachine { get; }

        public Task StartAsync(CancellationToken cancellationToken) => DependencyStateMachine.StartAsync(cancellationToken);
        public Task StopAsync(CancellationToken cancellationToken) => DependencyStateMachine.StopAsync(cancellationToken);
    }

}