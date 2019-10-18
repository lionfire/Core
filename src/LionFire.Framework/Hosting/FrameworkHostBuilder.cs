using LionFire.Applications.Hosting;
using LionFire.Referencing;
using LionFire.Persistence.Handles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using LionFire.Hosting;
using LionFire.Hosting.ExtensionMethods;
using LionFire.ObjectBus.Filesystem;
using LionFire.ObjectBus;

namespace LionFire.Hosting
{

    public static class FrameworkHostBuilder
    {
        public static void AddDefaultSerializers(this IServiceCollection services)
        {
            //services.AddJson();
            services.AddNewtonsoftJson();
        }

        public static IHostBuilder CreateVos(string[] args = null, Action<IServiceCollection> serializers = null)
        {
            return Create(args, serializers: serializers)
                .AddObjectBus<FSOBus>()
                .AddVosApp();
        }

        public static IHostBuilder CreateDefault(string[] args = null, Action<IServiceCollection> serializers = null)
        {
            return Create(args, serializers: serializers)
                .AddObjectBus<FSOBus>()
                .AddVosApp();
        }

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
            IHostBuilder hostBuilder = defaultBuilder ? Host.CreateDefaultBuilder(args) : new HostBuilder();

            return hostBuilder
                .UseDependencyContext()
                //.ConfigureLogging(loggingBuilder =>
                //{
                //})
                .ConfigureServices((context, services) =>
                {
                    services
                    .AddSerialization();

                    (serializers ?? AddDefaultSerializers)(services);

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
    }
}
