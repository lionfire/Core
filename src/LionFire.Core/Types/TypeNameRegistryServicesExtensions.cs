using LionFire.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LionFire.Hosting;

public static class TypeNameRegistryServicesExtensions
{
    public static IServiceCollection AddTypeNameRegistry(this IServiceCollection services)
        => services
            .AddSingleton<ITypeResolver, TypeResolver>()
            .AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptionsMonitor<TypeNameRegistry>>().CurrentValue)
            ;

    //public static IServiceCollection RegisterTypeNames(this IServiceCollection services, Assembly assembly, Predicate<Type> filter = null, bool registerShortNames = true, bool publicTypesOnly = true, bool concreteTypesOnly = true)
    //{
    //    services
    //        .Configure((TypeNameRegistry r) =>
    //        {
    //            foreach (var type in publicTypesOnly ? assembly.ExportedTypes : assembly.GetTypes())
    //            {
    //                if (concreteTypesOnly && (type.GetTypeInfo().IsAbstract || type.GetTypeInfo().IsInterface)) continue;
    //                if (filter != null && !filter(type)) continue;
    //                r.Types.Add(registerShortNames ? type.Name : type.FullName, type);
    //            }
    //        })
    //        .AddSingleton(serviceProvider => serviceProvider.GetService<IOptionsMonitor<TypeNameRegistry>>().CurrentValue)
    //        .AddSingleton<ITypeResolver, TypeResolver>()
    //    ;
    //    return services;
    //}

    public static Func<Type, string> DefaultTypeNameSelector { get; set; } = t => t.Name;


    public static IServiceCollection RegisterTypeName<T>(this IServiceCollection services, string name = null)
        => services
                .Configure((TypeNameRegistry r) => r.Types.Add(name ?? DefaultTypeNameSelector(typeof(T)), typeof(T)));
    //.AddSingleton(new TypeNameRegistryInitializer(new Dictionary<string, Type>
    //{
    //    [name ?? typeof(T).Name] = typeof(T),
    //}));

    public static IServiceCollection RegisterTypeName(this IServiceCollection services, Type type, string name = null)
        => RegisterTypeNames(services, new(Type type, string? name)[] { (type, name) });

    public static IServiceCollection RegisterTypeNames(this IServiceCollection services, IEnumerable<(Type type, string? name)> list)
        => services
                .Configure((TypeNameRegistry r) =>
                {
                    foreach (var x in list)
                    {
                        var type = x.type;
                        var name = x.name;

                        var key = name ?? type.Name;
                        if (r.Types.ContainsKey(key))
                        {
                            if (r.Types[key].FullName != type.FullName) throw new AlreadyException($"Type name {key} is already registered with a different type: {r.Types[key].FullName}.  Cannot register as {type.FullName}");
                        }
                        else if (r.TypeNames.ContainsKey(type))
                        {
                            if (r.TypeNames[type] != key) throw new AlreadyException($"Type {type} is already registered with a different key: {r.TypeNames[type]}.  Cannot register as {key}");
                        }
                        else
                        {
                            r.Types.Add(key, type);
                            r.TypeNames.Add(type, key);
                        }
                    }
                });

    //=> services
    //          .AddSingleton(new TypeNameRegistryInitializer(new Dictionary<string, Type>
    //          {
    //              [name ?? type.Name] = type,
    //          }));

    public static IServiceCollection RegisterTypeNames(this IServiceCollection services, Assembly assembly, bool exportedTypesOnly = true, Func<Type, string> selector = null, Func<Type, bool> filter = null, bool abstractTypes = false, bool genericTypes = false, bool interfaceTypes = false)
    {
        if (selector == null) selector = t => null; // Uses a default in RegisterTypeName
        if (filter == null) filter = t => true;


        var list = new List<(Type, string?)>();
        foreach (var type in (exportedTypesOnly ? assembly.GetExportedTypes() : assembly.GetTypes()).Where(filter))
        {
            if (!abstractTypes && type.IsAbstract && !type.IsInterface) continue;
            if (!genericTypes && type.ContainsGenericParameters) continue;
            if (!interfaceTypes && type.IsInterface) continue;
            list.Add((type, selector(type)));
        }

        services.RegisterTypeNames(list);
        return services;
    }

    public static IServiceCollection RegisterTypesNamesWithAttribute<T>(this IServiceCollection services,
        Assembly assembly,
        bool exportedTypesOnly = true,
        Func<Type, string> selector = null,
        Func<Type, bool> filter = null,
        bool inheritAttribute = false)
        where T : Attribute
        => services.RegisterTypeNames(assembly, exportedTypesOnly: exportedTypesOnly, selector: selector, filter: t => t.GetCustomAttribute<T>(inheritAttribute) != null);
}
