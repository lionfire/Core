using LionFire.Dependencies;
using LionFire.ExtensionMethods;
using LionFire.Persistence;
using LionFire.Structures;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.DependencyMachines
{
    // ENH: Pause/unpause
    // ENH: Reactive: detect started/stopped/faulted/paused states on members

    public class DependencyStateMachine : IDependencyStateMachine 
    {
        #region Parameters
        public bool AllowRoundRobinWithinStageWhenMissingDependencies { get; set; } = false; // true not implemented

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

        NamedObjectsByType ObjectRegistry = new NamedObjectsByType();

        #region Convenience

        public IEnumerable<IParticipant> Participants => Definition.Participants;

        #endregion

        #region State

        // TODO: State machine / some sort of IExecutor interface: Started Stopped / Starting Stopping
        public bool IsStarted { get; private set; }
        public bool IsStarting { get; private set; }

        public IEnumerable<IParticipant> StartedParticipants => startedParticipants.Values;
        ConcurrentDictionary<string, IParticipant> startedParticipants { get; } = new ConcurrentDictionary<string, IParticipant>();
        public IEnumerable<IParticipant> StoppedParticipants => stoppedParticipants.Values;
        ConcurrentDictionary<string, IParticipant> stoppedParticipants { get; } = new ConcurrentDictionary<string, IParticipant>();

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

        public IDependencyStateMachine Register(IEnumerable<IParticipant> participants)
        {
            foreach (var participant in participants) { Register(participant); }
            return this;
        }

        public IDependencyStateMachine Register(IParticipant participant, bool isAlreadyStarted = false)
        {
            if (Definition.IsFrozen)
                if (IsStarted || IsStarting) { throw new NotImplementedException("Not implemented: registering new members after started."); }

            // TODO: LionFire.Execution integration - detect if already started

            //if (participant.StartAction == null && participant.StopAction == null) return;
            //if (isAlreadyStarted && participant.StopAction == null) return;

            if (!Definition.participants.TryAdd(participant.Key, participant)) throw new AlreadyException($"Participant with key '{participant.Key}' is already registered.");

            if (isAlreadyStarted) { OnStarted(participant, initialRegistration: true); }
            else { OnStopped(participant, initialRegistration: true); }

            return this;
        }

        public bool Unregister(IParticipant participant)
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


        private List<int> stopStageLog; // TODO DEPRECATE this - let people do it via events if they want it.

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

            var endOfStage = new List<IParticipant>();

            async Task<object?> start(IParticipant item, CancellationToken cancellationToken2)
            {
                if (stoppedParticipants.ContainsKey(item.Key))
                {
                    OnStarting(item);
                    if (item is ITryStartable ts)
                    {
                        try
                        {
                            var result = await ts.TryStartAsync(cancellationToken2).ConfigureAwait(false);
                            if (result != null) return result;
                        }
                        catch (Exception ex)
                        {
                            return ex;
                        }
                    }
                    OnStarted(item);
                }
                return null;
            }

            foreach (var stage in Stages)
            {
                endOfStage.Clear();

                foreach (var item in stage.Members)
                {
                    if (item.Flags.HasFlag(ParticipantFlags.StageEnder))
                    {
                        endOfStage.Add(item);
                        continue;
                    }
                    if (!InjectMissingDependencies(item))
                    {
                        if(AllowRoundRobinWithinStageWhenMissingDependencies)
                        {
                            throw new NotImplementedException();
                        } else
                        {
                            throw new DependencyMissingException($"Participant '{item}' has missing dependencies: " + item.DependencyHandles.Where(h => !h.HasValue).Select(h=>h.Key).Aggregate((x, y) => $"{x}, {y}"));
                        }
                    }
                    await start(item, cancellationToken).ConfigureAwait(false);
                }
                foreach (var item in endOfStage) { await start(item, cancellationToken).ConfigureAwait(false); }
                OnStartedStage(stage);
            }
        }

        protected bool InjectMissingDependencies(IParticipant participant)
        {
            var hasUnresolved = false;
            foreach(var handle in participant.DependencyHandles.Where(h => !h.HasValue))
            {
                var obj = ObjectRegistry.Get(handle.Type, handle.Reference.Path);
                if (obj != null)
                {
                    handle.SetValue(obj);
                }
                else {
                    hasUnresolved = true;
                }
            }
            return hasUnresolved;
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            IsStarted = false;

            if (IsLoggingEnabled)
            {
                stopLog = new List<string>();
                stopStageLog = new List<int>();
            }

            async Task<object?> stop(IParticipant item, CancellationToken cancellationToken2)
            {
                if (stoppedParticipants.ContainsKey(item.Key))
                {
                    OnStopping(item);
                    if (item is ITryStoppable ts)
                    {
                        try
                        {
                            var result = await ts.TryStopAsync(cancellationToken2).ConfigureAwait(false);
                            if (result != null) return result;
                        }
                        catch (Exception ex)
                        {
                            return ex;
                        }
                    }
                    OnStopped(item);
                }
                return null;
            }
            foreach (var stage in Stages.Reverse())
            {
                foreach (var item in stage.Members)
                {
                    if (startedParticipants.ContainsKey(item.Key))
                    {
                        OnStopping(item);
                        await stop(item, cancellationToken).ConfigureAwait(false);
                        OnStopped(item);
                    }
                }
                OnStoppedStage(stage);
            }
        }

        #endregion
        #endregion

        #region Events

        public event Action<IParticipant> Starting;
        public event Action<IParticipant> Started;
        public event Action<string> StartedStage;
        public event Action<string> StoppedStage;
        public event Action<IParticipant> Stopping;
        public event Action<IParticipant, bool /* initialRegistration */> Stopped;

        #endregion
        #region (Private) Methods

        public void OnStarting(IParticipant participant)
        {
            Starting?.Invoke(participant);
        }
        public void OnStopping(IParticipant participant)
        {
            Stopping?.Invoke(participant);
        }
        public void OnStarted(IParticipant participant, bool initialRegistration = false)
        {
            if (!initialRegistration) { stoppedParticipants.TryRemove(participant.Key, out _); }
            startedParticipants.AddOrUpdate(participant.Key, participant, (k, v) => participant);
            if (startLog != null) { startLog.Add(participant.Key); }
            Started?.Invoke(participant);
        }

        protected void OnStopped(IParticipant participant, bool initialRegistration = false)
        {
            if (!initialRegistration) { startedParticipants.TryRemove(participant.Key, out _); }
            stoppedParticipants.AddOrUpdate(participant.Key, participant, (k, v) => participant);
            if (stopLog != null) { stopLog.Add(participant.Key); }
            Stopped?.Invoke(participant, initialRegistration);
        }

        public void OnStartedStage(DependencyStage stage)
        {
            if (startStageLog != null) { startStageLog.Add(stage.Id); }
            StartedStage?.Invoke(stage.Key);
        }
        public void OnStoppedStage(DependencyStage stage)
        {
            if (stopStageLog != null) { stopStageLog.Add(stage.Id); }
            StoppedStage?.Invoke(stage.Key);
        }

        #endregion

    }
}
