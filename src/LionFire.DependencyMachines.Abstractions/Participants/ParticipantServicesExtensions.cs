#nullable enable
using LionFire.DependencyMachines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Services
{
    public static class ParticipantServicesExtensions
    {
        #region Configure

        public static IServiceCollection AddParticipant<T>(this IServiceCollection services, Action<T> configure, params object[] constructorParameters)
            where T : IParticipant
        {
            services.TryAdd(new ServiceDescriptor(typeof(IParticipant),
                serviceProvider =>
                {
                    var obj = ActivatorUtilities.CreateInstance<T>(serviceProvider, constructorParameters);
                    configure(obj);
                    return obj;
                },
                ServiceLifetime.Transient));
            return services;
        }
        public static IServiceCollection AddParticipant(this IServiceCollection services, Action<StartableParticipant> configure, params object[] constructorParameters)
           => services.AddParticipant<StartableParticipant>(configure, constructorParameters);

        #endregion

        #region Just an Action/Func

        // TODO: Add StopTask

        public static IServiceCollection AddParticipant(this IServiceCollection services, Action startAction, params object[] constructorParameters)
                   => services.AddParticipant<StartableParticipant>(p => p.StartAction = startAction, constructorParameters);
        public static IServiceCollection AddParticipant(this IServiceCollection services, Func<StartableParticipant, CancellationToken, object?> startFunc, params object[] constructorParameters)
                           => services.AddParticipant<StartableParticipant>(p => p.StartFunc = startFunc, constructorParameters);
        public static IServiceCollection AddParticipant(this IServiceCollection services, Func<object?> startFunc, params object[] constructorParameters)
                           => services.AddParticipant<StartableParticipant>(p => p.StartFunc = (p,ct) => startFunc(), constructorParameters);

        public static IServiceCollection AddParticipant(this IServiceCollection services, Func<IServiceProvider, CancellationToken, Task<object?>> startFunc, params object[] constructorParameters)
                           => services.AddParticipant<StartableParticipant>(
                               p => p.StartTask = (p, ct) => startFunc(p.ServiceProvider, ct), constructorParameters);

        #endregion

        #region Method injection

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="startDelegate">Parameters will be injected from the Participant's ServiceProvider.  
        /// Must return Task&lt;object?&gt;, object?, or void.  Return null on success, otherwise an object representing why 
        /// the method failed.  For void return values, throw an exception if the method fails.</param>
        /// <param name="constructorParameters"></param>
        /// <returns></returns>
        public static IServiceCollection AddInitializer(this IServiceCollection services, Delegate startDelegate, Action<StartableParticipant>? configure = null, params object[] constructorParameters)
                => services.AddParticipant<StartableParticipant>(
                      p =>
                      {
                          p.StartTask = DependencyMachineDelegateHelpers.Invoke<StartableParticipant>(startDelegate);
                          configure?.Invoke(p);
                      }
                      , constructorParameters);

        #endregion

    }
}
