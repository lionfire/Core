using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace LionFire.DependencyMachines
{

    public interface IDependencyStateMachine : IHostedService
    {
        IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Name of the DependencyStateMachine.  (Used to retrieve named configuration.)
        /// </summary>
        string Name { get; }

        //IDependencyStateMachine Register(IParticipant participant, bool isAlreadyStarted = false);
        //IDependencyStateMachine Register(IEnumerable<IParticipant> participants);

        //bool Unregister(IParticipant participant);

        /// <summary>
        /// Manual config (replaces InjectedConfig)
        /// </summary>
        DependencyMachineConfig? Config { get; set; }
        DependencyMachineConfig EffectiveConfig { get; }
        DependencyMachineConfig InjectedConfig { get; }

        IEnumerable<string> StartLog { get; }
        IEnumerable<string> StopLog { get; }
        IEnumerable<int> StartStageLog { get; }
        IEnumerable<int> StopStageLog { get; }

        #region Events

        event Action<IParticipant> Starting;
        event Action<IParticipant> Started;
        event Action<string> StartedStage;
        event Action<string> StoppedStage;
        event Action<IParticipant> Stopping;
        event Action<IParticipant, bool /* initialRegistration */> Stopped;


        #endregion

        #region Runtime signals

        void Set(string key);
        void Set(string key, object value);

        #endregion
    }

    public static class IDependencyStateMachineExtensions
    {
        public static void Start(this IDependencyStateMachine dsm) => dsm.StartAsync(default).Wait();




    }
}