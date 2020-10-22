using LionFire.Referencing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Hosting
{
    public static class ReferenceProviderServicesExtensions
    {
        public static IServiceCollection AddReferenceProvider(this IServiceCollection services)
        {
            return services
                .AddSingleton<IReferenceProviderService, ReferenceProviderService>()
                ;
        }
    }
}
