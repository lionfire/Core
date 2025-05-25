using LionFire.TypeRegistration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Types;

public static class TypeRegistryX
{
    public static Type? GetTypeFromName<TInterface>(this IServiceProvider sp, string name) => sp.GetRequiredService<IOptionsSnapshot<TypeRegistry>>().Get(typeof(TInterface).FullName).GetTypeFromName(name);
    
    //public static TypeRegistry GetTypeRegistry(this IServiceProvider sp, string name)
    //{
    //    return sp.GetRequiredService<TypeRegistry>().GetOrAdd(name);
    //}
}
