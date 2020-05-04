using LionFire.ExtensionMethods;
using LionFire.Ontology;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.DependencyMachines
{
    // ENH: Fluent API

    /// <summary>
    /// Starts/Stops the DependencyStateMachine, with support for IOptionsMonitor&lt;DependencyMachineConfig&gt;
    /// This is responsible for:
    ///  - passing the DependencyMachineConfig
    /// </summary>
    public class DependencyMachineService : IHostedService
    {

        public DependencyMachineService(IServiceProvider serviceProvider, IDependencyStateMachine dependencyMachine, IOptionsMonitor<DependencyMachineConfig> config)
        {
            ServiceProvider = serviceProvider;
            DependencyMachine = dependencyMachine;
            Config = config;
            //config.OnChange((c, name) => { });
        }

        protected IServiceProvider ServiceProvider { get; }
        public IDependencyStateMachine DependencyMachine { get; }
        public IOptionsMonitor<DependencyMachineConfig> Config { get; }

        // TODO: Use ClassStateMachine to track state?
        bool isStarted = false; 
        bool isStopped = false;

        public void AddFromObject(object obj)
        {
            if (obj is IHas<IParticipant> hasParticipant)
            {
                DependencyMachine.Register(hasParticipant.Object);
            }
            if (obj is IHasMany<IParticipant> hasParticipants)
            {
                DependencyMachine.Register(hasParticipants.Objects);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (isStarted) throw new AlreadyException();
            isStarted = true;

            foreach(var item in Config.CurrentValue.AutoRegisterFromServiceTypes.OrEmpty())
            {
                var service = ServiceProvider.GetService(item);
                AddFromObject(service);
            }
            foreach (var item in Config.CurrentValue.AutoRegisterParticipants.OrEmpty())
            {
                var participant = item(ServiceProvider);
                if (participant != null) { DependencyMachine.Register(participant); }
            }
            foreach (var item in Config.CurrentValue.AutoRegisterManyParticipants.OrEmpty())
            {
                var participants = item(ServiceProvider);
                if (participants != null) { DependencyMachine.Register(participants); }
            }

            await DependencyMachine.StartAsync(cancellationToken);
            isStopped = false;
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (isStopped) throw new AlreadyException();
            isStopped = true;

            await DependencyMachine.StopAsync(cancellationToken);
            isStarted = false;
        }
    }
}