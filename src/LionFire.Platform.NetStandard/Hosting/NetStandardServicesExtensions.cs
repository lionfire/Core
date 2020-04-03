using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Services
{
    public static class NetStandardServicesExtensions
    { 
        public static IServiceCollection AddNetStandardServices(this IServiceCollection services)
        {
            services
                .AddSingleton<IRandomProvider, NetStandardRandomProvider>()
                ;
            return services;
        }
    }
}
