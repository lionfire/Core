using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Applications.Hosting;

namespace LionFire.Persistence
{
    public static class PersistersHost
    {
        public static void AddDefaultSerializers(this IServiceCollection services)
        {
            //services.AddJson();
            services.AddNewtonsoftJson();
        }

        public static IHostBuilder Create(string[] args = null, bool defaultBuilder = true, Action<IServiceCollection> serializers = null)
        {
            IHostBuilder hostBuilder = defaultBuilder ? Host.CreateDefaultBuilder(args) : new HostBuilder();

            return hostBuilder
                .ConfigureServices((context, services) =>
                {
                    services.AddSerialization();
                    (serializers ?? AddDefaultSerializers)(services);
                    services
                        .AddSingleton<IReferenceToHandleService, ReferenceToHandleService>()
                        .AddSingleton<IReferenceProviderService, ReferenceProviderService>()
                    ;
                });
        }
    }
}