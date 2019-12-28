using System;
using LionFire.Persistence;
using LionFire.Vos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LionFire.Dependencies;
using System.Diagnostics;
using LionFire.Persistence.Handles;
using LionFire.Vos.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Persistence.Persisters.Vos;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using LionFire.Vos.Services;
using LionFire.Vos.Mounts;

namespace LionFire.Services
{
    public static class VosHost
    {
        public static IHostBuilder Create(string[] args = null, bool defaultBuilder = true, Action<IServiceCollection> serializers = null)
        {
            return PersistersHost.Create(args, defaultBuilder: defaultBuilder)
                .AddVos();
        }

        public static IHostBuilder AddVos(this IHostBuilder hostBuilder)
            => hostBuilder
                .AddPersisters()
                .ConfigureServices((_, services) =>
                    {
                        services
                            .AddSingleton<VosRootManager>()
                            .AddSingleton<VosInitializer>()
                            .AddSingleton(serviceProvider => serviceProvider.GetService<IOptionsMonitor<VosOptions>>().CurrentValue)
                            .AddSingleton<IPersisterProvider<VosReference>, VosPersisterProvider>()
                            .AddSingleton<IReadHandleProvider<VosReference>, VosHandleProvider>()
                            .AddSingleton<IReadWriteHandleProvider<VosReference>, VosHandleProvider>()
                            .Configure<VosOptions>(vo =>
                            {
                                vo.RootNames = new string[] { "", "TestAltRoot" };

                                Debug.WriteLine("Configure VosOptions - defaults");
                            })
                            .PostConfigure<VosOptions>(o =>
                            {
                                Debug.WriteLine("Mounts:");
                                foreach (var mount in o.Mounts)
                                {
                                    Debug.WriteLine(" - " + mount);
                                }
                            })

                            // TEMP - MOVE this.  Work in progress:
                            .InitializeRootVob(vob =>
                            {
                                vob
                                .AddServiceProvider()
                                .GetService<IServiceCollection>()
                                    .AddSingleton<VobMounter>()
                                ;

                            })
                              .InitializeVob("InitializeVobTest", vob =>
                             {
                                 vob.AddServiceProvider();
                             })
                            .InitializeVob("TestAltRoot", "/", vob =>
                            {
                                vob.AddServiceProvider();
                            })
                        ;
                    })
            ;

    }

    public static class VosConstants
    {
        public const string DefaultRoot = "";
    }

    public class VobInitializer
    {
        public string VobRootName { get; set; } = VosConstants.DefaultRoot;

        public string VobPath { get; set; }

        /// <summary>
        /// Targets Root Vob of default Root
        /// </summary>
        /// <param name="initializationAction"></param>
        public VobInitializer(Action<Vob> initializationAction)
        {
            InitializationAction = initializationAction;
        }

        /// <summary>
        /// Targets default Root
        /// </summary>
        /// <param name="path"></param>
        /// <param name="initializationAction"></param>
        public VobInitializer(string path, Action<Vob> initializationAction)
        {
            VobPath = path;
            InitializationAction = initializationAction;
        }
        public Action<Vob> InitializationAction { get; set; }
    }

    public class VosInitializer
    {
        public VosInitializer(VosRootManager vosRootManager, IEnumerable<VobInitializer> vobInitializers)
        {
            foreach (var initializer in vobInitializers)
            {
                var rootVob = vosRootManager.Get(initializer.VobRootName);

                Vob vob = rootVob;
                if (!string.IsNullOrEmpty(initializer.VobPath)) vob = vob[initializer.VobPath];
                initializer.InitializationAction(vob);
            }
        }
    }
    public static class VobInitializationExtensions
    {
        public static IServiceCollection InitializeRootVob(this IServiceCollection services, Action<Vob> action)
        {
            services.TryAddEnumerableSingleton(new VobInitializer(action));
            return services;
        }
        public static IServiceCollection InitializeVob(this IServiceCollection services, string vobPath, Action<Vob> action)
        {
            services.TryAddEnumerableSingleton(new VobInitializer(action) { VobPath = vobPath });
            return services;
        }
        public static IServiceCollection InitializeVob(this IServiceCollection services, string vobRootName, string vobPath, Action<Vob> action)
        {
            services.TryAddEnumerableSingleton(new VobInitializer(action) { VobPath = vobPath, VobRootName = vobRootName });
            return services;
        }
    }
}

