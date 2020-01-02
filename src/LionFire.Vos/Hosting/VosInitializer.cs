using LionFire.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

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
            foreach (var initializer in VobInitializers.Where(vi => vi.VobRootName == rootVob.RootName))
            {
                IVob vob = rootVob;
                if (!string.IsNullOrEmpty(initializer.VobPath)) vob = vob[initializer.VobPath];
                initializer.InitializationAction(ServiceProvider, vob);
            }
        }
    }
}


