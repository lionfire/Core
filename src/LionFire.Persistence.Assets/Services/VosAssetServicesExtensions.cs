using LionFire.Assets;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;
using LionFire.Vos;
using LionFire.Vos.Assets.Persisters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Services
{
    public static class VosAssetServicesExtensions
    {
        public static IServiceCollection AddAssetPersister(this IServiceCollection services, VosAssetOptions options = null, IVosReference contextVob = null)
        {
            services.InitializeVob(contextVob, (serviceProvider, vob) =>
            {
                vob.AddOwn<IPersister<IAssetReference>>(v =>
                {
                    return (VosAssetPersister)ActivatorUtilities.CreateInstance(serviceProvider, typeof(VosAssetPersister), options);
                });
            });

            return services;
        }
    }
}
