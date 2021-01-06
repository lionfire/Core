using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using LionFire.Applications.Hosting;
using LionFire.Serialization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using LionFire.Dependencies;
using LionFire.Hosting;
using LionFire.DependencyInjection.ExtensionMethods;
using LionFire.Persistence;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace LionFire.Hosting
{
    public static class PersistersHost
    {
        public static IHostBuilder Create(IConfiguration config = null, string[] args = null, bool defaultBuilder = true)
         => LionFireHost.Create(config, args, defaultBuilder)
                .AddPersisters();

        public static IHostBuilder AddPersisters(this IHostBuilder hostBuilder)
            => hostBuilder.ConfigureServices((context, services) => services.AddPersisters());

        public static IServiceCollection AddPersisters(this IServiceCollection services)
                  => services
                              .AddSerialization()
                              .AddBuiltInSerializers()
                              .AddSingleton<IPersistenceConventions, PersistenceConventions>()
                              .AddSingleton<IReferenceToHandleService, ReferenceToHandleService>()
                              .AddSingleton<IReferenceProviderService, ReferenceProviderService>()
                              .Configure<ObjectHandleProviderOptions>(c =>
                              {
                                  c.AutoRegister = true;
                                  c.CheckIReferencable = true;
                                  c.ReuseHandles = true;
                              })
                              .AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptionsMonitor<ObjectHandleProviderOptions>>().CurrentValue)
                              .AddSingleton<IObjectHandleProvider, ObjectHandleProvider>()
                          ;
    }
}