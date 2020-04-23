using Microsoft.Extensions.Hosting;

namespace LionFire.DependencyMachines
{
    internal interface IDependencyStateMachineInternal : IHostedService, IDependencyStateMachine
    {

    }

}