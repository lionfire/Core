using System;
using System.Collections.Generic;
using LionFire.Data;
using LionFire.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting
{
    public static class RedisDependencyInjectionExtensions
    {
        public static IHostBuilder AddRedisConnectionManager(this IHostBuilder hostBuilder, IConfiguration configuration = null) 
            => hostBuilder.AddConnectionManager<RedisConnection, RedisConnectionOptions>(configuration);

        public static IServiceCollection AddRedisConnectionManager(this IServiceCollection services, IConfiguration configuration)
            => services.AddConnectionManager<RedisConnection, RedisConnectionOptions>(configuration);
    }
}
