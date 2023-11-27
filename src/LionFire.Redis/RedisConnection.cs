using LionFire.Data;
using LionFire.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LionFire.Data.Connections;

namespace LionFire.Redis
{
    // FUTURE - state machine?
    //public enum ConnectionStates
    //{
    //    Unspecified,
    //    Connected,
    //    Connecting,
    //    WaitingToReconnect,
    //    Reconnecting,
    //    FailedToConnect,
    //    Disconnected,
    //}

    public static class AsyncEnumerable
    {
#pragma warning disable CS1998
        public static async IAsyncEnumerable<T> Empty<T>()
#pragma warning restore CS1998
        {
            yield break;
        }
    }

    /// <summary>
    /// Connection string:
    ///   Comma separated host:port
    ///   E.g. "server1:6379,server2:6379"
    ///   Order not important; master is automatically identified
    /// </summary>
    public class RedisConnection : OptionsConnectionBase<RedisConnectionOptions, RedisConnection>
    {

        public IDatabase Db => redis.GetDatabase();
        public ConnectionMultiplexer Redis => redis;
        private ConnectionMultiplexer redis;

        public void BatchesExecute<T>(IEnumerable<T> source, int batchSize, Action<List<T>> action)
        {
            List<T> batch = new List<T>();
            foreach (var item in source)
            {
                batch.Add(item);
                if (batch.Count == batchSize)
                {
                    action(batch);
                    batch = new List<T>();
                }
            }
            if (batch.Count != 0)
            {
                action(batch);
            }
        }

        public IAsyncEnumerable<RedisKey> Keys(int database = -1, RedisValue pattern = default, int pageSize = 250)
        {
            foreach (var endpoint in redis.GetEndPoints().OfType<System.Net.DnsEndPoint>())
            {
                var server = redis.GetServer(endpoint.Host + ":" + endpoint.Port);
                return server.KeysAsync(database, pattern, pageSize);
            }

            return AsyncEnumerable.Empty<RedisKey>();

            //if (pattern == null)
            //{
            //    var result = redis.GetDatabase(0).Execute("scan", 0, "match", pattern);
            //    throw new NotImplementedException();
            //} else
            //{
            //    var result = redis.GetDatabase(0).Execute("scan", 0, "match", pattern);
            //    return result.Select(key => (string)key);
            //}
        }

        public RedisConnection(string name, IOptionsMonitor<NamedConnectionOptions<RedisConnectionOptions>> options, ILogger<RedisConnection> logger) : base(name, options, logger)
        {
        }

        public bool IsConnectionDesired
        {
            get => isConnectionDesired;
            set
            {
                if (value)
                {
                    ConnectImpl().FireAndForget();
                }
                else
                {
                    DisconnectImpl().FireAndForget();
                }
            }
        }
        private bool isConnectionDesired;
        private Task<ConnectionMultiplexer> connectingTask;

        public override async Task ConnectImpl(CancellationToken cancellationToken = default(CancellationToken))
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
            var msg = $"[CONNECTING] Connecting to redis at {ConnectionString ?? "null"}...";
            if (ConnectionString == null)
            {
                logger.LogError(msg);
                throw new ArgumentNullException(nameof(ConnectionString), msg);
            }
            else
            {
                logger.LogDebug(msg);
            }
            connectingTask = ConnectionMultiplexer.ConnectAsync(ConnectionString);
            redis = connectingTask.Result;
            connectingTask = null;
            var connectionStringSanitized = ConnectionString;
            var pwIndex = connectionStringSanitized.IndexOf("password=");
            if (pwIndex > -1)
            {
                connectionStringSanitized = connectionStringSanitized.Substring(0, pwIndex);
            }

            logger.LogInformation($"[connected] ...connected to redis at {connectionStringSanitized}");
        }

        public override async Task DisconnectImpl(CancellationToken cancellationToken = default(CancellationToken))
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


    }
}
