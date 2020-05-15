using LionFire.Vos;
using LionFire.Vos.Environment;
using LionFire.Vos.Internals;
using Microsoft.Extensions.DependencyInjection;
using LionFire.DependencyMachines;
using System;

namespace LionFire.Services
{
    public static class VobEnvironmentServiceExtensions
    {
        public static IServiceCollection VobEnvironment<T>(this IServiceCollection services, VosReference vob, string key, T value, bool addEnvironmentNodeAtVobIfMissing = true)
        {
            key = key.TrimStart('$');

            return services.InitializeVob(vob, v =>
                   (addEnvironmentNodeAtVobIfMissing
                   ? v.GetOrAddOwn<VobEnvironment>()
                   : v.GetNextOrCreateAtRoot<VobEnvironment>())
                   [key.StartsWith("$") ? key.Substring(1) : key] = value,
              participant => 
                participant
                    .Key($"{vob} ${key}")
                    .Provides($"{vob} ${key}")
                    .Contributes($"environment:{vob}")
                    .Before(vob.ToString()) // Should be redundnat to Contributes environment:{vob} if vob is root and environment:{vob} is part of a stage chain
             // Contributes may be wrong here, if GetNext finds a VobEnvironment not at root. 
             // REVIEW - Also, it leads to a duplicate contributes item.  Maybe .Contributes should be a no-op for duplicates.
             //.Contributes(addEnvironmentNodeAtVobIfMissing ? vob.ToString() : vob.GetRoot().ToString())
              );
        }

        /// <summary>
        /// Set environment on root Vob
        /// </summary>
        /// <param name="services"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IServiceCollection VobEnvironment<T>(this IServiceCollection services, string key, T value)
            => services.VobEnvironment<T>("/", key, value, addEnvironmentNodeAtVobIfMissing: true);

    }
}
