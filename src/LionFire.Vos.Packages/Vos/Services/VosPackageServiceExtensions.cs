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
        public static IVob AddPackageManager(this IVob vob, VobPackageManagerOptions options = null)
        {
            var mt = vob as IMultiTypable;
            mt.AsTypeOrCreate<VobPackageManager>(() => new VobPackageManager(vob, options));

            //vob.MultiTyped().AsTypeOrCreate<VobPackageManager>()
            //vob.GetOrAddOwn<IServiceCollection>()
            //    .AddSingleton<VobPackageManager>(new VobPackageManager(vob, options))
            //    ;
            return vob;
        }
    }
}
