using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;

namespace LionFire.Data.Connections
{

    // REVIEW - can this be abstracted into a generalized DI Factory class?
    //    public class DependencyInjectionFactory<TItem>
    //    {
    //        private readonly IServiceProvider serviceProvider;
    //        protected ConcurrentDictionary<string, TItem> Items { get; } = new ConcurrentDictionary<string, TItem>();
    //// ...
    //    }

    public class ConnectionManager<TConnection, TConnectionOptions> : IConnectionManager<TConnection>
        where TConnection : class, IConnection
        where TConnectionOptions : ConnectionOptions<TConnectionOptions>
    {
        #region Dependencies

        private readonly IServiceProvider serviceProvider;
        private ILogger Logger { get; }
        private IOptionsMonitor<NamedConnectionOptions<TConnectionOptions>> OptionsMonitor { get; }

        #endregion

        public ConnectionManager(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public ConnectionManager(IOptionsMonitor<NamedConnectionOptions<TConnectionOptions>> options, ILogger logger, IServiceProvider serviceProvider)
        {
            Logger = logger;
            OptionsMonitor = options;
            this.serviceProvider = serviceProvider;
        }

        #region State

        protected ConcurrentDictionary<string, TConnection> ConnectionsByConnectionName { get; } = new ConcurrentDictionary<string, TConnection>();

        #endregion

        #region Methods

        public TConnection DefaultConnection => this[DefaultConnectionName];
        public const string DefaultConnectionName = "";

        /// <summary>
        /// Example of default key in configuration for connection string: "RedisServer
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="autoConnect"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public TConnection this[string connectionName] => ConnectionsByConnectionName.GetOrAdd(connectionName ?? "", name => CreateConnection(serviceProvider, name));

        /// <summary>
        /// Default implementation creates an instance of TConnection with constructor injection, with the connectionName being passed in the constructor.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="connectionName"></param>
        /// <param name="autoConnect"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected TConnection CreateConnection(IServiceProvider serviceProvider, string connectionName) => ActivatorUtilities.CreateInstance<TConnection>(serviceProvider, connectionName);

        #endregion
    }
}
