using System;
using LionFire.DependencyInjection;
using LionFire.Vos;
using LionFire.Vos.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Services
{
    public static class VobServiceProviderExtensions
    {
        public static IVob AddServiceProvider(this IVob vob)
        {
            var vobI = vob as IVobInternals;

            var dsp = new DynamicServiceProvider();

            vobI.GetOrAddVobNode<IServiceProvider>(dsp);
            vobI.GetOrAddVobNode<IServiceCollection>(dsp);

            //vob.GetOrAddNextVobNode<IServiceProvider, VobServiceProvider>(addAtRoot: false);
            return vob;
        }
    }
}

