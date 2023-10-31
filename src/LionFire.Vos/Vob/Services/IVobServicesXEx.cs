using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LionFire.Flex;

namespace LionFire.Vos.Services;

public static class IVobServicesXEx
{
    public static IServiceProvider GetServiceProvider(this IVob vob)
        => vob.Acquire<IServiceProvider>();
        //=> vob.RecursiveQuery<IVob, IServiceProvider>();

}