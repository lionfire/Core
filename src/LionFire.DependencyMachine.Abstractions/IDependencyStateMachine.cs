using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace LionFire.DependencyMachine
{
    // ENH: Fluent API

    public interface IDependencyStateMachine : IHostedService
    {
        IServiceProvider ServiceProvider { get; set; }

        void Register(IDependencyMachineParticipant participant, bool isAlreadyStarted = false);
        bool Unregister(IDependencyMachineParticipant participant);

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