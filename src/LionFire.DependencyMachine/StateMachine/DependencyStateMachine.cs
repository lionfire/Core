using LionFire.ExtensionMethods;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.DependencyMachine
{
    // ENH: Add events
    // ENH: Pause/unpause
    // ENH: Reactive: detect started/stopped/faulted/paused states on members
    // ENH: Max one Placeholder per stage
    // ENH: Don't bother starting placeholder if there are other items in the stage.  It's only a noop placeholder.  Use stage events if you need to know if a stage is starting/stopping/stopped/started.

    public class DependencyStateMachine : IDependencyStateMachine
    {
        #region Parameters

        public IServiceProvider ServiceProvider { get; set; }

        public DependenyMachineDefinition Definition { get; } = new DependenyMachineDefinition();

        #region IsLoggingEnabled

        public bool IsLoggingEnabled
        {
            get => isLoggingEnabled;
            set
            {
                isLoggingEnabled = value;
                if (!isLoggingEnabled)
                {
                    startStageLog = null;
                    stopStageLog = null;
                    startLog = null;
                    stopLog = null;
                }
            }
        }
        private bool isLoggingEnabled;

        #endregion


        #endregion

        #region Convenience

        public IEnumerable<IDependencyMachineParticipant> Participants => Definition.Participants;

        #endregion

        #region State

        // TODO: State machine / some sort of IExecutor interface: Started Stopped / Starting Stopping
        public bool IsStarted { get; private set; }
        public bool IsStarting { get; private set; }

        public IEnumerable<IDependencyMachineParticipant> Started => startedParticipants.Values;
        ConcurrentDictionary<string, IDependencyMachineParticipant> startedParticipants { get; } = new ConcurrentDictionary<string, IDependencyMachineParticipant>();
        public IEnumerable<IDependencyMachineParticipant> Stopped => stoppedParticipants.Values;
        ConcurrentDictionary<string, IDependencyMachineParticipant> stoppedParticipants { get; } = new ConcurrentDictionary<string, IDependencyMachineParticipant>();

        #region Dependency Logic State

        CompiledDependencyMachine compiled;
        protected void CalculateStages()
        {
            compiled = new CompiledDependencyMachine(Definition);
        }

        protected IEnumerable<DependencyStage> Stages => compiled.Stages;

        #endregion

        #endregion

        #region (Public) Methods

        #region Parameters

        public void Register(IEnumerable<IDependencyMachineParticipant> participants)
        {
            foreach (var participant in participants) { Register(participant); }
        }

        public void Register(IDependencyMachineParticipant participant, bool isAlreadyStarted = false)
        {
            if (Definition.IsFrozen)
                if (IsStarted || IsStarting) { throw new NotImplementedException("Not implemented: registering new members after started."); }

            // TODO: LionFire.Execution integration - detect if already started

            //if (participant.StartAction == null && participant.StopAction == null) return;
            //if (isAlreadyStarted && participant.StopAction == null) return;

            if (!Definition.participants.TryAdd(participant.Key, participant)) throw new AlreadyException($"Participant with key '{participant.Key}' is already registered.");

            if (isAlreadyStarted) { OnStarted(participant, initialRegistration: true); }
            else { OnStopped(participant, initialRegistration: true); }
        }

        public bool Unregister(IDependencyMachineParticipant participant)
        {
            stoppedParticipants.TryRemove(participant.Key, out _);
            startedParticipants.TryRemove(participant.Key, out _);
            return Definition.participants.TryRemove(participant.Key, out _);
        }

        #endregion

        public IEnumerable<string> StartLog => startLog;
        private List<string> startLog;
        public IEnumerable<string> StopLog => stopLog;
        private List<string> stopLog;
        public IEnumerable<int> StartStageLog => startStageLog;
        private List<int> startStageLog;
        public IEnumerable<int> StopStageLog => stopStageLog;
        private List<int> stopStageLog;

        #region IHostedService

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            IsStarting = true;
            IsStarting = false;
            IsStarted = true;

            if (IsLoggingEnabled)
            {
                startLog = new List<string>();
                startStageLog = new List<int>();
            }

            CalculateStages();

            foreach (var stage in Stages)
            {
                foreach (var item in stage.Members)
                {
                    if (stoppedParticipants.ContainsKey(item.Key))
                    {
                        Debug.WriteLine($"Starting '{item}'");
                        await item.StartAsync(cancellationToken).ConfigureAwait(false);
                        OnStarted(item);
                    }
                }
                OnStartedStage(stage);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            IsStarted = false;

            if (IsLoggingEnabled)
            {
                stopLog = new List<string>();
                stopStageLog = new List<int>();
            }

            foreach (var stage in Stages.Reverse())
            {
                foreach (var item in stage.Members)
                {
                    if (startedParticipants.ContainsKey(item.Key))
                    {
                        Debug.WriteLine($"Stopping '{item.Key}'");
                        await item.StopAsync(cancellationToken).ConfigureAwait(false);
                        OnStopped(item);
                        //Debug.WriteLine($"Starting '{item}'");
                        //await item.StartAsync(cancellationToken).ConfigureAwait(false);
                        //OnStarted(item);
                    }
                }
                OnStoppedStage(stage);
            }
        }

        #endregion
        #endregion

        #region (Private) Methods

        public void OnStarted(IDependencyMachineParticipant participant, bool initialRegistration = false)
        {
            if (!initialRegistration) { stoppedParticipants.TryRemove(participant.Key, out _); }
            startedParticipants.AddOrUpdate(participant.Key, participant, (k, v) => participant);
            if (startLog != null) { startLog.Add(participant.Key); }
        }

        protected void OnStopped(IDependencyMachineParticipant participant, bool initialRegistration = false)
        {
            if (!initialRegistration) { startedParticipants.TryRemove(participant.Key, out _); }
            stoppedParticipants.AddOrUpdate(participant.Key, participant, (k, v) => participant);
            if (stopLog != null) { stopLog.Add(participant.Key); }
        }

        public void OnStartedStage(DependencyStage stage)
        {
            if (startStageLog != null) { startStageLog.Add(stage.Id); }
            Debug.WriteLine($"[stage {stage.Id}] started");
        }
        public void OnStoppedStage(DependencyStage stage)
        {
            if (stopStageLog != null) { stopStageLog.Add(stage.Id); }
            Debug.WriteLine($"[stage {stage.Id}] stopped");
        }

        #endregion

    }
}
