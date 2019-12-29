using LionFire.Vos;
using System.Collections.Generic;

namespace LionFire.Services
{
    public class VosInitializer  
    {
        public VosInitializer(VosRootManager vosRootManager, IEnumerable<VobInitializer> vobInitializers)
        {
            foreach (var initializer in vobInitializers)
            {
                var rootVob = vosRootManager.Get(initializer.VobRootName);

                IVob vob = rootVob;
                if (!string.IsNullOrEmpty(initializer.VobPath)) vob = vob[initializer.VobPath];
                initializer.InitializationAction(vob);
            }
        }
    }
}


