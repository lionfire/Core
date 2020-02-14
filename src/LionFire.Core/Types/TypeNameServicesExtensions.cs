using System;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Structures;
using System.Reflection;
using LionFire.Instantiating;
using LionFire.Types;
using LionFire.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LionFire.Services
{
    public static class TypeNameServicesExtensions
    {
        public static IServiceCollection RegisterTypeNames(this IServiceCollection services, Assembly assembly, Predicate<Type> filter = null, bool registerShortNames = true, bool publicTypesOnly = true, bool concreteTypesOnly = true)
        {
            services
                .Configure((TypeNameRegistry r) =>
                {
                    foreach (var type in publicTypesOnly ? assembly.ExportedTypes : assembly.GetTypes())
                    {
                        if (concreteTypesOnly && (type.GetTypeInfo().IsAbstract || type.GetTypeInfo().IsInterface)) continue;
                        if (filter != null && !filter(type)) continue;
                        r.Types.Add(registerShortNames ? type.Name : type.FullName, type);
                    }
                })
                .AddSingleton(serviceProvider => serviceProvider.GetService<IOptionsMonitor<TypeNameRegistry>>().CurrentValue)
                .AddSingleton<ITypeResolver, TypeResolver>()
            ;
            return services;
        }
    }
}
