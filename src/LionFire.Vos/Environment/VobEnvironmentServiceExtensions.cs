using LionFire.Vos;
using LionFire.Vos.Environment;
using LionFire.Vos.Internals;
using Microsoft.Extensions.DependencyInjection;
using LionFire.DependencyMachines;
using System;
using LionFire.ExtensionMethods.Dependencies;
using LionFire.Vos.Services;

namespace LionFire.Services
{
    public static class VobEnvironmentServiceExtensions
    {
        #region Value

        /// <summary>
        /// Set environment on root Vob
        /// </summary>
        /// <param name="services"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IServiceCollection VobEnvironment<T>(this IServiceCollection services, string key, T value)
            => services.VobEnvironment<T>("/", key, value, addEnvironmentNodeAtVobIfMissing: true);

        public static IServiceCollection VobEnvironment<T>(this IServiceCollection services, VobReference vob, string key, T value, bool addEnvironmentNodeAtVobIfMissing = true)
        {
            ValidateKey(key);
            return services.InitializeVob(vob,
                v => GetVobEnvironment(v, addEnvironmentNodeAtVobIfMissing)[key] = value,
                participant => ConfigureParticipant(participant, vob, key)
              );
        }

        #endregion

        #region Value Factory

        public static IServiceCollection VobEnvironment<T>(this IServiceCollection services, string key, Func<IServiceProvider, T> valueFactory)
                       => services.VobEnvironment<T>("/", key, valueFactory, addEnvironmentNodeAtVobIfMissing: true);

        public static IServiceCollection VobEnvironment<T>(this IServiceCollection services, VobReference vob, string key, Func<IServiceProvider, T> valueFactory, bool addEnvironmentNodeAtVobIfMissing = true)
        {
            ValidateKey(key);
            return services.InitializeVob(vob,
              v => GetVobEnvironment(v, addEnvironmentNodeAtVobIfMissing)[key] = valueFactory(v.GetServiceProvider()),
              participant => ConfigureParticipant(participant, vob, key));
        }

        #endregion

        #region (Private) Methods

        private static string ValidateKey(this string key)
        {
            //if (key.StartsWith("$")) throw new ArgumentException("key cannot start with $");
            //key = key.TrimStart('$');
            //key = key.StartsWith("$") ? key.Substring(1) : key;

            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Environment keys must be non-null, non-empty and cannot be all whitespace");
            if (!char.IsLetter(key[0])) throw new ArgumentException("Environment keys must begin with a letter");

            if (VobEnvironmentSettings.StrictEnvironmentKeys)
            {
                for (int i = 1; i < key.Length; i++)
                {
                    if (char.IsLetterOrDigit(key[i])) continue;
                    if (key[i] == '_') continue;

                    throw new ArgumentException("Environment keys can only contain letters, numbers, and underscores");
                }
            }
            return key;
        }

        private static VobEnvironment GetVobEnvironment(IVob v, bool addEnvironmentNodeAtVobIfMissing)
       => (addEnvironmentNodeAtVobIfMissing
              ? v.GetOrAddOwn<VobEnvironment>()
              : v.GetNextOrCreateAtRoot<VobEnvironment>());


        private static void ConfigureParticipant(IParticipant participant, VobReference vob, string key)
       => participant
               .Key($"{vob} ${key}")
               .After($"services:{vob}")
               .Provide($"{vob} ${key}")
               .Contributes($"{vob}<VobEnvironment>/*")
               .Before(vob.ToString()) // Should be redundant to Contributes {vob}<VobEnvironment> if vob is root and {vob}<VobEnvironment> is part of a stage chain 
                                       // Contributes may be wrong here, if GetNext finds a VobEnvironment not at root. 
                                       // REVIEW - Also, it leads to a duplicate contributes item.  Maybe .Contributes should be a no-op for duplicates.
                                       //.Contributes(addEnvironmentNodeAtVobIfMissing ? vob.ToString() : vob.GetRoot().ToString())
       ;

        #endregion

    }
}
