#nullable enable
using LionFire.DependencyMachines.Abstractions;
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
        public static IServiceCollection AddDependencyMachine(this IServiceCollection services, string? name = null, bool addHostedService = true)
        {
            if (name == null)
            {
                services.AddSingleton<IDependencyStateMachine>(serviceProvider =>
                     machines.GetValue(services, _ =>
                         ActivatorUtilities.CreateInstance<DependencyStateMachine>(serviceProvider,
                            name == null ? Array.Empty<object>() : new object[] { name })
                ));
                if (addHostedService) { 
                    services.AddHostedService<DependencyMachineService>(); 
                }
            }
            else
            {
                // register as named singleton  FUTURE
               // services.AddNamedSingleton<IDependencyStateMachine>(serviceProvider =>
               //     machines.GetValue(services, _ =>
               //         ActivatorUtilities.CreateInstance<DependencyStateMachine>(serviceProvider,
               //            name == null ? Array.Empty<object>() : new object[] { name })
               //));
                throw new NotImplementedException();
            }
            
            return services;
        }

        //public static IServiceCollection AddHostedDependencyService<TValue>(this IServiceCollection services, IDependencyParticipant service)
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

        public static IServiceCollection ConfigureDependencyMachine(this IServiceCollection services, Action<DependencyMachineConfig> action, string? name = null)
        {
            if (name != null)
            {
                services.Configure<DependencyMachineConfig>(name, action);
            }
            else
            {
                services.Configure<DependencyMachineConfig>(action);
            }
            return services;
        }

        //public static void AddHostedDependencyService<TValue>(this IServiceCollection services, TValue service)
        //    where TValue : class, IHostedService
        //{
        //    services.AddSingleton<TValue>(service);
        //    services.GetDependencyStateMachine().Register(new HostedServiceDependencyProvider<TValue>(service));
        //}

    }
}
