using System;
using System.Collections.Generic;
using LionFire.Data;
using LionFire.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LionFire.Services
{
    public static class RedisDependencyInjectionExtensions
    {
        public static IHostBuilder AddRedis(this IHostBuilder hostBuilder, IConfiguration configuration = null) 
            => hostBuilder.AddConnectionManager<RedisConnection, RedisConnectionOptions>(configuration);
    }
}
