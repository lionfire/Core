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

namespace LionFire.Hosting;

public static class PersistersHost
{
    #region ILionFireHostBuilder

    public static ILionFireHostBuilder Persisters(this ILionFireHostBuilder hostBuilder)
    {
        hostBuilder.HostBuilder
            .AddPersisters()
            ;
        return hostBuilder;
    }

    #endregion

    public static IHostBuilder AddPersisters(this IHostBuilder hostBuilder)
        => hostBuilder.ConfigureServices((context, services) => services.AddPersisters());

    public static IServiceCollection AddPersisters(this IServiceCollection services)
              => services
                          .AddSerialization()
                          .AddBuiltInSerializers()
                          .AddSingleton<IPersistenceConventions, PersistenceConventions>()
                          .AddSingleton<IReferenceToHandleService, ReferenceToHandleService>()
                          .AddReferenceProvider()
                          .AddSingleton<IHandleRegistry, WeakHandleRegistry>()
                          .Configure<ObjectHandleProviderOptions>(c =>
                          {
                              c.AutoRegister = true;
                              c.CheckIReferencable = true;
                              c.ReuseHandles = true;
                          })
                          .AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptionsMonitor<ObjectHandleProviderOptions>>().CurrentValue)
                          .AddSingleton<IObjectHandleProvider, ObjectHandleProvider>()
                      ;

    #region OLD

    [Obsolete("Use LionFireHostBuilder")]
    public static IHostBuilder Create(string[] args = null)
     => Host.CreateDefaultBuilder(args)
            .LionFire()
            .AddPersisters();

    #endregion
}