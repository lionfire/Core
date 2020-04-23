#nullable enable
using LionFire.DependencyMachines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public static IServiceCollection AddParticipant(this IServiceCollection services, Action<Participant> configure, params object[] constructorParameters)
           => services.AddParticipant<Participant>(configure, constructorParameters);

        #endregion

        #region Just an Action/Func

        // TODO: Add StopTask

        public static IServiceCollection AddParticipant(this IServiceCollection services, Action startAction, params object[] constructorParameters)
                   => services.AddParticipant<Participant>(p => p.StartAction = startAction, constructorParameters);
        public static IServiceCollection AddParticipant(this IServiceCollection services, Func<object?> startFunc, params object[] constructorParameters)
                           => services.AddParticipant<Participant>(p => p.StartFunc = startFunc, constructorParameters);

        public static IServiceCollection AddParticipant(this IServiceCollection services, Func<IServiceProvider, CancellationToken, Task<object?>> startFunc, params object[] constructorParameters)
                           => services.AddParticipant<Participant>(
                               p => p.StartTask = (p, ct) => startFunc(p.ServiceProvider, ct), constructorParameters);

        #endregion

        #region Method injection

#error NEXT: Make this work.  Go through startDelegate parameters and invoke p.ServiceProvider.GetRequiredService().  Require Task<object?> return value, else wrap it in a converter.

        public static IServiceCollection AddParticipantD(this IServiceCollection services, Delegate startDelegate, params object[] constructorParameters)
                => services.AddParticipant<Participant>(
                                       p => p.StartTask = (p, ct) => startFunc(p.ServiceProvider, ct), constructorParameters);


        #endregion

        static void x()
        {
            IServiceCollection sc;

            sc.AddParticipantD(
               new Func<string, int, object?>((vob, x) => { return x.ToString() == vob; });


        }
    }
}
