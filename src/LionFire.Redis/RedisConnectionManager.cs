using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using LionFire.Data;
using Microsoft.Extensions.Options;

namespace LionFire.Redis
{
    /// <summary>
    /// Returns a RedisConnection (wraps ConnectionMultiplexer) based on a Configuration key for a connection string.  Will reuse connections that match the same
    /// connection string.
    /// </summary>
    public class RedisConnectionManager : ConnectionManager<RedisConnection, RedisConnectionOptions>
    {
        
        public RedisConnectionManager(IOptionsMonitor<NamedConnectionOptions<RedisConnectionOptions>> options, ILogger<RedisConnectionManager> logger, IServiceProvider serviceProvider) : base(options, logger, serviceProvider)
        {
        }
    }

}
