using LionFire.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.DependencyMachine
{
    public static class DependencyMachineServicesExtensions
    {
        public static IServiceCollection AddDependencyMachine(this IServiceCollection services)
        {
            return services.AddSingleton<DependencyStateMachine>();
        }

        //public static IServiceCollection AddHostedDependencyService<T>(this IServiceCollection services, IReactor service)
        //{
        //    //return services.TryAddEnumerableSingleton<IEnumerable<IReactor>>();
        //    return services.TryAddEnumerableSingleton<IEnumerable<IReactor>>();
        //}

        private static ConditionalWeakTable<IServiceCollection, DependencyStateMachine> machines = new ConditionalWeakTable<IServiceCollection, DependencyStateMachine>();

        public static IDependencyStateMachine GetDependencyStateMachine(this IServiceCollection services)
            => machines.GetOrCreateValue(services);

        public static void AddHostedDependencyService<T>(this IServiceCollection services, T service)
            where T : class, IHostedService
        {
            services.AddSingleton<T>(service);
            services.GetDependencyStateMachine().Register(new HostedServiceDependencyProvider<T>(service));
        }

    }
}
