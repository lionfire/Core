using System;
using LionFire.DependencyInjection;
using LionFire.Vos;

namespace LionFire.Services
{
    public static class VobServiceProviderExtensions
    {
        public static Vob AddServiceProvider(this Vob vob)
        {
            var vobI = vob as IVobInternals;
            vobI.GetOrAddVobNode<IServiceProvider, DynamicServiceProvider>();

            throw new NotImplementedException();
            //vob.GetOrAddNextVobNode<IServiceProvider, VobServiceProvider>(addAtRoot: false);
            return vob;
        }
    }
}

namespace LionFire.Vos.ExtensionMethods
{
}
