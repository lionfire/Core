using LionFire.ExtensionMethods;
using LionFire.TypeRegistration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LionFire.Hosting;

public static class TypeRegistryHostingX
{
    /// <summary>
    /// Scans assemblies for types implementing the interface and registers them.
    /// Open generic types are registered by their base name and can be closed by callers.
    /// </summary>
    public static IServiceCollection RegisterTypesFromAssemblies<TInterface>(this IServiceCollection services, params IEnumerable<Type> typesInsideAssemblies)
        => services.RegisterTypesFromAssemblies<TInterface>(typesInsideAssemblies.Select(t => t.Assembly));

    /// <summary>
    /// Scans assemblies for types implementing the interface and registers them.
    /// Open generic types are registered by their base name and can be closed by callers.
    /// </summary>
    public static IServiceCollection RegisterTypesFromAssemblies<TInterface>(this IServiceCollection services, IEnumerable<Type> typesInsideAssemblies, Func<Type, bool>? filter = null)
        => services.RegisterTypesFromAssemblies<TInterface>(typesInsideAssemblies.Select(t => t.Assembly), filter);

    /// <summary>
    /// Scans assemblies for types implementing the interface and registers them.
    /// Open generic types are registered by their base name and can be closed by callers.
    /// </summary>
    public static IServiceCollection RegisterTypesFromAssemblies<TInterface>(this IServiceCollection services, IEnumerable<Assembly> assemblies, Func<Type, bool>? filter = null)
        => services.AddKeyedSingleton<TypeRegistry>(typeof(TInterface), (sp, t) => new TypeRegistry((Type?)t, assemblies, filter != null ? new Predicate<Type>(filter) : null));
}
