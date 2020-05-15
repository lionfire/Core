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

        //public DependencyMachineConfig CurrentConfig => Name == null ? OptionsMonitor.CurrentValue : OptionsMonitor.Get(Name);
        public DependencyMachineConfig ManualConfig { get; private set; }

        public DependencyMachineService(IServiceProvider serviceProvider, IDependencyStateMachine dependencyMachine, IOptionsMonitor<DependencyMachineConfig> optionsMonitor, string? name = null)
        {
            ServiceProvider = serviceProvider;
            DependencyMachine = dependencyMachine;
            OptionsMonitor = optionsMonitor;

            Name = name;
            //config.OnChange((c, name) => { if(name == this.Name) { Reconfigure(); } });
        }

        protected IServiceProvider ServiceProvider { get; }
        public IDependencyStateMachine DependencyMachine { get; }
        public IOptionsMonitor<DependencyMachineConfig> OptionsMonitor { get; }
        public string? Name { get; }

        // TODO: Use ClassStateMachine to track state?
        bool isStarted = false; 
        bool isStopped = false;

        public void AddFromObject(object obj)
        {
            if (obj is IHas<IParticipant> hasParticipant)
            {
                ManualConfig.Register(hasParticipant.Object);
            }
            if (obj is IHasMany<IParticipant> hasParticipants)
            {
                ManualConfig.Register(hasParticipants.Objects);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            ManualConfig = Name == null ? OptionsMonitor.CurrentValue : OptionsMonitor.Get(Name);

            if (isStarted) throw new AlreadyException();
            isStarted = true;

            //foreach(var item in ManualConfig.AutoRegisterFromServiceTypes.OrEmpty())
            //{
            //    var service = ServiceProvider.GetService(item);
            //    AddFromObject(service);
            //}
            
            foreach (var item in ManualConfig.AutoRegisterParticipants.OrEmpty())
            {
                throw new Exception("FIXME: Move these to CompiledDependencyMachine");
                var participant = item(ServiceProvider);
                if (participant != null) { ManualConfig.Register(participant); }
            }
            foreach (var item in ManualConfig.AutoRegisterManyParticipants.OrEmpty())
            {
                throw new Exception("FIXME: Move these to CompiledDependencyMachine");
                var participants = item(ServiceProvider);
                if (participants != null) { ManualConfig.Register(participants); }
            }

            //DependencyMachine.Config = ManualConfig;

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