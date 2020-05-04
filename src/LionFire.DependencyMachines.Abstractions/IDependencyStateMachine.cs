using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace LionFire.DependencyMachines
{

    public interface IDependencyStateMachine: IHostedService
    {
        IServiceProvider ServiceProvider { get; set; }

        IDependencyStateMachine Register(IParticipant participant, bool isAlreadyStarted = false);
        IDependencyStateMachine Register(IEnumerable<IParticipant> participants);

        bool Unregister(IParticipant participant);

        IEnumerable<string> StartLog { get; }
        IEnumerable<string> StopLog { get; }
        IEnumerable<int> StartStageLog { get; }
        IEnumerable<int> StopStageLog { get; }
    }

    public static class IDependencyStateMachineExtensions
    {
        public static void Start(this IDependencyStateMachine dsm) => dsm.StartAsync(default).Wait();


        

    }
}