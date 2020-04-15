using LionFire.Structures;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.DependencyMachine
{

    public interface IReactor : IKeyed, IHostedService
    {
        IEnumerable<object> Conditions { get; }
        IEnumerable<object> Provides { get; }
        IEnumerable<object> Contributes { get; }

        InitializerFlags Flags { get; }

        Func<DependendyMachineContext, CancellationToken, Task<object>> StartAction { get; }
        Func<DependendyMachineContext, CancellationToken, Task<object>> StopAction { get; }

        bool RerunOnChanges { get; }
    }
}
