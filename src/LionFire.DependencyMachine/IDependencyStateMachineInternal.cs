using Microsoft.Extensions.Hosting;

namespace LionFire.DependencyMachine
{
    internal interface IDependencyStateMachineInternal : IHostedService, IDependencyStateMachine
    {

    }

}