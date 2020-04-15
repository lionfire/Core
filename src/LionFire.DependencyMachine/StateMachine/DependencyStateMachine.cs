using LionFire.Structures;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.DependencyMachine
{
    public class DependenyMachineDefinition : IFreezable, INotifyPropertyChanged
    {

        #region IFreezable

        public bool IsFrozen { get; private set; }
        public void Freeze() => IsFrozen = true;

        #endregion

        public IEnumerable<IReactor> Members => reactors.Values;
        ConcurrentDictionary<string, IReactor> reactors { get; } = new ConcurrentDictionary<string, IReactor>();

        public Dictionary<object, object> Dependencies = new Dictionary<object, object>();
        public void AddDependency(object dependant, object dependency)
        {
            if (IsFrozen) throw new ObjectFrozenException();
            Dependencies.Add(dependant, dependency);
            OnPropertyChanged(nameof(Dependencies));
        }
        public void AddDependency<TDependency>(object dependant) => AddDependency(dependant, typeof(TDependency));


        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

    }

    public class DependencyStateMachine : IDependencyStateMachine
    {
        #region Parameters

        public IServiceProvider ServiceProvider { get; set; }

        public DependenyMachineDefinition Definition { get; set; }

        #endregion

        #region State

        // TODO: State machine: Started Stopped / Starting Stopping
        public bool IsStarted { get; private set; }
        public bool IsStarting { get; private set; }
        
        public IEnumerable<IReactor> Started => startedReactors.Values;
        ConcurrentDictionary<string, IReactor> stoppedReactors { get; } = new ConcurrentDictionary<string, IReactor>();
        public IEnumerable<IReactor> Stopped => stoppedReactors.Values;
        ConcurrentDictionary<string, IReactor> startedReactors { get; } = new ConcurrentDictionary<string, IReactor>();

        #region Dependency Logic State

        protected void CalculateStages()
        {
            var stages = new Dictionary<string, DependencyStage>();

            Stages = stages;
        }

        protected Dictionary<string, DependencyStage> Stages { get; protected set; }

        #endregion

        #endregion

        #region (Public) Methods

        #region IHostedService

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            IsStarting = true;
            IsStarting = false;
            IsStarted = true;

            CalculateStages();

#error NEXT: Make an ordered list of stages, not dictionary.
            foreach (var stage in Stages) {
                foreach(var member in stage.Members)
                {
                    if(stoppedReactors.ContainsKey(member.Key))
                    {
#error - try to start
                    }
                }
            }

            foreach (var item in stoppedReactors)
            {
                Debug.WriteLine($"Starting '{item.Key}' ({item.Value})");
                await item.Value.StartAsync(cancellationToken).ConfigureAwait(false);
                OnStarted(item.Value);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            IsStarted = false;

            foreach (var item in startedReactors)
            {
                Debug.WriteLine($"Stopping '{item.Key}' ({item.Value})");
                await item.Value.StopAsync(cancellationToken).ConfigureAwait(false);
                OnStopped(item.Value);
            }
        }

        #endregion

        public void Register(IReactor reactor, bool isAlreadyStarted = false)
        {
            if(IsStarted || IsStarting) { throw new NotImplementedException("Not implemented: registering new members after started."); }

            // TODO: LionFire.Execution integration - detect if already started

            if (reactor.StartAction == null && reactor.StopAction == null) return;
            if (isAlreadyStarted && reactor.StopAction == null) return;

            if (!reactors.TryAdd(reactor.Key, reactor)) throw new AlreadyException($"Reactor with key '{reactor.Key}' is already registered.");

            if (isAlreadyStarted) { OnStarted(reactor, initialRegistration: true); }
            else { OnStopped(reactor, initialRegistration: true); }
        }
        public bool Unregister(IReactor reactor)
        {
            stoppedReactors.TryRemove(reactor.Key, out _);
            startedReactors.TryRemove(reactor.Key, out _);
            return reactors.TryRemove(reactor.Key, out _);
        }

        #endregion

        #region (Private) Methods

        public void OnStarted(IReactor reactor, bool initialRegistration = false)
        {
            if (!initialRegistration) { stoppedReactors.TryRemove(reactor.Key, out _); }
            startedReactors.AddOrUpdate(reactor.Key, reactor, (k, v) => reactor);
        }

        protected void OnStopped(IReactor reactor, bool initialRegistration = false)
        {
            if (!initialRegistration) { startedReactors.TryRemove(reactor.Key, out _); }
            stoppedReactors.AddOrUpdate(reactor.Key, reactor, (k, v) => reactor);

        }

        #endregion


    }
}
