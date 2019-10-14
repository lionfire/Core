using LionFire.Data;
using LionFire.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyCouch;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.CouchDB
{

    /// <summary>
    /// Connection string:
    ///   Comma separated host:port
    ///   E.g. "server1:6379,server2:6379"
    ///   Order not important; master is automatically identified
    /// </summary>
    public class CouchDBConnection : ConnectionBase
    {
        //public IDatabase Db => redis.GetDatabase();
        //public ConnectionMultiplexer CouchDB => redis;
        //private ConnectionMultiplexer redis;

        public CouchDBConnection(ILogger<CouchDBConnection> logger) : base(logger)
        {
        }

        //public CouchDBConnection(string connectionString, ILogger<CouchDBConnection> logger) : base(logger)
        //{
        //    ConnectionString = connectionString;
        //}

        public bool IsConnectionDesired
        {
            get => isConnectionDesired;
            set
            {
                if (value)
                {
                    Connect().FireAndForget();
                }
                else
                {
                    Disconnect().FireAndForget();
                }
            }
        }
        private bool isConnectionDesired;
        //private Task<ConnectionMultiplexer> connectingTask;

        public override async Task Connect(CancellationToken cancellationToken = default(CancellationToken))
        {
#if TODO
        start:
#region Detect already done or in progress REVIEW

            if (redis != null)
            {
                return;
            }

            if (connectingTask != null)
            {
                var copy = connectingTask;
                if (copy != null)
                {
                    await copy;
                    return;
                }
                else
                {
                    goto start;
                }
            }

#endregion

            isConnectionDesired = true;
            logger.LogDebug($"[CONNECTING] Connecting to redis at {ConnectionString}...");
            connectingTask = ConnectionMultiplexer.ConnectAsync(ConnectionString);
            redis = connectingTask.Result;
            connectingTask = null;
            logger.LogInformation($"[connected] ...connected to redis at {ConnectionString}");
#endif
        }

        public override async Task Disconnect(CancellationToken cancellationToken = default(CancellationToken))
        {
#if TODO
            isConnectionDesired = false;
            if (redis != null)
            {
                var redisCopy = redis;
                redis = null;
                try
                {
                    logger.LogDebug($"[DISCONNECTING] Disconnecting from redis at {ConnectionString}...");
                    await redisCopy.CloseAsync(true);
                    logger.LogInformation($"[disconnected] ...disconnected from redis at {ConnectionString}");
                }
                finally
                {
                    redisCopy.Dispose();
                }
            }
#endif
        }
    }
}
