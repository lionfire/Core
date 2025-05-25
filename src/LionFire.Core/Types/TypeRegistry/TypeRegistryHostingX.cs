using LionFire.ExtensionMethods;
using LionFire.TypeRegistration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Hosting;

public static class TypeRegistryHostingX
{

    public static IServiceCollection RegisterTypesFromAssemblies<TInterface>(this IServiceCollection services, params IEnumerable<Type> typesInsideAssemblies)
        => services.RegisterTypesFromAssemblies<TInterface>(typesInsideAssemblies.Select(t => t.Assembly));

    public static IServiceCollection RegisterTypesFromAssemblies<TInterface>(this IServiceCollection services, IEnumerable<Type> typesInsideAssemblies, Func<Type, bool>? filter = null)
        => services.RegisterTypesFromAssemblies<TInterface>(typesInsideAssemblies.Select(t => t.Assembly), filter);

    public static IServiceCollection RegisterTypesFromAssemblies<TInterface>(this IServiceCollection services, IEnumerable<Assembly> assemblies, Func<Type, bool>? filter = null)
        => services.AddKeyedSingleton<TypeRegistry>(typeof(TInterface), (sp, t) => new TypeRegistry((Type?)t, assemblies));

}
