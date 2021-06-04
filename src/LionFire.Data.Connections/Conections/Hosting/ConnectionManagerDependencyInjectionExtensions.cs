using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LionFire.Data.Connections;
using LionFire.Data.Connections.ExtensionMethods;
using LionFire.Services;
using System;

namespace LionFire.Data
{
    public static class ConnectionManagerDependencyInjectionExtensions
    {
        public static IHostBuilder AddConnectionManager<TConnection, TConnectionOptions>(this IHostBuilder hostBuilder, IConfiguration configuration = null)
            where TConnection : class, IConnection
            where TConnectionOptions : ConnectionOptions<TConnectionOptions>
                => hostBuilder.AddConnectionManager<TConnection, TConnectionOptions, ConnectionManager<TConnection, TConnectionOptions>>(configuration);

        public static IHostBuilder AddConnectionManager<TConnection, TConnectionOptions, TConnectionManager>(this IHostBuilder hostBuilder, IConfiguration configuration = null)
            where TConnection : IConnection
            where TConnectionManager : class, IConnectionManager<TConnection>
            where TConnectionOptions : ConnectionOptions<TConnectionOptions>
            => hostBuilder.ConfigureServices((context, services) =>
                {
                    services
                        .AddSingleton<TConnectionManager>()
                        .AddSingleton<IConnectionManager<TConnection>>(sp => sp.GetRequiredService<TConnectionManager>())
                        //.AddSingleton<IConnectionManager<TConnection>, TConnectionManager>()
                        .Configure<NamedConnectionOptions<TConnectionOptions>>(o =>
                        {
                            var section = configuration ?? context.Configuration.GetSection(typeof(TConnectionOptions).GetConfigurationKey());
                            section.Bind(o);
                            if (typeof(IHasConnectionStringRW).IsAssignableFrom(typeof(TConnectionOptions)))
                            {
                                foreach(var stringsKvp in o.ConnectionStrings)
                                {
                                    if (!o.Connections.ContainsKey(stringsKvp.Key))
                                    {
                                        var conn = Activator.CreateInstance<TConnectionOptions>();
                                        ((IHasConnectionStringRW)conn).ConnectionString = stringsKvp.Value;
                                    }
                                }
                            }
                        });
                });

        public static IServiceCollection AddConnectionManager<TConnection, TConnectionOptions>(this IServiceCollection services, IConfiguration configuration)
            where TConnection : class, IConnection
            where TConnectionOptions : ConnectionOptions<TConnectionOptions>
                => services.AddConnectionManager<TConnection, TConnectionOptions, ConnectionManager<TConnection, TConnectionOptions>>(configuration);

        public static IServiceCollection AddConnectionManager<TConnection, TConnectionOptions, TConnectionManager>(this IServiceCollection services, IConfiguration configuration)
            where TConnection : IConnection
            where TConnectionManager : class, IConnectionManager<TConnection>
            where TConnectionOptions : ConnectionOptions<TConnectionOptions>
            =>
                services
                            .AddSingleton<TConnectionManager>()
                            .AddSingleton<IConnectionManager<TConnection>>(sp => sp.GetRequiredService<TConnectionManager>())
                            .Configure<NamedConnectionOptions<TConnectionOptions>>(o =>
                        {
                            var section = configuration.GetSection(typeof(TConnectionOptions).GetConfigurationKey());
                            section.Bind(o);
                        });
    }
}
