using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Applications.Hosting;
using LionFire.Serialization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using LionFire.Dependencies;
using LionFire.Persistence.Filesystem;
using LionFire.Hosting.ExtensionMethods;

namespace LionFire.Persistence
{
    public static class PersistersHost
    {
        public static void AddDefaultSerializers(this IServiceCollection services)
        {
            services.AddNewtonsoftJson();
            services.AddTextSerializer(); // .txt
            services.AddBinarySerializer(); // .bin
            services.TryAddEnumerableSingleton<ISerializeScorer, MatchingExtensionSerializeScorer>();
        }

        public static IHostBuilder AllowAsync(this IHostBuilder hostBuilder)
        {
            DependencyLocatorConfiguration.UseServiceProviderToActivateSingletons = false;
            DependencyLocatorConfiguration.UseSingletons = false;
            DependencyContext.AsyncLocal = new DependencyContext();
            return hostBuilder;
        }

        public static IHostBuilder Create(string[] args = null, bool defaultBuilder = true, Action<IServiceCollection> serializers = null)
        {
            IHostBuilder hostBuilder = defaultBuilder ? Host.CreateDefaultBuilder(args) : new HostBuilder();

            return hostBuilder
                .AllowAsync()
                .ConfigureServices((context, services) =>
                {
                    services.AddSerialization();
                    (serializers ?? AddDefaultSerializers)(services);
                    services
                        .AddSingleton<IReferenceToHandleService, ReferenceToHandleService>()
                        .AddSingleton<IReferenceProviderService, ReferenceProviderService>()
                        .AddFilesystem()
                    ;
                });
        }
    }
}