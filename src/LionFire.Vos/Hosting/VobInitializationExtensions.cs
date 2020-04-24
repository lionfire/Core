#nullable enable
using System;
using LionFire.Vos;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Dependencies;
using System.Collections.Generic;
using LionFire.DependencyMachines;
using LionFire.ExtensionMethods;
using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Services
{
    //public static class VobInitializerListExtensions
    //{
    //    public static void Add(this ConcurrentDictionary<string, List<VobInitializer>> dict, VobInitializer i)
    //        => dict.GetOrAdd(i.Key, k => new List<VobInitializer>()).Add(i);
    //}

    //public class VobInitializerParticipant : Participant
    //{
    //    public const string DefaultStageName = "Vobs";

    //    List<VobInitializer> vobInitializers;
    //    public VobInitializerParticipant(IOptionsMonitor<List<VobInitializer>> options, string stageName = DefaultStageName)
    //    {
    //        vobInitializers = options.Get(stageName);

    //        if (!string.IsNullOrWhiteSpace(stageName))
    //        {
    //            this.Contributes = new List<object> { stageName };
    //        }
    //    }
    //}

    public static class VobInitializationExtensions
    {
        //public static  IServiceCollection AddVobInitializer(IServiceCollection services, string stageName)
        //{

        //    services.Configure<DependencyMachineConfig>(config =>
        //    {
        //        config.Participants.Add(new VobInitializerParticipant())
        //    })
        //}

        #region IParticipant

        //public static IServiceCollection InitializeVob(this IServiceCollection services, Action<IServiceProvider, IVob> action, IVosReference vosReference)
        //{

        //    services.TryAddEnumerable(new ServiceDescriptor(typeof(VobInitializer), serviceProvider =>
        //    {
        //        return new VobInitializer(vosReference, action);
        //    }, ServiceLifetime.Transient));

        //    //services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict =>
        //        //dict.Add(new VobInitializer(VosReference.FromRootName(rootName), action)));
        //    return services;
        //}

        //#region Configure VobInitializer

        //// Configure action
        //public static IServiceCollection InitializeVob(this IServiceCollection services, VosReference vosReference, Action<VobInitializer> configure)
        //    => services.AddParticipant(configure, vosReference);

        //// Configure action, with custom VobInitializer type
        //public static IServiceCollection InitializeVob<TInitializer>(this IServiceCollection services, VosReference vosReference, Action<TInitializer> configure)
        //    where TInitializer : VobInitializer
        //    => services.AddParticipant(configure, vosReference);

        //#endregion

        #region Action

        // Simplest case: perform a synchronous action on a Vob
        public static IServiceCollection InitializeVob(this IServiceCollection services, VosReference vosReference, Action<IVob> action, Action<IParticipant>? configure = null)
            => services.AddInitializer((Action<IVos>)(v => action(v.GetVob(vosReference))), configure);
        public static IServiceCollection InitializeVob(this IServiceCollection services, VosReference vosReference, Func<IVob, object?> func, Action<IParticipant>? configure = null)
            => services.AddInitializer((Func<IVos, object?>)(v => func(v.GetVob(vosReference))), configure);
        public static IServiceCollection InitializeVob(this IServiceCollection services, VosReference vosReference, Func<IVob, Task<object?>> func, Action<IParticipant>? configure = null)
            => services.AddInitializer((Func<IVos, Task<object?>>)(v => func(v.GetVob(vosReference))), configure);

        #endregion

        //public static IServiceCollection InitializeVob(this IServiceCollection services, VosReference vosReference, Func<IServiceProvider, IVob, CancellationToken, Task<object?>> initializationAction)
        //{
        //    services.InitializeVob(vosReference, vi =>
        //    {
        //        vi.InitializationAction = initializationAction,
        //    }, stage: stage);

        //    services.TryAddEnumerable(new ServiceDescriptor(typeof(VobInitializer), serviceProvider =>
        //    {
        //        ActivatorUtilities.CreateInstance<VobInitializer>(serviceProvider, vosReference


        //        return factory(serviceProvider);
        //    }, ServiceLifetime.Transient));
        //    return services;
        //}
        #endregion

        //public static IServiceCollection InitializeRootVob(this IServiceCollection services, Action<IVob> action)
        //{
        //    services.TryAddEnumerableSingleton(new VobInitializer(action));
        //    return services;
        //}
        //public static IServiceCollection InitializeVob(this IServiceCollection services, string vobPath, Action<IVob> action)
        //{
        //    services.TryAddEnumerableSingleton(new VobInitializer(action) { VobPath = vobPath });
        //    return services;
        //}
        //public static IServiceCollection InitializeVob(this IServiceCollection services, string vobRootName, string vobPath, Action<IVob> action)
        //{
        //    services.TryAddEnumerableSingleton(new VobInitializer(action) { VobPath = vobPath, VobRootName = vobRootName });
        //    return services;
        //}

        //#error NEXT: How should VobInitializers work?  I think I should have action, or participant, not both.  If action, then create a participant.  If stage undeclared, use "RootVobs", "AlternateRootVobs", or "Vobs".  Maybe each child Vob should depend on its ancestors?  Maybe that means having an ordering within Vobs, if the IParticipants are IComparable.  Use alphabetical order for Vobs not in same lineage.  Instead of those 3 stages, just use "VobTree".

#if false

        public static IServiceCollection InitializeRootVob(this IServiceCollection services, Func<IServiceProvider, IRootVob, object> action, string rootName = VosConstants.DefaultRootName)
        {
            services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict => dict.Add(new VobInitializer(VosReference.FromRootName(rootName), action)));
            return services;
        }
        public static IServiceCollection InitializeRootVob(this IServiceCollection services, Action<IServiceProvider, IRootVob> action, string rootName = VosConstants.DefaultRootName)
        {
            services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict => dict.Add(new VobInitializer(VosReference.FromRootName(rootName), action)));
            return services;
        }

        public static IServiceCollection InitializeRootVob(this IServiceCollection services, Action<IRootVob> action, string rootName = VosConstants.DefaultRootName, IEnumerable<string>? contributes = null)
        {
            services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict => dict.Add(new VobInitializer(VosReference.FromRootName(rootName), action)));
            return services;
        }

        #region InitializeVob

        //public static IServiceCollection InitializeVob(this IServiceCollection services, string vobPath, Action<IServiceProvider, IVob> action) 
        //=> services.InitializeVob(vobPath.ToVosReference(), action);

        public static IServiceCollection InitializeVob(this IServiceCollection services, IVosReference vob, Action<IServiceProvider, IVob> action)
        {
            services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict => dict.Add(new VobInitializer(vob, action)));
            return services;
        }

        public static IServiceCollection InitializeVob(this IServiceCollection services, VosReference vobPath, Action<IVob> action)
        {
            services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict => dict.Add(new VobInitializer(vobPath.ToVosReference(), action)));
            return services;
        }
        //public static IServiceCollection InitializeVob(this IServiceCollection services, string vobRootName, string vobPath, Action<IVob> action)
        //{
        //    services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict => dict.Add(new VobInitializer(vobPath.ToVosReference(), action) { VobPath = vobPath, VobRootName = vobRootName }));
        //    return services;
        //}

        public static IServiceCollection InitializeVob(this IServiceCollection services, IEnumerable<string> vobPath, Action<IVob> action)
        {
            services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict => dict.Add(new VobInitializer(new VosReference(vobPath), action)));
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="vobPath"></param>
        /// <param name="action">Return true if completed successfully, false if the action should be invoked again after trying other initializers.</param>
        /// <returns></returns>
        public static IServiceCollection InitializeVob(this IServiceCollection services, VosReference vobPath, Func<IServiceProvider, IVob, object> action)
        {
            services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict => dict.Add(new VobInitializer(vobPath.ToVosReference(), action)));
            return services;
        }

        #endregion
#endif

    }
}


