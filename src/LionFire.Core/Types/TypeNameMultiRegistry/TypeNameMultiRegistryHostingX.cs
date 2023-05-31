using LionFire.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class TypeNameMultiRegistryHostingX
{
    public static IServiceCollection AddTypeNameMultiRegistry(this IServiceCollection services
        /* FUTURE bool autoscan = true */
        )
    {
        services.AddSingleton<ITypeNameMultiRegistry, TypeNameMultiRegistry>();

        services.PostConfigure<TypeNameMultiRegistryOptions>(options =>
        {
            if (options.AutoRegisterFullNames || options.AutoRegisterNames)
            {
                foreach (var registry in options.Registries.Where(kvp => kvp.Key != TypeNameRegistryNames.FullName && kvp.Key != TypeNameRegistryNames.Name).Select(kvp => kvp.Value).ToArray())
                {
                    foreach (var kvp in registry.TypeNames)
                    {
                        if (options.AutoRegisterFullNames)
                        {
                            options[TypeNameRegistryNames.FullName].RegisterFullName(kvp.Key);
                        }
                        if (options.AutoRegisterNames)
                        {
                            options[TypeNameRegistryNames.Name].RegisterName(kvp.Key);
                        }
                    }
                }
            }
        });
        return services;
    }
}
