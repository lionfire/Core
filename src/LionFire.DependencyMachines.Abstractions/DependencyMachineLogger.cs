using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.DependencyMachines.Abstractions
{
    public class DependencyMachineLogger
    {
        public DependencyMachineLogger(ILogger<IDependencyStateMachine> logger, IDependencyStateMachine dependencyStateMachine)
        {
            if (dependencyStateMachine.Name == null)
            {
                logger.LogInformation("=============================================");
            }

            Logger = logger;
            DependencyStateMachine = dependencyStateMachine;

            DependencyStateMachine.Starting += p => Logger.LogDebug($"  - [starting] {p}");
            DependencyStateMachine.Started += p => Logger.LogInformation($"  - [STARTED] {p}");

            DependencyStateMachine.Stopping += p => Logger.LogDebug($"  - [stopping] {p}");
            DependencyStateMachine.Stopped += (p,_) => Logger.LogInformation($"  - [STOPPED] {p}");

            DependencyStateMachine.StartedStage += p => Logger.LogInformation($"[[STARTED]] Stage {p}");
            DependencyStateMachine.StoppedStage += p => Logger.LogInformation($"[[STOPPED]] Stage {p}");
        }

        public ILogger<IDependencyStateMachine> Logger { get; }
        public IDependencyStateMachine DependencyStateMachine { get; }
    }
}
