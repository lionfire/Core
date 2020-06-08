using LionFire.Applications;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Services
{
    public static class LionFireAppServicesExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services
                .AddSingleton<ILionFireApp, LionFireApp>()
                .AddHostedService<ApplicationTelemetry>()
                ;

            return services;
        }
    }
}
