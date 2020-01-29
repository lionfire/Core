using LionFire.Execution;
using LionFire.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Vos
{
    // ENH: Initialize non-root Vobs as they are needed
    // ENH: (probably another class): initialize Vobs with a recurring logic rather than one set of logic registered to one VobPath

    /// <summary>
    /// Used by VosRootManager to initialize RootVobs and their children
    /// </summary>
    public class VosInitializer
    {
        List<VobInitializer> VobInitializers { get; }

        IServiceProvider ServiceProvider { get; }
        public VosInitializer(IServiceProvider serviceProvider, IOptionsMonitor<List<VobInitializer>> vobInitializers)
        {
            ServiceProvider = serviceProvider;
            VobInitializers = vobInitializers.CurrentValue;
        }

        public void Initialize(RootVob rootVob)
        {
            VobInitializers.Where(vi => vi.Reference.RootName() == rootVob.RootName).RepeatAllUntilNull(initializer => 
            () => Task.FromResult(initializer.InitializationAction(ServiceProvider, string.IsNullOrEmpty(initializer.Reference?.Path) ? rootVob : rootVob[initializer.Reference.Path])));

            //foreach (var initializer in VobInitializers.Where(vi => vi.Reference.RootName() == rootVob.RootName))
            //{
            //    IVob vob = rootVob;
            //    if (!string.IsNullOrEmpty(initializer.Reference?.Path)) vob = vob[initializer.Reference.Path];
            //    initializer.InitializationAction(ServiceProvider, vob);
            //}
        }
    }
}

