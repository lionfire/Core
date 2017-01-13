using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Structures;
using System.Reflection;
using LionFire.Instantiating;
using LionFire.Types;

namespace LionFire.Applications.Hosting
{
    public static class InstantiatingAppHostExtensiosn
    {
        public static IAppHost AddDataAssembly(this IAppHost host, Assembly assembly)
        {
            var tnc = ManualSingleton<TypeNamingContext>.Instance;

            if (tnc != null && tnc.UseShortNamesForDataAssemblies)
            {
                if (tnc != null)
                {
                    tnc.UseShortNamesForAssembly(assembly);
                }
            }

            return host;
        }
    }
}
