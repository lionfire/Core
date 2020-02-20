using LionFire.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LionFire.Services
{
    public static class TypeNameRegistryServicesExtensions
    {
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
            => services
                    .Configure((TypeNameRegistry r) => r.Types.Add(name ?? type.Name, type));

                //=> services
                //          .AddSingleton(new TypeNameRegistryInitializer(new Dictionary<string, Type>
                //          {
                //              [name ?? type.Name] = type,
                //          }));

        public static IServiceCollection RegisterTypeNames(this IServiceCollection services, Assembly assembly, bool exportedTypesOnly = true, Func<Type, string> selector = null, Func<Type, bool> filter = null, bool concreteTypesOnly = true)
        {
            if (selector == null) selector = t => null; // Uses a default in RegisterTypeName
            if (filter == null) filter = t => true;

            foreach (var type in (exportedTypesOnly ? assembly.GetExportedTypes() : assembly.GetTypes()).Where(filter))
            {
                if (concreteTypesOnly && (type.GetTypeInfo().IsAbstract || type.GetTypeInfo().IsInterface)) continue;
                services.RegisterTypeName(type, selector(type));
            }
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
}
