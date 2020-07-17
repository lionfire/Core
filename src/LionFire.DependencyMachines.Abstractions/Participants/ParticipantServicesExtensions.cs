#nullable enable
using LionFire.DependencyMachines;
using LionFire.Ontology;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Services.DependencyMachines
{
    public static class ParticipantServicesExtensions
    {

        #region Configure

        #region Custom Participant type

        //public static IServiceCollection AddParticipant<T>(this IServiceCollection services, Action<T> configure, params object[] constructorParameters)
        //    where T : IParticipant
        //    => services.AddParticipant(null, configure, constructorParameters);

        public static IServiceCollection AddParticipant<T>(this IServiceCollection services, Action<T> configure, params object[] constructorParameters)
            where T : IParticipant
        {
            services.Configure<DependencyMachineConfig>(c => c.InjectedParticipants.Add(serviceProvider =>
            {
                var obj = ActivatorUtilities.CreateInstance<T>(serviceProvider, constructorParameters);
                obj.Key = null;
                configure(obj);
                return obj;
            }));

            //services.TryAdd(new ServiceDescriptor(typeof(IParticipant),
            //    serviceProvider =>
            //    {
            //        var obj = ActivatorUtilities.CreateInstance<T>(serviceProvider, constructorParameters);
            //        obj.Key = name;
            //        configure(obj);
            //        return obj;
            //    },
            //    ServiceLifetime.Transient));
            return services;
        }
        public static IServiceCollection AddParticipant<T>(this IServiceCollection services, string participantName, Action<T> configure, params object[] constructorParameters)
              where T : IParticipant
        {
            services.Configure<DependencyMachineConfig>(c => c.InjectedParticipants.Add(serviceProvider =>
            {
                var obj = ActivatorUtilities.CreateInstance<T>(serviceProvider, constructorParameters);
                obj.Key = participantName;
                configure(obj);
                return obj;
            }));
            return services;
        }

        public static IServiceCollection AddParticipant<T>(this IServiceCollection services, string dependencyMachineName, string participantName, Action<T> configure, params object[] constructorParameters)
           where T : IParticipant
        {
            services.Configure<DependencyMachineConfig>(dependencyMachineName, c => c.InjectedParticipants.Add(serviceProvider =>
            {
                var obj = ActivatorUtilities.CreateInstance<T>(serviceProvider, constructorParameters);
                obj.Key = participantName;
                configure(obj);
                return obj;
            }));

            //services.TryAdd(new ServiceDescriptor(typeof(IParticipant),
            //    serviceProvider =>
            //    {
            //        var obj = ActivatorUtilities.CreateInstance<T>(serviceProvider, constructorParameters);
            //        obj.Key = name;
            //        configure(obj);
            //        return obj;
            //    },
            //    ServiceLifetime.Transient));
            return services;
        }


        #endregion
        private static Action<IParticipant>? GetConfigureParticipantForType(Type type)
        {
            var mi = type.GetMethod("ConfigureParticipant", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            if (mi != null) { return p => mi.Invoke(null, new object[] { p }); }
            else { return null; }
        }

        #region IHostedService

        public static IServiceCollection AddSingletonHostedServiceDependency<T>(this IServiceCollection services, Action<IParticipant>? configure = null)
            where T : class, IHostedService
            => services
            .AddSingleton<T>()
            .AddHostedServiceDependency<T>(null, configure)
            ;

        public static IServiceCollection AddSingletonHostedServiceDependency<TInterface, T>(this IServiceCollection services)
                 where TInterface : class
                 where T : class, TInterface, IHostedService
                 => services
                 .AddSingleton<T>()
                 .AddSingleton<TInterface, T>(serviceProvider=> serviceProvider.GetRequiredService<T>())
                 .AddHostedServiceDependency<T>(null, null)
                 ;

        public static IServiceCollection AddHostedServiceDependency<T>(this IServiceCollection services)
              where T : IHostedService
              => services.AddHostedServiceDependency<T>(null, null);

        public static IServiceCollection AddLateHostedServiceDependency<T>(this IServiceCollection services)
                 where T : IHostedService
                 => services.AddLateHostedServiceDependency<T>(null, null, Array.Empty<object>());

        public static IServiceCollection AddHostedServiceDependency<T>(this IServiceCollection services, Action<IParticipant>? configure)
            where T : IHostedService
            => services.AddHostedServiceDependency<T>(null, configure);
        public static IServiceCollection AddLateHostedServiceDependency<T>(this IServiceCollection services, Action<IParticipant>? configure, params object[] constructorParameters)
                 where T : IHostedService
                 => services.AddLateHostedServiceDependency<T>(null, configure, constructorParameters);

        public static IServiceCollection AddHostedServiceDependency<T>(this IServiceCollection services, string? name, Action<IParticipant>? configure)
          where T : IHostedService
        {
            services.Configure<DependencyMachineConfig>(c => c.InjectedParticipants.Add(serviceProvider =>
            {
                var obj = serviceProvider.GetRequiredService<T>();
                var participant = new HostedServiceParticipant<T>(obj)
                {
                    //Key = name ?? "type:" + typeof(T).FullName,
                };
                    
                participant.Provide(participant.DefaultKey!);
                configure?.Invoke(participant);

                GetConfigureParticipantForType(typeof(T))?.Invoke(participant);

                return participant;
            }));
            return services;
        }
        public static IServiceCollection AddLateHostedServiceDependency<T>(this IServiceCollection services, string? name, Action<IParticipant>? configure, params object[] constructorParameters)
            where T : IHostedService
        {
            services.Configure<DependencyMachineConfig>(c => c.InjectedParticipants.Add(serviceProvider =>
            {
                var obj = ActivatorUtilities.CreateInstance<T>(serviceProvider, constructorParameters);
                var participant = new HostedServiceParticipant<T>(obj)
                {
                    Key = name ?? "type:" + typeof(T).FullName,
                };
                configure?.Invoke(participant);

                GetConfigureParticipantForType(typeof(T))?.Invoke(participant);

                return participant;
            }));

            //services.TryAddEnumerable(
            //    ServiceDescriptor.Describe(typeof(IParticipant), typeof(HostedServiceParticipant<T>), ServiceLifetime.Transient);

            //new ServiceDescriptor(typeof(IParticipant), typeof(HostedServiceParticipant<T>)
            //    ,
            //    ServiceLifetime.Transient
            //    )
            //{
            //    ImplementationFactory = serviceProvider =>
            //    {
            //        var obj = ActivatorUtilities.CreateInstance<T>(serviceProvider, constructorParameters);
            //        var participant = new HostedServiceParticipant<T>(obj)
            //        {
            //            Key = name ?? "type:" + typeof(T).FullName,
            //        };
            //        configure(participant);
            //        return participant;
            //    }
            //});
            return services;
        }

        #endregion

        #region Default IParticipant type: Participant

        public static IServiceCollection AddParticipant(this IServiceCollection services, Action<Participant> configure)
           => services.AddParticipant<Participant>(configure);

        public static IServiceCollection AddParticipant(this IServiceCollection services, string dependencyMachineName, Action<Participant> configure)
                   => services.AddParticipant(configure, dependencyMachineName);

        #endregion

        #endregion

        #region Initializers (StartTask only)

        // Uses default IParticipant type: StartableParticipant
        public static IServiceCollection AddInitializer(this IServiceCollection services, Action<StartableParticipant> configure)
           => services.AddParticipant<StartableParticipant>(configure);
        public static IServiceCollection AddInitializer(this IServiceCollection services, string participantName, Action<StartableParticipant> configure)
           => services.AddParticipant<StartableParticipant>(participantName, configure);

        #region StartTask

        /// <summary>
        /// Custom IParticipant type, with custom constructorParameters (any unspecified constructor parameters will be injected from IServiceProvider)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="startTask"></param>
        /// <param name="configure"></param>
        /// <param name="constructorParameters">Parameters used to construct T, with unset parameters injected from IServiceProvider</param>
        /// <returns></returns>
        public static IServiceCollection AddInitializer<T>(
                 this IServiceCollection services,
                 Func<T, CancellationToken, Task<object?>> startTask,
                 Action<T>? configure = null,
                 params object[] constructorParameters
                 )
            where T : IParticipant<T>, IHas<IServiceProvider>
                => services.AddParticipant<T>(
                    p => { p.StartTask = startTask; configure?.Invoke(p); },
                    constructorParameters
                    );

        // Default type (StartableParticipant)
        public static IServiceCollection AddInitializer(
            this IServiceCollection services,
            Func<IParticipant, CancellationToken, Task<object?>> startTask,
            Action<StartableParticipant>? configure = null
            )
               => services.AddParticipant<StartableParticipant>(
                   participant =>
                   {
                       participant.StartTask = startTask;
                       configure?.Invoke(participant);
                   });

        #endregion

        #region Simplifications from StartTask: Action / Func / Func<IServiceProvider, ...>

        // Action
        public static IServiceCollection AddInitializer(this IServiceCollection services, Action startAction, Action<StartableParticipant>? configure = null)
                   => services.AddParticipant<StartableParticipant>(p => { p.StartAction = startAction; configure?.Invoke(p); });

        // Func
        public static IServiceCollection AddInitializer(this IServiceCollection services, Func<object?> startFunc, Action<StartableParticipant>? configure = null)
               => services.AddParticipant<StartableParticipant>(p => { p.StartFunc = (p, ct) => startFunc(); configure?.Invoke(p); });

        #region Func with IServiceProvider and CancellationToken

        // Default type (StartableParticipant)
        public static IServiceCollection AddInitializer(this IServiceCollection services, Func<IServiceProvider, CancellationToken, Task<object?>> startFunc, Action<StartableParticipant>? configure = null)
               => services.AddParticipant<StartableParticipant>(
                   p => { p.StartTask = (p, ct) => startFunc(p.ServiceProvider, ct); configure?.Invoke(p); });
        public static IServiceCollection AddInitializer(this IServiceCollection services, string participantName, Func<IServiceProvider, CancellationToken, Task<object?>> startFunc, Action<StartableParticipant>? configure = null)
               => services.AddParticipant<StartableParticipant>(participantName,
                   p => { p.StartTask = (p, ct) => startFunc(p.ServiceProvider, ct); configure?.Invoke(p); });

        //public static IServiceCollection AddParticipant(this IServiceCollection services, Func<StartableParticipant, CancellationToken, object?> startFunc, params object[] constructorParameters)
        //=> services.AddParticipant<StartableParticipant>(p => p.StartFunc = startFunc, constructorParameters);

        // Custom type
        public static IServiceCollection AddInitializer<T>(this IServiceCollection services, Func<IServiceProvider, CancellationToken, Task<object?>> startFunc, Action<T>? configure = null)
            where T : IParticipant<T>, IHas<IServiceProvider>
               => services.AddParticipant<T>(
                   p => { p.StartTask = (p, ct) => startFunc(p.Object, ct); configure?.Invoke(p); });
        public static IServiceCollection AddInitializer<T>(this IServiceCollection services, string participantName, Func<IServiceProvider, CancellationToken, Task<object?>> startFunc, Action<T>? configure = null)
            where T : IParticipant<T>, IHas<IServiceProvider>
               => services.AddParticipant<T>(participantName,
                   p => { p.StartTask = (p, ct) => startFunc(p.Object, ct); configure?.Invoke(p); });

        #endregion 

        #endregion

        #region Method injection

        /// <summary>
        /// Provide a delegate, and the method parameters will be injected.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="startDelegate">Parameters will be injected from the Participant's ServiceProvider.  
        /// Must return Task&lt;object?&gt;, object?, or void.  Return null on success, otherwise an object representing why 
        /// the method failed.  For void return values, throw an exception if the method fails.</param>
        /// <returns></returns>
        public static IServiceCollection AddInitializer(this IServiceCollection services, Delegate startDelegate, Action<StartableParticipant>? configure = null)
                => services.AddParticipant<StartableParticipant>(
                      p =>
                      {
                          p.StartTask = DependencyMachineDelegateHelpers.CreateInvoker<StartableParticipant>(startDelegate);
                          configure?.Invoke(p);
                      });

        public static IServiceCollection AddInitializer<T>(this IServiceCollection services, Delegate startDelegate, Action<T>? configure = null, params object[] constructorParameters)
            where T : IParticipant<T>, IHas<IServiceProvider>
                => services.AddParticipant<T>(
                      p =>
                      {
                          p.StartTask = DependencyMachineDelegateHelpers.CreateInvoker<T>(startDelegate);
                          configure?.Invoke(p);
                      }
                      , constructorParameters);

        #endregion

        #endregion

        #region FUTURE: Deinitializers (StopTask only)

        // FUTURE: AddDeinitializer, AddDependencyService (with start and stop)
        //public static IServiceCollection AddDeinitializer(this IServiceCollection services, Action stopAction, Action<StoppableParticipant>? configure = null)
        //=> services.AddParticipant<StoppableParticipant>(p => { p.StopAction = stopAction; configure?.Invoke(p); });

        #endregion

        #region FUTURE: AddDependencyService (StartTask and StopTask)

        //public static IServiceCollection AddDependencyService(this IServiceCollection services, Action startAction, Action stopAction, Action<Participant>? configure = null)

        #endregion

    }
}
