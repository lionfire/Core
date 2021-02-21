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
        public static IServiceCollection AddRedis(this IServiceCollection services) // RENAME to AddRedisPersistence, and also have AddRedisConnections which is just LionFire.Redis
            => services
                 .Configure<RedisPersisterOptions>(o => { })
                 .AddSingleton(s => s.GetService<IOptionsMonitor<RedisPersisterOptions>>()?.CurrentValue)

        #region Handles

                 .AddSingleton<RedisHandleProvider>()

                 .AddSingleton<IReadHandleProvider<IRedisReference>, RedisHandleProvider>(s => s.GetRequiredService<RedisHandleProvider>())
                 .AddSingleton<IReadWriteHandleProvider<IRedisReference>, RedisHandleProvider>(s => s.GetRequiredService<RedisHandleProvider>())
                 .AddSingleton<IWriteHandleProvider<IRedisReference>, RedisHandleProvider>(s => s.GetRequiredService<RedisHandleProvider>())

        #endregion

        #region Persister

                 .AddSingleton<RedisPersister>()
                 .AddSingleton<RedisPersisterProvider>()
                 .AddSingleton<IPersisterProvider<IRedisReference>, RedisPersisterProvider>(s => s.GetRequiredService<RedisPersisterProvider>())

        #endregion

                 //.AddSingleton<IReadWriteHandleProvider<RedisReference>, RedisHandleProvider>(s => s.GetRequiredService<RedisHandleProvider>())
                 //.AddSingleton<IWriteHandleProvider<ProviderRedisReference>, RedisHandleProvider>(s => s.GetRequiredService<RedisHandleProvider>())
                 ;
    }
}
