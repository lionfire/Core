using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace LionFire.Redis
{
    /// <summary>
    /// Returns a RedisConnection (wraps ConnectionMultiplexer) based on a Configuration key for a connection string.  Will reuse connections that match the same
    /// connection string.
    /// </summary>
    public class RedisConnectionManager
    {
        #region Dependencies

        private readonly IConfiguration configuration;
        private readonly ILogger logger;
        private IServiceProvider serviceProvider;

        #endregion

        #region Configuration

        public string DefaultConnectionStringKey { get; set; } = "RedisServer";
        public string DefaultConnectionString { get; set; } = "localhost";

        #endregion

        #region Construction

        public RedisConnectionManager(IConfiguration configuration, ILogger<RedisConnectionManager> logger, IServiceProvider serviceProvider)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        #endregion

        #region State

        private ConcurrentDictionary<string, RedisConnection> ConnectionsByConnectionStringKey { get; } = new ConcurrentDictionary<string, RedisConnection>();

        #endregion

        #region Methods

        public RedisConnection GetConnection(string connectionStringConfigurationKey = null, bool autoConnect = true)
        {
            var connectionString = configuration[connectionStringConfigurationKey ?? DefaultConnectionStringKey] ?? DefaultConnectionString;

            if (connectionString == null)
            {
                logger.LogInformation($"[resolve] ConnectionStringConfigurationKey: '{connectionStringConfigurationKey}' is disabled.  Returning null for RedisConnection.");
                return null;
            }

            return ConnectionsByConnectionStringKey.GetOrAdd(connectionString ?? string.Empty,
                n =>
                {
                    var result = new RedisConnection(connectionString, (ILogger<RedisConnection>)serviceProvider.GetService(typeof(ILogger<RedisConnection>)));
                    if (autoConnect)
                    {
                        result.StartAsync().FireAndForget();
                    }
                    return result;
                });
        }

        #endregion
    }
}
