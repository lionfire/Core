using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Services
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection TryAddEnumerableSingleton<TService, TImplementation>(this IServiceCollection services)
        {
            services.TryAddEnumerable(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton));

            // TODO: Use a function that captures the instance after it is created and set ManualSingleton<T>.Instance to it 
            //if (AppHostBuilderSettings.SetManualSingletons)
            //{
            //    ManualSingleton<T>.Instance = implementationInstance;
            //}
            return services;
        }

        public static IServiceCollection TryAddEnumerableSingleton<TService>(this IServiceCollection services, TService instance)
        {
            services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(TService), typeof(TService), ServiceLifetime.Singleton));
            //services.TryAddEnumerable(new ServiceDescriptor(typeof(TService), instance));

            // TODO: Use a function that captures the instance after it is created and set ManualSingleton<T>.Instance to it 
            //if (AppHostBuilderSettings.SetManualSingletons)
            //{
            //    ManualSingleton<T>.Instance = implementationInstance;
            //}
            return services;
        }

        public static IServiceCollection TryAddEnumerableSingleton<TService, TImplementation>(this IServiceCollection services, TImplementation instance)
        {
            services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton));
            return services;
        }
    }
}
