using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Structures;
using System.Reflection;
using LionFire.Instantiating;
using LionFire.Types;
using LionFire.Execution;

namespace LionFire.Applications.Hosting
{
    public static class InstantiatingAppHostExtensiosn
    {
        
        public static IAppHost AddDataAssembly(this IAppHost host, Assembly assembly)
        {
            //host.ConfigureServices(serviceCollection =>
            //{
            //    //serviceCollection.Select(sd => sd.ImplementationInstance).OfType<TypeNamingContext>().FirstOrDefault();
            //});

            var tnc = ManualSingleton<TypeNamingContext>.GuaranteedInstance;

            if (tnc != null && tnc.UseShortNamesForDataAssemblies)
            {
                if (tnc != null)
                {
                    tnc.UseShortNamesForAssembly(assembly);
                }
            }

            foreach (var type in assembly.ExportedTypes)
            {
                if (type.GetTypeInfo().IsAbstract || type.GetTypeInfo().IsInterface) continue;

                tnc.Register(type.FullName, type);
            }

            return host;
        }
    }
}
