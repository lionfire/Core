using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Data
{
    public abstract class ConnectionManagerBase<TConnection>
    where TConnection : class, IConnection
    {
        #region Dependencies

        private readonly IConfiguration configuration;
        protected readonly ILogger logger;
        private readonly IServiceProvider serviceProvider;

        #endregion

        #region Configuration

        public string DefaultConnectionStringKey { get; set; }
        public string DefaultConnectionString { get; set; } = "localhost";

        #endregion

        public ConnectionManagerBase(IConfiguration configuration, ILogger logger, IServiceProvider serviceProvider)
        {
            if (DefaultConnectionStringKey == null)
            {
                if (GetType().Name.EndsWith("ConnectionManager"))
                {
                    DefaultConnectionStringKey = GetType().Name.Substring(0, GetType().Name.LastIndexOf("ConnectionManager")) + "Server";
                }
            }
            this.configuration = configuration;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        #region State

        protected ConcurrentDictionary<string, Task<TConnection>> ConnectionsByConnectionStringKey { get; } = new ConcurrentDictionary<string, Task<TConnection>>();

        #endregion

        #region Methods

        /// <summary>
        /// Example of default key in configuration for connection string: "RedisServer
        /// </summary>
        /// <param name="connectionStringConfigurationKey"></param>
        /// <param name="autoConnect"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TConnection> GetConnection(string connectionStringConfigurationKey = null, bool autoConnect = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            var connectionString = configuration[connectionStringConfigurationKey ?? DefaultConnectionStringKey] ?? DefaultConnectionString;

            if (connectionString == null)
            {
                logger.LogInformation($"[resolve] ConnectionStringConfigurationKey: '{connectionStringConfigurationKey}' is disabled.  Returning null for RedisConnection.");
                return null;
            }

            connectionString = connectionString ?? string.Empty; // REVIEW - warn if empty?

            if (ConnectionsByConnectionStringKey.TryGetValue(connectionString, out Task<TConnection> result))
            {
                var connection = await result;
                if (autoConnect)
                {
                    await connection.StartAsync(cancellationToken);
                }
                return connection;
            }

            return await ConnectionsByConnectionStringKey.GetOrAdd(connectionString,
                async n =>
                {
                    var result2 = ActivatorUtilities.CreateInstance<TConnection>(serviceProvider);
                    result2.ConnectionString = connectionString;
                    //result.Logger = (ILogger<TConnection>)serviceProvider.GetService(typeof(ILogger<TConnection>));

                    if (autoConnect)
                    {
                        await result2.StartAsync(cancellationToken);
                    }
                    return result2;
                });
        }

        #endregion
    }
}
