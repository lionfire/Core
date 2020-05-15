using LionFire.Applications.Hosting;
using LionFire.Referencing;
using LionFire.Persistence.Handles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using LionFire.Hosting;
//using LionFire.Services;
using LionFire.ObjectBus;
using LionFire.Persistence.Filesystem;
using LionFire.Services;
using LionFire.Serialization;
using LionFire.Dependencies;
using LionFire.Vos;
using LionFire.DependencyInjection.ExtensionMethods;
using LionFire.FlexObjects;
using LionFire.Vos.VosApp;
using LionFire.Vos.Assets.Persisters;

namespace LionFire.Hosting
{

    public static class FrameworkHostBuilder
    {
        public static IServiceCollection AddDefaultSerializers(this IServiceCollection services)
        {
            services.AddBuiltInSerializers();
            services.AddNewtonsoftJson();
            return services;
        }
        private static void DefaultAddDefaultSerializers(this IServiceCollection services) => services.AddDefaultSerializers();

#if TODO // TOVOSAPP
        public static IHostBuilder CreateVos(string[] args = null, Action<IServiceCollection> serializers = null)
        {
            return Create(args, serializers: serializers)
                //.AddObjectBus<FSOBus>()
                .AddVosApp();
        }

        public static IHostBuilder CreateDefault(string[] args = null, Action<IServiceCollection> serializers = null)
        {
            return Create(args, serializers: serializers)
                //.AddObjectBus<FSOBus>()
                .AddVosApp();
        }
#endif

        /// <summary>
        /// Included:
        ///  - IReferenceToHandleService
        ///  - IReferenceProviderService
        ///  - ISerializationProvider
        ///    - DefaultScorers.DefaultDeserializerScorers
        ///    - DefaultScorers.DefaultSerializerScorers
        /// </summary>
        /// <param name="defaultBuilder">Invokes Host.CreateDefaultBuilder(args).  Set to false for simple unit tests that don't need logging or configuration.</param>
        /// <param name="serializers"></param>
        /// <returns></returns>
        public static IHostBuilder Create(string[] args = null, bool defaultBuilder = true, Action<IServiceCollection> serializers = null)
        {
            // TODO: Base on VosHost or VosAppHost?

            IHostBuilder hostBuilder = defaultBuilder ? Host.CreateDefaultBuilder(args) : new HostBuilder();

            return hostBuilder
                .UseDependencyContext()
                //.ConfigureLogging(loggingBuilder =>
                //{
                //})
                .ConfigureServices((context, services) =>
                {
                    services
                    .AddSerialization()
                    .AddFilesystem();

                    (serializers ?? DefaultAddDefaultSerializers)(services);

                    services
                    .AddSingleton<IReferenceToHandleService, ReferenceToHandleService>()
                    .AddSingleton<IReferenceProviderService, ReferenceProviderService>()
                    ;
                    //services.Configure<ObjectBusOptions>(option => // Allows injection of IOptions<ObjectBusOptions>
                    //{
                    //    option.SampleOption = 123;
                    //});

                })
                //.ConfigureHostConfiguration(config =>
                //{

                //})
                //.ConfigureAppConfiguration((context, config) =>
                //{
                //    //context.HostingEnvironment.
                //})
                ;
        }

        public static IHostBuilder CreateDefault(string[] args = null, bool defaultBuilder = true, FrameworkHostBuilderOptions frameworkOptions = null, IFlex options = null
            //, Action<IServiceCollection> serializers = null
            )
            => CreateDefaultVosAppHost(args, defaultBuilder, frameworkOptions, options);

        public class FrameworkHostBuilderOptions
        {
            public Action<IServiceCollection> Serializers { get; set; }
        }
        public static IHostBuilder CreateDefaultPersisterHost(string[] args = null, bool defaultBuilder = true, FrameworkHostBuilderOptions frameworkOptions = null, IFlex options = null)
        {
            return Create(args, defaultBuilder)
                .AugmentWithFramework(frameworkOptions ?? options?.Get<FrameworkHostBuilderOptions>())
                ;
        }

        private static IHostBuilder AugmentWithFramework(this IHostBuilder hostBuilder, FrameworkHostBuilderOptions options = null)
        {
            return hostBuilder
                .ConfigureServices((context, services) =>
                {
                    (options?.Serializers ?? DefaultAddDefaultSerializers)(services);
                    services
                        .AddFilesystem()
                        .AddAssets()
                    //.InitializeVob("/", v => v.AddOwn<VosAssetPersister>(), p => p.Key = $"/<VosAssetPersister>")
                    //.AddAssetPersister()
                    ;
                });
        }

        public static IHostBuilder CreateDefaultVosHost(string[] args = null, bool defaultBuilder = true, FrameworkHostBuilderOptions frameworkOptions = null, IFlex options = null)
        {
            return VosHost.Create(args, defaultBuilder: defaultBuilder, options)
                .AugmentWithFramework(frameworkOptions ?? options?.Get<FrameworkHostBuilderOptions>())
                ;
        }
        public static IHostBuilder CreateDefaultVosAppHost(string[] args = null, bool defaultBuilder = true, FrameworkHostBuilderOptions frameworkOptions = null, IFlex options = null)
        {
            return VosAppHostBuilder.Create(args, defaultBuilder: defaultBuilder, options: options?.Get<VosAppOptions>())
                .AugmentWithFramework(frameworkOptions ?? options?.Get<FrameworkHostBuilderOptions>())
                ;
        }
    }
}
