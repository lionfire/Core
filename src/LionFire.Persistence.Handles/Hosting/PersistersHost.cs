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

namespace LionFire.Services
{
    public static class PersistersHost
    {
        public static IHostBuilder Create(string[] args = null, bool defaultBuilder = true)
        {
            IHostBuilder hostBuilder = defaultBuilder ? Host.CreateDefaultBuilder(args) : new HostBuilder();
            return hostBuilder.AddPersisters();
        }

        public static IHostBuilder AddPersisters(this IHostBuilder hostBuilder) => hostBuilder.ConfigureServices((context, services) => services.AddPersisters());

        public static IServiceCollection AddPersisters(this IServiceCollection services)
                  => services
                              .AddSerialization()
                              .AddBuiltInSerializers()
                              .AddSingleton<IPersistenceConventions, PersistenceConventions>()
                              .AddSingleton<IReferenceToHandleService, ReferenceToHandleService>()
                              .AddSingleton<IReferenceProviderService, ReferenceProviderService>()
                          ;
    }
}