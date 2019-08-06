using LionFire.Applications.Hosting;
using LionFire.Referencing;
using LionFire.Persistence.Handles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Hosting
{
    public static class FrameworkHost
    {
        public static void AddDefaultSerializers(this IServiceCollection services)
        {
            //services.AddJson();
            services.AddNewtonsoftJson();
        }

        public static IHostBuilder Create(Action<IServiceCollection> serializers = null)
        {
            return new HostBuilder()
                .UseDefaults()
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
