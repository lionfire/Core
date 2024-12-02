using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Hosting;

//public class EnumerableHostedServiceProxy : IHostedService
//{
//    public Task StartAsync(CancellationToken cancellationToken)
//    {
//        throw new NotImplementedException();
//    }

//    public Task StopAsync(CancellationToken cancellationToken)
//    {
//        throw new NotImplementedException();
//    }
//}

public static class IServiceCollectionX
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

    public static IServiceCollection TryAddEnumerableSingleton<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> factory)
    {
        services.TryAddEnumerable(new ServiceDescriptor(typeof(TService), factory, ServiceLifetime.Singleton));

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

    public static IServiceCollection TryAddEnumerableHostedSingleton<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, IHostedService, TService
    {
        services.AddSingleton<TImplementation>();
        services.AddHostedService(sp => sp.GetRequiredService<TImplementation>());
        Func<IServiceProvider, TImplementation> x = sp => sp.GetRequiredService<TImplementation>();
        services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(TService), x, ServiceLifetime.Singleton));

        //services.TryAddEnumerable(ServiceDescriptor.Singleton<TService>(sp => sp.GetRequiredService<TImplementation>()));

        //services.AddHostedService<EnumerableHostedServiceProxy>();

        //services.AddHostedService<TService>(sp => sp.GetRequiredService<IEnumerable<TService>>().Where(Services => Services.GetType() == TImplementation).Single());

        return services;
    }

    public static IServiceCollection AddHostedSingleton<TService, TImplementation>(this IServiceCollection services)
      where TService : class
      where TImplementation : class, IHostedService, TService
    {
        services.AddSingleton<TService, TImplementation>();
        services.AddHostedService(sp => (TImplementation)sp.GetRequiredService<TService>());
        return services;
    }
    public static IServiceCollection AddHostedSingleton<TImplementation>(this IServiceCollection services)
        where TImplementation : class, IHostedService
    {
        services.AddSingleton<TImplementation>();
        services.AddHostedService(sp => sp.GetRequiredService<TImplementation>());
        return services;
    }

    public static IServiceCollection AddHostedKeyedSingleton<TService>(this IServiceCollection services, object key, Func<IServiceProvider, object?, TService> factory)
    where TService : class, IHostedService
    {
        services.AddKeyedSingleton<TService>(key, factory);
        services.AddHostedService(sp => sp.GetRequiredKeyedService<TService>(key));
        return services;
    }

    public static IServiceCollection AddHostedKeyedSingleton<TService, TImplementation>(this IServiceCollection services, object key)
        where TService : class, IHostedService
        where TImplementation : class, TService, IHostedService
    {
        services.AddKeyedSingleton<TService, TImplementation>(key);
        services.AddHostedService(sp => sp.GetRequiredKeyedService<TService>(key));
        return services;
    }

    //public static IServiceCollection AddEnumerableSingleton<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, TService> factory)
    //{
    //    services.TryAddEnumerable(new ServiceDescriptor(typeof(TService), sp => factory(sp), ServiceLifetime.Singleton)
    //    {
    //        ImplementationType = typeof(TImplementation),
    //    });

    //    //services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(TService), sp => factory(sp), ServiceLifetime.Singleton));
    //    //services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(TService), typeof(TService), ServiceLifetime.Singleton));
    //    //services.TryAddEnumerable(new ServiceDescriptor(typeof(TService), instance));

    //    // TODO: Use a function that captures the instance after it is created and set ManualSingleton<T>.Instance to it 
    //    //if (AppHostBuilderSettings.SetManualSingletons)
    //    //{
    //    //    ManualSingleton<T>.Instance = implementationInstance;
    //    //}
    //    return services;
    //}

    public static IServiceCollection TryAddEnumerableSingleton<TService, TImplementation>(this IServiceCollection services, TImplementation instance)
    {
        services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton));
        return services;
    }

    public static IServiceCollection If(this IServiceCollection services, bool condition, Action<IServiceCollection> action)
    {
        if (condition)
        {
            action(services);
        }
        return services;
    }

    #region IHostedService

    public static IServiceCollection AddSingletonHostedService<T>(this IServiceCollection services)
        where T : class, IHostedService
        => services
            .AddSingleton<T>()
            .AddHostedService<T>(sp => sp.GetRequiredService<T>());

    #endregion
}
