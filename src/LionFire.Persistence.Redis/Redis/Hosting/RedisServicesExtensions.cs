using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Persistence.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace LionFire.Hosting
{
    public static class RedisServicesExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services)
            => services
                 .Configure<RedisPersisterOptions>(o => { })
                 .AddSingleton(s => s.GetService<IOptionsMonitor<RedisPersisterOptions>>()?.CurrentValue)
                 .AddSingleton<RedisPersister>()

                 .AddSingleton<RedisHandleProvider>()
                 .AddSingleton<IReadHandleProvider<RedisReference>, RedisHandleProvider>(s => s.GetRequiredService<RedisHandleProvider>())
                 .AddSingleton<IReadWriteHandleProvider<RedisReference>, RedisHandleProvider>(s => s.GetRequiredService<RedisHandleProvider>())

                 .AddSingleton<RedisPersisterProvider>()
                 .AddSingleton<IPersisterProvider<RedisReference>, RedisPersisterProvider>(s => s.GetRequiredService<RedisPersisterProvider>())
                 .AddSingleton<IPersisterProvider<RedisReference>, RedisPersisterProvider>(s => s.GetRequiredService<RedisPersisterProvider>())

                 .AddSingleton<IWriteHandleProvider<RedisReference>, RedisPersisterProvider>(s => s.GetRequiredService<RedisHandleProvider>())
                 //.AddSingleton<IReadWriteHandleProvider<RedisReference>, RedisHandleProvider>(s => s.GetRequiredService<RedisHandleProvider>())
                 //.AddSingleton<IWriteHandleProvider<ProviderRedisReference>, RedisHandleProvider>(s => s.GetRequiredService<RedisHandleProvider>())
                 ;
    }
}
