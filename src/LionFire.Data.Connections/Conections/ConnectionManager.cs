using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Data.Connections; // RENAME to LionFire.Connections

// REVIEW - can this be abstracted into a generalized DI Factory class?
//    public class DependencyInjectionFactory<TItem>
//    {
//        private readonly IServiceProvider serviceProvider;
//        protected ConcurrentDictionary<string, TItem> Items { get; } = new ConcurrentDictionary<string, TItem>();
//// ...
//    }


// RENAME to ConnectionCatalog.  Add ConnectionSupervisor class to attempt to reconnect, etc.
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

//#error TODO NEXT: Instead of creating TConnection, create IConnectionSupervisor<TConnection> which will attempt to reconnect, etc.
//#error TODO NEXT: add nullable conn string to CreateConnection?

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

#if TODO
public class ConnectionSupervisor<TConnection, TConnectionOptions> : IHostedService
    where TConnection : class, IConnection
    where TConnectionOptions : ConnectionOptions<TConnectionOptions>
{
    #region Dependencies

    private readonly IServiceProvider serviceProvider;
    private ILogger Logger { get; }
    private IOptionsMonitor<NamedConnectionOptions<TConnectionOptions>> OptionsMonitor { get; }

    public TConnection Connection { get; }

    #endregion

    public ConnectionSupervisor(TConnection connection, IOptionsMonitor<NamedConnectionOptions<TConnectionOptions>> options, ILogger logger, IServiceProvider serviceProvider)

    {
        Connection = connection;
        Logger = logger;
        OptionsMonitor = options;
        this.serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
#endif
