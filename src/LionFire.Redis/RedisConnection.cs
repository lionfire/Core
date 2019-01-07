using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Redis
{
    public class RedisConnection : IHostedService
    {
        public IDatabase Db => redis.GetDatabase();
        public ConnectionMultiplexer Redis => redis;
        private ConnectionMultiplexer redis;

        private readonly ILogger logger;

        /// <summary>
        /// Comma separated host:port
        /// E.g. "server1:6379,server2:6379"
        /// Order not important; master is automatically identified
        /// </summary>
        public string ConnectionString { get; private set; }

        public RedisConnection(string connectionString, ILogger<RedisConnection> logger)
        {
            ConnectionString = connectionString;
            this.logger = logger;
        }

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
        private Task<ConnectionMultiplexer> connectingTask;

        public async Task Connect()
        {
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
        }

        public async Task Disconnect()
        {
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
        }

        #region IHostedService

        // TODO: cancellationToken
        public async Task StartAsync(CancellationToken cancellationToken = default(CancellationToken)) => await Connect();

        // TODO: cancellationToken
        public async Task StopAsync(CancellationToken cancellationToken = default(CancellationToken)) => await Disconnect();

        #endregion
    }
}
