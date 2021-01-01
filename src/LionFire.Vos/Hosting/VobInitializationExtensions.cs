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
using LionFire.Hosting;
using LionFire.Referencing;
using System.Runtime.CompilerServices;
using System.Linq;

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

        //public static IServiceCollection InitializeVob(this IServiceCollection services, Action<IServiceProvider, IVob> action, IVobReference vobReference)
        //{

        //    services.TryAddEnumerable(new ServiceDescriptor(typeof(VobInitializer), serviceProvider =>
        //    {
        //        return new VobInitializer(vobReference, action);
        //    }, ServiceLifetime.Transient));

        //    //services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict =>
        //        //dict.Add(new VobInitializer(VobReference.FromRootName(rootName), action)));
        //    return services;
        //}

        //#region Configure VobInitializer

        //// Configure action
        //public static IServiceCollection InitializeVob(this IServiceCollection services, VobReference vobReference, Action<VobInitializer> configure)
        //    => services.AddParticipant(configure, vobReference);

        //// Configure action, with custom VobInitializer type
        //public static IServiceCollection InitializeVob<TInitializer>(this IServiceCollection services, VobReference vobReference, Action<TInitializer> configure)
        //    where TInitializer : VobInitializer
        //    => services.AddParticipant(configure, vobReference);

        //#endregion

        #region Action

        // Simplest case: perform a synchronous action on a Vob
        public static IServiceCollection InitializeVob(this IServiceCollection services, VobReference vobReference, Action<IVob> init, Action<IParticipant>? configure = null, VobInitializerFlags flags = VobInitializerFlags.Default)
            => services.AddInitializer((Action<IVos>)(v => init(v.GetVob(vobReference))), configureWrapper(vobReference, configure, flags));

        /// <summary>
        /// Initialize synchronously, with an opportunity to return a validation failure explanation (rather than throwing an exception.)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="vobReference"></param>
        /// <param name="initAndValidate">Must return null on success, otherwise something to indicate why the initialization failed.</param>
        /// <param name="configure"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static IServiceCollection InitializeVobWithValidation(this IServiceCollection services, VobReference vobReference, Func<IVob, object?> initAndValidate, Action<IParticipant>? configure = null, VobInitializerFlags flags = VobInitializerFlags.Default)
            => services.AddInitializer((Func<IVos, object?>)(v => initAndValidate(v.GetVob(vobReference))), configureWrapper(vobReference, configure, flags));
        //c =>
        //{
        //    c.Contributes(vobReference.ToString());
        //    if (defaultDependency)
        //    {
        //        if (vobReference.Path == "/") c.DependsOn("vos:");
        //        else c.DependsOn(vobReference.GetParent().ToString());
        //    }
        //    configure?.Invoke(c);
        //});

        /// <summary>
        /// Initialize asynchronously, with an opportunity to return a validation failure explanation (rather than throwing an exception.)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="vobReference"></param>
        /// <param name="initAndValidate"></param>
        /// <param name="configure"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static IServiceCollection InitializeVobWithValidation(this IServiceCollection services, VobReference vobReference, Func<IVob, Task<object?>> initAndValidate, Action<IParticipant>? configure = null, VobInitializerFlags flags = VobInitializerFlags.Default)
            => services.AddInitializer((Func<IVos, Task<object?>>)(v => initAndValidate(v.GetVob(vobReference))), configureWrapper(vobReference, configure, flags));

        #endregion

        private static Action<IParticipant> configureWrapper(IVobReference vobReference, Action<IParticipant>? configure, VobInitializerFlags flags = VobInitializerFlags.Default, string? key = null)
        {

            return c =>
            {
                foreach (var env in vobReference.ExtractEnvironmentVariables())
                {
                    //.DependsOn("vos:/<VobEnvironment>/*")
                    c.DependsOn("vos:/ " + env); // FIXME - I think this should be $"vos:/<VobEnvironment>/{env}"
                }
                string automaticKey = "";
                if (flags.HasFlag(VobInitializerFlags.Contributes)) { c.Contributes(vobReference.ToString()); automaticKey += $"{vobReference} contributor"; }
                if (flags.HasFlag(VobInitializerFlags.AfterParent))
                {
                    if (automaticKey == "") { automaticKey += $"After {vobReference}"; }

                    if (vobReference.ToString() != "vos:")
                    {
                        IVobReference? ancestor = vobReference.GetParent(nullIfBeyondRoot: true);
                        do
                        {
                            // OPTIMIZE - Though this gets inefficient - may want to build hierarchical After/DependsOn into DependencyStateMachine
                            if (ancestor == null) c.After("vos:");
                            else
                            {
                                c.After(ancestor.ToString());
                                ancestor = ancestor.GetParent(nullIfBeyondRoot: true);
                            }
                        }
                        while (ancestor != null);
                    }
                }

                if (flags.HasFlag(VobInitializerFlags.DependsOnParent))
                {
                    throw new NotImplementedException(); // Do same logic as AfterParent 
                }
                automaticKey += " " + Guid.NewGuid().ToString();
                configure?.Invoke(c);
                if (!c.HasKey)
                {
                    c.Key(key ?? automaticKey);
                }
            };
        }

        #region Method Injection

        // Function injection, with manual insertion of IVob
        public static IServiceCollection InitializeVob(this IServiceCollection services,
            VobReference vobReference,
            Delegate del,
            Action<IParticipant>? configure = null, VobInitializerFlags flags = VobInitializerFlags.Default)
            => services.AddInitializer(
                (Func<StartableParticipant, CancellationToken, Task<object?>>)
                (DependencyMachineDelegateHelpers.CreateInvoker<StartableParticipant>(
                        del,
                        (typeof(IVob), sp => sp.GetRequiredService<IVos>().GetVob(vobReference)))
                ),
                configureWrapper(vobReference, configure, flags));

        // Overload passthrough: Converts VobReference to IVobReference.  Allows implicit string operator to be used for VobReference.
        public static IServiceCollection InitializeVob<TParameter1>(this IServiceCollection services,
                VobReference vobReference,
                Action<IVob, TParameter1> action,
                Action<IParticipant>? configure = null, VobInitializerFlags flags = VobInitializerFlags.Default, string? key = null)
                => InitializeVob<TParameter1>(services, (IVobReference)vobReference, action, configure, flags, key);

        public static IServiceCollection InitializeVob<TParameter1>(this IServiceCollection services,
                IVobReference vobReference,
                Action<IVob, TParameter1> action,
                Action<IParticipant>? configure = null, VobInitializerFlags flags = VobInitializerFlags.Default, string? key = null)
                => services.AddInitializer(
                    (Func<StartableParticipant, CancellationToken, Task<object?>>)
                    (DependencyMachineDelegateHelpers.CreateInvoker<StartableParticipant>(
                            action,
                            (typeof(IVob), sp => sp.GetRequiredService<IVos>().GetVob(vobReference)))
                    ),
                    configureWrapper(vobReference, configure, flags, key));

        public static IServiceCollection InitializeVob<TParameter1, TParameter2>(this IServiceCollection services,
                VobReference vobReference,
                Action<IVob, TParameter1, TParameter2> action,
                Action<IParticipant>? configure = null, VobInitializerFlags flags = VobInitializerFlags.Default)
                => services.AddInitializer(
                    (Func<StartableParticipant, CancellationToken, Task<object?>>)
                    (DependencyMachineDelegateHelpers.CreateInvoker<StartableParticipant>(
                            action,
                            (typeof(IVob), sp => sp.GetRequiredService<IVos>().GetVob(vobReference)))
                    ),
                    configureWrapper(vobReference, configure, flags));

        #endregion

        //public static IServiceCollection InitializeVob(this IServiceCollection services, VobReference vobReference, Func<IServiceProvider, IVob, CancellationToken, Task<object?>> initializationAction)
        //{
        //    services.InitializeVob(vobReference, vi =>
        //    {
        //        vi.InitializationAction = initializationAction,
        //    }, stage: stage);

        //    services.TryAddEnumerable(new ServiceDescriptor(typeof(VobInitializer), serviceProvider =>
        //    {
        //        ActivatorUtilities.CreateInstance<VobInitializer>(serviceProvider, vobReference


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
            services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict => dict.Add(new VobInitializer(VobReference.FromRootName(rootName), action)));
            return services;
        }
        public static IServiceCollection InitializeRootVob(this IServiceCollection services, Action<IServiceProvider, IRootVob> action, string rootName = VosConstants.DefaultRootName)
        {
            services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict => dict.Add(new VobInitializer(VobReference.FromRootName(rootName), action)));
            return services;
        }

        public static IServiceCollection InitializeRootVob(this IServiceCollection services, Action<IRootVob> action, string rootName = VosConstants.DefaultRootName, IEnumerable<string>? contributes = null)
        {
            services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict => dict.Add(new VobInitializer(VobReference.FromRootName(rootName), action)));
            return services;
        }

        #region InitializeVob

        //public static IServiceCollection InitializeVob(this IServiceCollection services, string vobPath, Action<IServiceProvider, IVob> action) 
        //=> services.InitializeVob(vobPath.ToVobReference(), action);

        public static IServiceCollection InitializeVob(this IServiceCollection services, IVobReference vob, Action<IServiceProvider, IVob> action)
        {
            services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict => dict.Add(new VobInitializer(vob, action)));
            return services;
        }

        public static IServiceCollection InitializeVob(this IServiceCollection services, VobReference vobPath, Action<IVob> action)
        {
            services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict => dict.Add(new VobInitializer(vobPath.ToVobReference(), action)));
            return services;
        }
        //public static IServiceCollection InitializeVob(this IServiceCollection services, string vobRootName, string vobPath, Action<IVob> action)
        //{
        //    services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict => dict.Add(new VobInitializer(vobPath.ToVobReference(), action) { VobPath = vobPath, VobRootName = vobRootName }));
        //    return services;
        //}

        public static IServiceCollection InitializeVob(this IServiceCollection services, IEnumerable<string> vobPath, Action<IVob> action)
        {
            services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict => dict.Add(new VobInitializer(new VobReference(vobPath), action)));
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="vobPath"></param>
        /// <param name="action">Return true if completed successfully, false if the action should be invoked again after trying other initializers.</param>
        /// <returns></returns>
        public static IServiceCollection InitializeVob(this IServiceCollection services, VobReference vobPath, Func<IServiceProvider, IVob, object> action)
        {
            services.Configure<ConcurrentDictionary<string, List<VobInitializer>>>(dict => dict.Add(new VobInitializer(vobPath.ToVobReference(), action)));
            return services;
        }

        #endregion
#endif

    }
}


