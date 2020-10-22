using Microsoft.Extensions.DependencyInjection;
using System;

namespace LionFire.Hosting
{
    public static class HostingExtensions
    {

        /// <summary>
        /// Wraps any type in a HostedService&lt;T&gt; in IHostedService and add it via AddHostedService.
        /// The type T is a constructor parameter and will be injected by the dependency injection mechanism.
        /// IHostedService.StopAsync will dispose the instance if T implements IDisposable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAsHostedService<T>(this IServiceCollection services)
            => services.AddHostedService<HostedService<T>>();

        /// <summary>
        /// Hosts a factory Func as a HostedFactory&lt;T&gt; (which implements IHostedService) and add it via AddHostedService.
        /// IHostedService.StopAsync will dispose the instance if T implements IDisposable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHostedFactory<T>(this IServiceCollection services, Func<T> factory)
            => services.AddHostedService<HostedFactory<T>>(_ => new HostedFactory<T> { Factory = factory });

        /// <summary>
        /// Hosts a factory Func as a HostedFactory&lt;T&gt; (which implements IHostedService) and add it via AddHostedService.
        /// IHostedService.StopAsync will dispose the instance if T implements IDisposable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHostedFactory<T>(this IServiceCollection services, Func<IServiceProvider, T> factory)
            => services.AddHostedService<HostedFactory<T>>(serviceProvider => new HostedFactory<T> { Factory = () => factory(serviceProvider) });
    }
}
