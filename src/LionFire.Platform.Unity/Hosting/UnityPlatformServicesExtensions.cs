using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Services
{
    public static class UnityPlatformServicesExtensions
    {
        //public static IServiceCollection AddUnityEngineServices(this IServiceCollection services)
        //{
        //    services
        //        .AddSingleton<IRandomProvider, UnityRandomProvider>()
        //        ;
        //    return services;
        //}

        //public static IHostBuilder AddUnityEngine(this IHostBuilder hostBuilder)
        //           => hostBuilder.ConfigureServices((_, services) => services.AddUnityEngine());


        public static IServiceCollection AddUnityEngine(this IServiceCollection services)
            => services
            .AddSingleton<IRandomProvider, UnityRandomProvider>()
            ;
    }

}
