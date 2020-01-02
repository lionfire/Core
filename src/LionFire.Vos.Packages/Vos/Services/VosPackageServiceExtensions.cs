using LionFire.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using LionFire.Vos;
using LionFire.Vos.Services;
using LionFire.Vos.Packages;
using LionFire.Vos.Internals;
using LionFire.MultiTyping;

namespace LionFire.Services
{
    public static class VosPackageServiceExtensions
    {
        public static IVob AddPackageManager(this IVob vob, VosPackageManagerOptions options = null)
        {
            var mt = vob as IMultiTypable;
            mt.AsTypeOrCreate<VosPackageManager>(() => new VosPackageManager(vob, options));

            //vob.MultiTyped().AsTypeOrCreate<VosPackageManager>()
            //vob.GetOrAddOwn<IServiceCollection>()
            //    .AddSingleton<VosPackageManager>(new VosPackageManager(vob, options))
            //    ;
            return vob;
        }
    }
}
