using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LionFire.Data.Connections;
using LionFire.Data.Connections.ExtensionMethods;
using System;
using LionFire.Data;

namespace LionFire.Hosting;

// TODO: Document here that IConfiguration is expected to be the Root
// REVIEW: Can this entire thing be eliminated with new Keyed Services?

public static class ConnectionManagerHostingX
{

    #region IHostApplicationBuilder

    public static IHostApplicationBuilder AddConnectionManager<TConnection, TConnectionOptions, TConnectionManager>(this IHostApplicationBuilder hostApplicationBuilder)
        where TConnection : class, IConnection
        where TConnectionOptions : ConnectionOptions<TConnectionOptions>
        where TConnectionManager : class, IConnectionManager<TConnection>
    {
        hostApplicationBuilder.Services
            .AddConnectionManager<TConnection, TConnectionOptions, TConnectionManager>(hostApplicationBuilder.Configuration);
        return hostApplicationBuilder;
    }

    #endregion

    #region HostApplicationBuilder

    public static HostApplicationBuilder AddConnectionManager<TConnection, TConnectionOptions, TConnectionManager>(this HostApplicationBuilder hostApplicationBuilder)
        where TConnection : class, IConnection
        where TConnectionOptions : ConnectionOptions<TConnectionOptions>
        where TConnectionManager : class, IConnectionManager<TConnection>
        => hostApplicationBuilder.I(i => i.AddConnectionManager<TConnection, TConnectionOptions, TConnectionManager>());

    #endregion

    #region IHostBuilder

    public static IHostBuilder AddConnectionManager<TConnection, TConnectionOptions>(this IHostBuilder hostBuilder, IConfiguration configuration = null)
        where TConnection : class, IConnection
        where TConnectionOptions : ConnectionOptions<TConnectionOptions>
            => hostBuilder.AddConnectionManager<TConnection, TConnectionOptions, ConnectionManager<TConnection, TConnectionOptions>>(configuration);

    public static IHostBuilder AddConnectionManager<TConnection, TConnectionOptions, TConnectionManager>(this IHostBuilder hostBuilder, IConfiguration configuration = null)
        where TConnection : IConnection
        where TConnectionManager : class, IConnectionManager<TConnection>
        where TConnectionOptions : ConnectionOptions<TConnectionOptions>
        => hostBuilder.ConfigureServices((context, services) 
            => services.AddConnectionManager<TConnection, TConnectionOptions, TConnectionManager>(configuration)
            );

    #endregion       

    #region IServiceCollection

    public static IServiceCollection AddConnectionManager<TConnection, TConnectionOptions>(this IServiceCollection services, IConfiguration configuration)
        where TConnection : class, IConnection
        where TConnectionOptions : ConnectionOptions<TConnectionOptions>
            => services.AddConnectionManager<TConnection, TConnectionOptions, ConnectionManager<TConnection, TConnectionOptions>>(configuration);

    public static IServiceCollection AddConnectionManager<TConnection, TConnectionOptions, TConnectionManager>(this IServiceCollection services, IConfiguration configuration)
        where TConnection : IConnection
        where TConnectionOptions : ConnectionOptions<TConnectionOptions>
        where TConnectionManager : class, IConnectionManager<TConnection>
        =>
            services
                .AddSingleton<TConnectionManager>()
                .AddSingleton<IConnectionManager<TConnection>>(sp => sp.GetRequiredService<TConnectionManager>())
                .Configure<NamedConnectionOptions<TConnectionOptions>>(o =>
                {
                    var section = configuration.GetSection(typeof(TConnectionOptions).GetConfigurationKey());
                    section.Bind(o);

                    if (typeof(IHasConnectionStringRW).IsAssignableFrom(typeof(TConnectionOptions)))
                    {
                        foreach (var stringsKvp in o.ConnectionStrings)
                        {
                            if (!o.Connections.ContainsKey(stringsKvp.Key))
                            {
                                var conn = Activator.CreateInstance<TConnectionOptions>();
                                ((IHasConnectionStringRW)conn).ConnectionString = stringsKvp.Value;
                            }
                        }
                    }
                });

    #endregion

    #region (private) IServiceCollection, IConfiguration

    // DUPLICATE
    //private static IServiceCollection __AddConnectionManager<TConnection, TConnectionOptions, TConnectionManager>(this IServiceCollection services, IConfiguration configuration)
    //    where TConnection : IConnection
    //    where TConnectionOptions : ConnectionOptions<TConnectionOptions>
    //    where TConnectionManager : class, IConnectionManager<TConnection>
    //{
    //    services
    //        .AddSingleton<TConnectionManager>()
    //        .AddSingleton<IConnectionManager<TConnection>>(sp => sp.GetRequiredService<TConnectionManager>())
    //        //.AddSingleton<IConnectionManager<TConnection>, TConnectionManager>()
    //        .Configure<NamedConnectionOptions<TConnectionOptions>>(o =>
    //        {
    //            var section = configuration ?? configuration.GetSection(typeof(TConnectionOptions).GetConfigurationKey());
    //            section.Bind(o);
    //            if (typeof(IHasConnectionStringRW).IsAssignableFrom(typeof(TConnectionOptions)))
    //            {
    //                foreach (var stringsKvp in o.ConnectionStrings)
    //                {
    //                    if (!o.Connections.ContainsKey(stringsKvp.Key))
    //                    {
    //                        var conn = Activator.CreateInstance<TConnectionOptions>();
    //                        ((IHasConnectionStringRW)conn).ConnectionString = stringsKvp.Value;
    //                    }
    //                }
    //            }
    //        });
    //}

    #endregion
}
