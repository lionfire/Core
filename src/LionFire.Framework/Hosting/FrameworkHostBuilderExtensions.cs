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
using Microsoft.Extensions.Configuration;

namespace LionFire.Hosting
{

    public static class FrameworkSerializerServiceCollectionExtensions
    {
        public static IServiceCollection AddDefaultSerializers(this IServiceCollection services)
        {
            services.AddBuiltInSerializers();
            //services.AddNewtonsoftJson();
            return services;
        }
    }

    public class FrameworkHostBuilderOptions
    {
        public bool DefaultSerializers { get; set; } = true;
    }

    public static class FrameworkHostBuilderExtensions
    {
        #region ILionFireHostBuilder

        /// <summary>
        /// Add the full opinionated LionFire Kitchen Sink(tm)
        /// </summary>
        /// <param name="lf"></param>
        /// <param name="frameworkHostBuilderOptions"></param>
        /// <returns></returns>
        public static ILionFireHostBuilder Framework(this ILionFireHostBuilder lf, FrameworkHostBuilderOptions frameworkHostBuilderOptions = null)
        {
            lf
                .VosApp()
                .HostBuilder
                    .AugmentWithFramework(frameworkHostBuilderOptions)
                    ;
            return lf;
        }

        #endregion

        #region IHostBuilder

        private static IHostBuilder AugmentWithFramework(this IHostBuilder hostBuilder, FrameworkHostBuilderOptions options = null)
        {
            return hostBuilder
                .ConfigureServices((context, services) =>
                {
                    services
                        .If(options?.DefaultSerializers != false, sc=>sc.AddDefaultSerializers())
                        //.AddSerialization()
                        //.AddPersisters()
                        .AddReferenceProvider()
                        .AddFilesystem()
                    //.AddIdPersister()
                    //.AddAssets()

                    //.InitializeVob("/", v => v.AddOwn<VosAssetPersister>(), p => p.Key = $"/<VosAssetPersister>")
                    //.AddAssetPersister()
                    ;
                });
        }

        #endregion
        

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
            //IHostBuilder hostBuilder = defaultBuilder ? Host.CreateDefaultBuilder(args) : new HostBuilder();
            //IHostBuilder hostBuilder = VosAppHostBuilder.Create(args: args, defaultBuilder: defaultBuilder);

            return Host.CreateDefaultBuilder(args).LionFire(b =>
                {
                    b
                        .Framework()
                        ;
                })
                ;

            //return hostBuilder
            //.ConfigureLogging(loggingBuilder =>
            //{
            //})
            //.ConfigureServices((context, services) =>
            //{
            //    //services
            //    //.AddSerialization()
            //    //.AddPersisters()
            //    //.AddFilesystem()
            //    //;

            //    //(serializers ?? DefaultAddDefaultSerializers)(services);

            //    //services
            //    //.AddSingleton<IReferenceToHandleService, ReferenceToHandleService>()
            //    //.AddSingleton<IReferenceProviderService, ReferenceProviderService>()
            //    //;
            //    //services.Configure<ObjectBusOptions>(option => // Allows injection of IOptions<ObjectBusOptions>
            //    //{
            //    //    option.SampleOption = 123;
            //    //});

            //})
            //.ConfigureHostConfiguration(config =>
            //{

            //})
            //.ConfigureAppConfiguration((context, config) =>
            //{
            //    //context.HostingEnvironment.
            //})
            ;
        }

        //[Obsolete]
        //public static IHostBuilder CreateDefault(IConfiguration config = null, string[] args = null, bool defaultBuilder = true, IHostBuilder hostBuilder = null, FrameworkHostBuilderOptions frameworkOptions = null, IFlex options = null
        //    //, Action<IServiceCollection> serializers = null
        //    )
        //    => CreateDefaultVosAppHost(config, args, defaultBuilder, hostBuilder: hostBuilder, frameworkOptions: frameworkOptions, options: options);

        
        //public static IHostBuilder CreateDefaultPersisterHost(string[] args = null, bool defaultBuilder = true, FrameworkHostBuilderOptions frameworkOptions = null, IFlex options = null)
        //{
        //    return Create(args, defaultBuilder)
        //        .AugmentWithFramework(frameworkOptions ?? options?.Get<FrameworkHostBuilderOptions>())
        //        ;
        //}

        //public static IHostBuilder CreateDefaultVosHost(IConfiguration config = null, string[] args = null, bool defaultBuilder = true, IHostBuilder hostBuilder = null, FrameworkHostBuilderOptions frameworkOptions = null)
        //{
        //    return VosHost.Create(config, args, defaultBuilder: defaultBuilder, hostBuilder: hostBuilder)
        //        .AugmentWithFramework(frameworkOptions ?? options?.Get<FrameworkHostBuilderOptions>())
        //        ;
        //}
        //public static IHostBuilder CreateDefaultVosAppHost(IConfiguration config = null, string[] args = null, bool defaultBuilder = true, IHostBuilder hostBuilder = null, FrameworkHostBuilderOptions frameworkOptions = null, IFlex options = null)
        //{
        //    return VosAppHostBuilder.Create(config, args, defaultBuilder: defaultBuilder, hostBuilder: hostBuilder, options: options?.Get<VosAppOptions>())
        //        .AugmentWithFramework(frameworkOptions ?? options?.Get<FrameworkHostBuilderOptions>())
        //        ;
        //}
    }
}
