#nullable enable
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;

namespace LionFire.Persistence.Redis
{
    public class RedisHandleProvider :
        IReadHandleProvider<RedisReference>
        , IReadWriteHandleProvider<RedisReference>
        , IReadWriteHandleProvider // REVIEW
        //, IReadHandleProvider<ProviderRedisReference>
        , IWriteHandleProvider<RedisReference>
        , IWriteHandleProvider
    {
        IPersister<RedisReference> persister;
        //IPersisterProvider<ProviderRedisReference> providerRedisPersisterProvider;
        public RedisHandleProvider(
            IPersisterProvider<RedisReference> redisPersisterProvider
            //IPersisterProvider<ProviderRedisReference> providerRedisPersisterProvider
            )
        {
            this.persister = redisPersisterProvider.GetPersister();
            //this.providerRedisPersisterProvider = providerRedisPersisterProvider;
        }

        public IReadHandle<T> GetReadHandle<T>(RedisReference reference, T preresolvedValue = default)
            => new PersisterReadHandle<RedisReference, T, IPersister<RedisReference>>(persister, reference, preresolvedValue);
        IReadHandle<T>? IReadHandleProvider.GetReadHandle<T>(IReference reference, T preresolvedValue) => (reference is RedisReference redisReference) ? GetReadHandle<T>(redisReference, preresolvedValue) : null;

        public IReadWriteHandle<T> GetReadWriteHandle<T>(RedisReference reference, T preresolvedValue = default)
            => new PersisterReadWriteHandle<RedisReference, T, IPersister<RedisReference>>(persister, reference, preresolvedValue);
        IReadWriteHandle<T>? IReadWriteHandleProvider.GetReadWriteHandle<T>(IReference reference, T preresolvedValue) => (reference is RedisReference redisReference) ? GetReadWriteHandle<T>(redisReference, preresolvedValue) : null;

        //public IReadHandle<T> GetReadHandle<T>(ProviderRedisReference reference, T preresolvedValue = default)
            //=> new PersisterReadWriteHandle<ProviderRedisReference, T, IPersister<ProviderRedisReference>>(providerRedisPersisterProvider.GetPersister(reference.Persister), reference, preresolvedValue);
        //public IReadHandle<T> GetReadWriteHandle<T>(ProviderRedisReference reference)
        //        => new PersisterReadWriteHandle<ProviderRedisReference, T, IPersister<ProviderRedisReference>>(providerRedisPersisterProvider.GetPersister(reference.Persister), reference);

        //IReadHandle<T> IReadHandleProvider<RedisReference>.GetReadHandle<T>(RedisReference reference) => throw new System.NotImplementedException();

        public IWriteHandle<T> GetWriteHandle<T>(RedisReference reference,T prestagedValue = default) => GetReadWriteHandle<T>(reference, prestagedValue); // REVIEW - 
        IWriteHandle<T>? IWriteHandleProvider.GetWriteHandle<T>(IReference reference,T prestagedValue) => (reference is RedisReference redisReference) ? GetWriteHandle<T>(redisReference, prestagedValue) : null;
    }

    //public static class IReadHandleProviderExtensions
    //{
    //    public static IReadHandle<TValue> GetReadHandle<TValue, TReference>(this IReadHandleProvider<TReference> readHandleProvider, IReference reference)
    //        where TReference : IReference
    //        => (reference is TReference concreteReference) ? readHandleProvider.GetReadHandle<TValue>(concreteReference) : null;
    //}
}
