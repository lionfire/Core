using System;
using LionFire.DependencyInjection;
using LionFire.Vos;
using LionFire.Vos.Internals;

namespace LionFire.Services
{
    public static class VobServiceProviderExtensions
    {
        public static IVob AddServiceProvider(this IVob vob)
        {
            var vobI = vob as IVobInternals;
            var node = vobI.GetOrAddVobNode<IServiceProvider, DynamicServiceProvider>();

            //vob.GetOrAddNextVobNode<IServiceProvider, VobServiceProvider>(addAtRoot: false);
            return vob;
        }
    }
}

