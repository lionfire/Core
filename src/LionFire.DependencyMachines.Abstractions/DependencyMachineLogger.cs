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
            //if (dependencyStateMachine.Name == null)
            //{
            //    logger.LogInformation("=============================================");
            //}

            Logger = logger;
            DependencyStateMachine = dependencyStateMachine;

            DependencyStateMachine.Starting += p => Logger.LogTrace($"  - [starting] {p}");
            DependencyStateMachine.Started += p => Logger.LogDebug($"  - [STARTED] {p}");

            DependencyStateMachine.Stopping += p => Logger.LogTrace($"  - [stopping] {p}");
            DependencyStateMachine.Stopped += (p,_) => Logger.LogDebug($"  - [STOPPED] {p}");

            DependencyStateMachine.StartedStage += p => Logger.LogDebug($"=====!   [[STARTED]] Stage {p}   !=====");
            DependencyStateMachine.StoppedStage += p => Logger.LogDebug($"=====!x   [[STOPPED]] Stage {p}   x!=====");
        }

        public ILogger<IDependencyStateMachine> Logger { get; }
        public IDependencyStateMachine DependencyStateMachine { get; }
    }
}
