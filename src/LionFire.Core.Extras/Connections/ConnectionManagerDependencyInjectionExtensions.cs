using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using LionFire.Data.ExtensionMethods;

namespace LionFire.Data
{
    public static class ConnectionManagerDependencyInjectionExtensions
    {
        public static IHostBuilder AddConnectionManager<TConnection, TConnectionOptions>(this IHostBuilder hostBuilder)
            where TConnection : class, IConnection
            where TConnectionOptions : ConnectionOptions<TConnectionOptions>//, new()
        {
            return hostBuilder.AddConnectionManager<TConnection, TConnectionOptions, ConnectionManager<TConnection, TConnectionOptions>>();
        }

        public static IHostBuilder AddConnectionManager<TConnection, TConnectionOptions, TConnectionManager>(this IHostBuilder hostBuilder)
            where TConnection : IConnection
        where TConnectionManager : class, IConnectionManager<TConnection>
        where TConnectionOptions : ConnectionOptions<TConnectionOptions>//, new()
        {

            hostBuilder.ConfigureServices((context, services) =>
            {
                //services.AddOptions<TConnectionOptions>()
                services
                    .AddSingleton<IConnectionManager<TConnection>, TConnectionManager>()
                    .Configure<NamedConnectionOptions<TConnectionOptions>>(o =>
                    {
                        var section = context.Configuration.GetSection(typeof(TConnectionOptions).GetConfigurationKey());
                        section.Bind(o);
                    });
            });

            return hostBuilder;
        }
    }
}
