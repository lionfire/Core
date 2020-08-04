using LionFire.Dependencies;
using LionFire.DependencyMachines.Abstractions;
using LionFire.ExtensionMethods;
using LionFire.ExtensionMethods.Collections;
using LionFire.Persistence;
using LionFire.Services.DependencyMachines;
using LionFire.Structures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.DependencyMachines
{
    // ENH: Pause/unpause
    // ENH: Reactive: detect started/stopped/faulted/paused states on members

    public class DependencyStateMachine : IDependencyStateMachine
    {
        #region Dependencies

        public DependencyMachineConfig InjectedConfig => Name == null ? OptionsMonitor.CurrentValue : OptionsMonitor.Get(Name);
        public IOptionsMonitor<DependencyMachineConfig> OptionsMonitor { get; }
        public string Name { get; }
        public ILogger<DependencyStateMachine> Logger { get; }
        public IServiceProvider ServiceProvider { get; set; }

        #endregion

        #region Parameters

        public bool AllowRoundRobinWithinStageWhenMissingDependencies { get; set; } = false; // true not implemented

        public DependencyMachineConfig EffectiveConfig => Config ?? InjectedConfig;

        public DependencyMachineConfig? Config
        {
            get => config;
            set
            {
                if (ReferenceEquals(config, value)) return;
                if (IsStarting || IsStarted)
                {
                    throw new Exception("Cannot change config after start");
                }
                config = value;
            }
        }
        private DependencyMachineConfig? config;

        public DependencyMachineConfig? ActiveConfig => compiled?.Config;

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

        #region Construction

        public DependencyStateMachine(IServiceProvider serviceProvider, IOptionsMonitor<DependencyMachineConfig> optionsMonitor, ILogger<DependencyStateMachine> logger, string name = null)
        {
            ServiceProvider = serviceProvider;
            OptionsMonitor = optionsMonitor;
            Name = name;
            Logger = logger;
            if (InjectedConfig.EnableLogging)
            {
                ActivatorUtilities.CreateInstance<DependencyMachineLogger>(serviceProvider, this);
            }
        }

        #endregion

        NamedObjectsByType ObjectRegistry = new NamedObjectsByType();

        #region Convenience

        public IEnumerable<IParticipant> Participants => compiled.Participants;

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

        CompiledDependencyMachine? compiled;
        protected void CalculateStages()
        {
            compiled = new CompiledDependencyMachine(ServiceProvider, EffectiveConfig);
            IsLoggingEnabled = ActiveConfig!.EnableLogging;
        }

        protected IEnumerable<DependencyStage> Stages => compiled.Stages;

        #endregion

        #endregion

        #region (Public) Methods

        #region Parameters

        //public IDependencyStateMachine Register(IEnumerable<IParticipant> participants)
        //{
        //    foreach (var participant in participants) { Register(participant); }
        //    return this;
        //}

        //public IDependencyStateMachine Register(IParticipant participant, bool isAlreadyStarted = false)
        //{
        //    //if (ActiveConfig.IsFrozen)
        //        //if (IsStarted || IsStarting) { throw new NotImplementedException("Not implemented: registering new members after started."); }

        //    // TODO: LionFire.Execution integration - detect if already started

        //    //if (participant.StartAction == null && participant.StopAction == null) return;
        //    //if (isAlreadyStarted && participant.StopAction == null) return;

        //    if (!ActiveConfig.participants.TryAdd(participant.Key, participant)) throw new AlreadyException($"Participant with key '{participant.Key}' is already registered.");

        //    if (isAlreadyStarted) { OnStarted(participant, initialRegistration: true); }
        //    else { OnStopped(participant, initialRegistration: true); }

        //    return this;
        //}

        //public bool Unregister(IParticipant participant)
        //{
        //    stoppedParticipants.TryRemove(participant.Key, out _);
        //    startedParticipants.TryRemove(participant.Key, out _);
        //    return ActiveConfig.participants.TryRemove(participant.Key, out _);
        //}

        #endregion

        public IEnumerable<string> StartLog => startLog ?? Enumerable.Empty<string>();
        private List<string>? startLog;
        public IEnumerable<string> StopLog => stopLog ?? Enumerable.Empty<string>();
        private List<string>? stopLog;
        public IEnumerable<int> StartStageLog => startStageLog ?? Enumerable.Empty<int>();
        private List<int>? startStageLog;
        public IEnumerable<int> StopStageLog => stopStageLog ?? Enumerable.Empty<int>();


        private List<int> stopStageLog; // TODO DEPRECATE this - let people do it via events if they want it.

        public void Dump()
        {
            //var sb = new StringBuilder();

            Logger.LogInformation("Dependency plan:");
            foreach (var stage in this.Stages)
            {

                Logger.LogInformation($" ===== [{stage.ToString()}] =====");
                //sb.AppendLine($"- [{stage.ToString()}]");
                foreach (var member in stage.Members)
                {
                    Logger.LogInformation($"   [[ {member.ToString()} ]]");
                    foreach (var contributes in member.Dependencies ?? Enumerable.Empty<object>())
                    {
                        Logger.LogInformation($"     o {contributes}");
                    }
                    foreach (var after in member.After ?? Enumerable.Empty<object>())
                    {
                        Logger.LogInformation($"     >> {after}");
                    }
                    foreach (var provides in member.Provides ?? Enumerable.Empty<object>())
                    {
                        if (provides?.ToString() == member.ToString()) { continue; }
                        Logger.LogInformation($"     = {provides}");
                    }
                    foreach(var contributes in member.Contributes ?? Enumerable.Empty<object>())
                    {
                        Logger.LogInformation($"     + {contributes}");
                    }
                    //sb.AppendLine($"  - {member.ToString()}");
                }
            }
            Logger.LogInformation($" ===== [end of stages] =====");

            //return sb.ToString();
        }

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
            stoppedParticipants.TryAddRange(Participants.Where(p => !startedParticipants.ContainsKey(p.Key)).Select(p => new KeyValuePair<string, IParticipant>(p.Key, p)));

            //Logger.LogInformation(
            Dump();
                //);

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

            void startOrAddToList(IParticipant item, object? startResult, ref Dictionary<IParticipant, object>? list)
            {
                if (startResult != null)
                {
                    if (list == null) { list = new Dictionary<IParticipant, object>(); }
                    list.Add(item, startResult);
                }
            }

            Dictionary<IParticipant, object>? tryAgain = null;
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
                    //if (InjectMissingDependencies(item)) // FUTURE
                    //{
                    //    if (AllowRoundRobinWithinStageWhenMissingDependencies)
                    //    {
                    //        throw new NotImplementedException();
                    //    }
                    //    else
                    //    {
                    //        throw new DependencyMissingException($"Participant '{item}' has missing dependencies: " + item.DependencyHandles.Where(h => !h.HasValue).Select(h => h.Key).Aggregate((x, y) => $"{x}, {y}"));
                    //    }
                    //}

                    startOrAddToList(item, await start(item, cancellationToken).ConfigureAwait(false), ref tryAgain);
                    //var startResult = await start(item, cancellationToken).ConfigureAwait(false);
                    //if (startResult != null)
                    //{
                    //    if (tryAgain == null) { tryAgain = new List<IParticipant>(); }
                    //    tryAgain.Add(item);
                    //}
                }
                int lastTryAgainCount = -1;
                while (tryAgain.NullableAny() && lastTryAgainCount != tryAgain.Count)
                {
                    Dictionary<IParticipant, object>? newTryAgain = null;
                    foreach (var item in tryAgain.Keys!)
                    {
                        startOrAddToList(item, await start(item, cancellationToken).ConfigureAwait(false), ref newTryAgain);
                    }

                    //var startResult = await start(item, cancellationToken).ConfigureAwait(false);
                    //if (startResult != null)
                    //{
                    //    if (tryAgain == null) { tryAgain = new List<IParticipant>(); }
                    //    tryAgain.Add(item);
                    //}

                    lastTryAgainCount = tryAgain.Count;
                    tryAgain = newTryAgain;
                }
                if (tryAgain.NullableAny())
                {
                    Logger.LogError($"{tryAgain.Count()} initialization failure(s):");
                    foreach (var fail in tryAgain!)
                    {
                        Logger.LogError($" - {fail.Key} failure: {fail.Value}");
                    }
                    throw new DependenciesUnresolvableException("Failed to start participants due to validation errors.  See Data for details. ", tryAgain!);
                }
                foreach (var item in endOfStage) { 
                    var startResult = await start(item, cancellationToken).ConfigureAwait(false); 
                    if(startResult != null)
                    {
                        throw new NotImplementedException("Stage-ender returned an error result.  Retry mechanism not implemented.");
                    }
                }
                OnStartedStage(stage);
            }
        }

        // FUTURE
        //protected bool InjectMissingDependencies(IParticipant participant)
        //{
        //    var hasUnresolved = false;
        //    if (participant.DependencyHandles != null)
        //    {
        //        foreach (var handle in participant.DependencyHandles.Where(h => !h.HasValue))
        //        {
        //            var obj = ObjectRegistry.Get(handle.Type, handle.Reference.Path);
        //            if (obj != null)
        //            {
        //                handle.SetValue(obj);
        //            }
        //            else
        //            {
        //                hasUnresolved = true;
        //            }
        //        }
        //    }
        //    return hasUnresolved;
        //}

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
            StartedStage?.Invoke(stage.ToLongString());
        }
        public void OnStoppedStage(DependencyStage stage)
        {
            if (stopStageLog != null) { stopStageLog.Add(stage.Id); }
            StoppedStage?.Invoke(stage.ToLongString());
        }

        #endregion

    }
}
