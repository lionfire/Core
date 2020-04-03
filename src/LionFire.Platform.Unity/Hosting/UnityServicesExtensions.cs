using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Services
{
    public static class UnityServicesExtensions
    { 
        public static IServiceCollection AddUnityServices(this IServiceCollection services)
        {
            services
                .AddSingleton<IRandomProvider, UnityRandomProvider>()
                ;
            return services;
        }
    }
}
