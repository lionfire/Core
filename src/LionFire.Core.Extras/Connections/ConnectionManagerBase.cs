using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace LionFire.Data
{

    public abstract class ConnectionManagerBase<TConnection>
        where TConnection : class, IConnection, new()
    {
        #region Dependencies

        private readonly IConfiguration configuration;
        protected readonly ILogger logger;
        private IServiceProvider serviceProvider;

        #endregion

        #region Configuration

        public string DefaultConnectionStringKey { get; set; }
        public string DefaultConnectionString { get; set; } = "localhost";

        #endregion
        
        public ConnectionManagerBase(IConfiguration configuration, ILogger logger, IServiceProvider serviceProvider)
        {
            if (DefaultConnectionStringKey == null)
            {
                if (this.GetType().Name.EndsWith("ConnectionManager"))
                {
                    DefaultConnectionStringKey = this.GetType().Name.Substring(0, this.GetType().Name.LastIndexOf("ConnectionManager")) + "Server";
                }
            }
            this.configuration = configuration;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        #region State

        protected ConcurrentDictionary<string, TConnection> ConnectionsByConnectionStringKey { get; } = new ConcurrentDictionary<string, TConnection>();

        #endregion

        #region Methods
        
        public TConnection GetConnection(string connectionStringConfigurationKey = null, bool autoConnect = true)
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
                    var result = new TConnection();
                    result.ConnectionString = connectionString;
                    result.Logger = (ILogger<TConnection>)serviceProvider.GetService(typeof(ILogger<TConnection>));

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
