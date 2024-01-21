using LionFire.Data;
using LionFire.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Data.Connections;

/// <summary>
/// FUTURE: retry logic
/// FUTURE: state machine
/// FUTURE: Health monitoring
/// </summary>
public abstract class ConnectionBase<TConnectionOptions, TConcrete> : IHostedService, IConnection
    where TConnectionOptions : ConnectionOptions<TConnectionOptions>
    where TConcrete : ConnectionBase<TConnectionOptions, TConcrete>
{
    protected ILogger logger;

    #region Options

    public TConnectionOptions Options
    {
        get => options;
        protected set
        {
            // REVIEW - when a change notification comes from IOptionsMonitor, is it a new object reference or does it change properties in place? (And is it different for collections vs properties?)
            var oldValue = options;
            options = value;
            OnOptionsChanged(options, oldValue);
        }
    }
    private TConnectionOptions options;
    private readonly string connectionName;
    private IOptionsMonitor<NamedConnectionOptions<TConnectionOptions>> optionsMonitor;

    private void OnOptionsChanged(TConnectionOptions options, TConnectionOptions oldValue)
    {
        Debug.WriteLine("TODO: Options changed");

        if (oldValue != null && oldValue is IHasConnectionString hcs)
        {
            if (hcs.ConnectionString != ((IHasConnectionString)options).ConnectionString)
            {
                Debug.WriteLine("TODO: Connection string changed.  If connected, disconnect to the old and connect to the new connection string.");
            }
        }
    }

    #endregion

    protected ConnectionState ConnectionState { get; set; }

    #region ConnectionString

    /// <summary>
    /// Set by connection manager when creating the connection
    /// </summary>
    public string ConnectionString => (Options as IHasConnectionString)?.ConnectionString;

    public string ConnectionStringSanitized
    {
        get
        {
            var connectionStringSanitized = ConnectionString;
            var pwIndex = connectionStringSanitized.ToLowerInvariant().IndexOf("password=");
            if (pwIndex > -1)
            {
                connectionStringSanitized = connectionStringSanitized.Substring(0, pwIndex) + "[redacted]";
            }
            return connectionStringSanitized;
        }
    }

    #endregion

    protected int connectionCount = 0;

    public ConnectionBase(string connectionName, IOptionsMonitor<NamedConnectionOptions<TConnectionOptions>> optionsMonitor,
ILogger<TConcrete> logger)

    {
        this.connectionName = connectionName;
        this.optionsMonitor = optionsMonitor;
        this.logger = logger;

        Options = optionsMonitor.CurrentValue.GetOptionsOrDefault(connectionName);

        //string cs = configuration.GetConnectionString(connectionName);

        if (Options is IConnectingConnectionOptions cco && cco.AutoConnect) Connect().FireAndForget();
    }

    public async Task Connect(CancellationToken cancellationToken = default)
    {
        try
        {
             await ConnectImpl(cancellationToken)
                //.WithTimeout(Options?.TimeoutMilliseconds,
                //() => throw new TimeoutException($"{this.GetType().Name}: Timed out while trying to connect. (Options: {Options?.ToString()})"))
                ;
            logger.LogInformation("Redis connected: '{name}'", connectionName);
            
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception when attempting to connect on connection name '{connectionName}'", this.connectionName);
        }

    }

    public async Task Disconnect(CancellationToken cancellationToken = default)
   => await DisconnectImpl(cancellationToken).WithTimeout(Options?.TimeoutMilliseconds,
            () => throw new TimeoutException($"{this.GetType().Name}: Timed out while trying to connect. (Options: {Options?.ToString()})"));

    public abstract Task ConnectImpl(CancellationToken cancellationToken = default);
    public abstract Task DisconnectImpl(CancellationToken cancellationToken = default);

    #region IHostedService

    public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        if (connectionCount++ == 0)
        {
            try
            {
                await ConnectImpl(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception when attempting to connect");
                throw;
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        if (--connectionCount <= 0)
        {
            connectionCount = 0;
            await DisconnectImpl(cancellationToken);
        }
    }

    #endregion
}
