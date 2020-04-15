using LionFire.DependencyMachine;
using LionFire.Execution;
using LionFire.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Vos
{

    // ENH: Initialize non-root Vobs as they are needed
    // ENH: (probably another class): initialize Vobs with a recurring logic rather than one set of logic registered to one VobPath

    /// <summary>
    /// HostedService that runs List&lt;VobInitializer&gt; and starts/stops the IDependencyStateMachine
    /// </summary>
    public class VosInitializationService 
    {
        List<VobInitializer> VobInitializers { get; }

        IServiceProvider ServiceProvider { get; }

        IDependencyStateMachine DependencyStateMachine { get; }
        public IRootManager RootManager { get; }

        public VosInitializationService(IServiceProvider serviceProvider, IOptionsMonitor<List<VobInitializer>> vobInitializers, IDependencyStateMachine dependencyStateMachine, IRootManager rootManager)
        {
            ServiceProvider = serviceProvider;
            VobInitializers = vobInitializers.CurrentValue;
            DependencyStateMachine = dependencyStateMachine;
            RootManager = rootManager;

            

            foreach (var vi in VobInitializers.Where(v=>v.Reactor != null))
            {
                DependencyStateMachine.Register(vi.Reactor);
            }
        }

#error TODO: multi-dependency: RootNames.  VosOption: AutoInitRoots (otherwise, initted on first retrieve)

        public async Task Initialize(RootVob rootVob, CancellationToken cancellationToken = default)
        {
            await VobInitializers.Where(vi => vi.Reference.RootName() == rootVob.RootName && vi.Reactor != null)
                .RepeatAllUntilNull(initializer => 
                    () => Task.FromResult(initializer.InitializationAction(ServiceProvider, string.IsNullOrEmpty(initializer.Reference?.Path) ? rootVob : rootVob[initializer.Reference.Path]))).ConfigureAwait(false);

            await DependencyStateMachine.StartAsync(cancellationToken).ConfigureAwait(false);


            //foreach (var initializer in VobInitializers.Where(vi => vi.Reference.RootName() == rootVob.RootName))
            //{
            //    IVob vob = rootVob;
            //    if (!string.IsNullOrEmpty(initializer.Reference?.Path)) vob = vob[initializer.Reference.Path];
            //    initializer.InitializationAction(ServiceProvider, vob);
            //}
        }

    
    }
}

