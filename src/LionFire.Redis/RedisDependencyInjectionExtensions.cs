using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LionFire.Data;
using LionFire.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LionFire.Services
{
   
    public static class RedisDependencyInjectionExtensions
    {
        public static IHostBuilder AddRedis(this IHostBuilder hostBuilder)
        {
            hostBuilder
                 //.AddConnectionManager<RedisConnectionManager, RedisConnectionOptions>()
//.AddConnectionManager<ConnectionManager<RedisConnection, RedisConnectionOptions>, RedisConnectionOptions>()
                 .AddConnectionManager<RedisConnection, RedisConnectionOptions>()
                ;
            return hostBuilder;
        }
    }
}
