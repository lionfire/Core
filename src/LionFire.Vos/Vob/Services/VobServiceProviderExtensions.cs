using System;
using LionFire.DependencyInjection;
using LionFire.Vos;
using LionFire.Vos.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace LionFire.Services
{
    public static class VobServiceProviderExtensions
    {
        public static IVob AddServiceProvider(this IVob vob, Action<IServiceCollection> configurator = null, IServiceProvider parentServiceProvider = null, bool allowDiscardExistingServiceProvider = false)
        {
            var vobI = vob as IVobInternals;

            var existingServiceProvider = vobI.TryAcquireOwnVobNode<IServiceProvider>()?.Value;

            var dsp = new DynamicServiceProvider();

            vobI.GetOrAddVobNode<IServiceProvider>(dsp);
            vobI.GetOrAddVobNode<IServiceCollection>(dsp);

            
            configurator?.Invoke(dsp);

            //vob.AcquireOrAddNextVobNode<IServiceProvider, VobServiceProvider>(addAtRoot: false);
            if(parentServiceProvider != null && existingServiceProvider != null && !allowDiscardExistingServiceProvider)
            {
                throw new ArgumentException($"{nameof(parentServiceProvider)} != null && {nameof(vob)}.TryAcquireOwnVobNode<IServiceProvider>()?.Value != null && !{nameof(allowDiscardExistingServiceProvider)}");
            }
            dsp.Parent = parentServiceProvider ?? existingServiceProvider;
            return vob;
        }
    }
}

