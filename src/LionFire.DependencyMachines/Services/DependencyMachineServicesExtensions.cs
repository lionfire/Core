#nullable enable
using LionFire.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.DependencyMachines
{

    public static class DependencyMachineServicesExtensions
    {
        public static IServiceCollection AddDependencyMachine(this IServiceCollection services, bool addHostedService = true)
        {
            services.AddSingleton<IDependencyStateMachine>(serviceProvider => machines.GetValue(services, _ => ActivatorUtilities.CreateInstance<DependencyStateMachine>(serviceProvider)));
            if (addHostedService) { services.AddHostedService<DependencyMachineService>(); }
            return services;
        }

        //public static IServiceCollection AddHostedDependencyService<T>(this IServiceCollection services, IDependencyParticipant service)
        //{
        //    //return services.TryAddEnumerableSingleton<IEnumerable<IDependencyParticipant>>();
        //    return services.TryAddEnumerableSingleton<IEnumerable<IDependencyParticipant>>();
        //}

        // MEMORYLEAK?
        private static readonly ConditionalWeakTable<IServiceCollection, DependencyStateMachine> machines
            = new ConditionalWeakTable<IServiceCollection, DependencyStateMachine>();

        // TODO: Return the DependencyMachineDefinition, which (typically) gets frozen after it gets started.
        public static IDependencyStateMachine? TryGetDependencyMachine(this IServiceCollection services)
            => machines.TryGetValue(services, out DependencyStateMachine result) ? result : null;
        public static IDependencyStateMachine GetDependencyMachine(this IServiceCollection services) 
            => services.TryGetDependencyMachine() 
            ?? throw new Exception("Missing IDependencyStateMachine.  Please invoke services.AddDependencyMachine() before invoking this.");

        public static IServiceCollection ConfigureDependencyMachine(this IServiceCollection services, Action<IDependencyStateMachine> action)
        {
            action(services.GetDependencyMachine());
            return services;
        }

        //public static void AddHostedDependencyService<T>(this IServiceCollection services, T service)
        //    where T : class, IHostedService
        //{
        //    services.AddSingleton<T>(service);
        //    services.GetDependencyStateMachine().Register(new HostedServiceDependencyProvider<T>(service));
        //}

    }
}
