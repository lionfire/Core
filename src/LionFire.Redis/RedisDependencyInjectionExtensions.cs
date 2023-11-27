using System;
using System.Collections.Generic;
using LionFire.Data;
using LionFire.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionFire.Hosting;

public static class RedisDependencyInjectionExtensions
{
    public static IHostApplicationBuilder AddRedisConnectionManager(this IHostApplicationBuilder hostApplicationBuilder) 
        => hostApplicationBuilder.AddConnectionManager<RedisConnection, RedisConnectionOptions, RedisConnectionManager>();

    public static IServiceCollection AddRedisConnectionManager(this IServiceCollection services, IConfiguration configuration)
        => services.AddConnectionManager<RedisConnection, RedisConnectionOptions, RedisConnectionManager>(configuration);
}

