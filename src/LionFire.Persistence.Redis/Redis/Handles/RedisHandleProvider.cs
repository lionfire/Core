#nullable enable
using LionFire.Persistence.Handles;
using LionFire.Persistence.Persisters;
using LionFire.Referencing;

namespace LionFire.Persistence.Redis
{
//#error NEXT: Should handle providers implement for IRedisReference / IVosReference?
    public class RedisHandleProvider :

        IReadHandleProvider<IRedisReference>
        , IReadHandleProvider
        , IReadWriteHandleProvider<IRedisReference>

        , IReadWriteHandleProvider // REVIEW
                                   //, IReadHandleProvider<ProviderRedisReference>
        , IWriteHandleProvider<IRedisReference>
        , IWriteHandleProvider
    {
        IPersister<IRedisReference> persister;
        //IPersisterProvider<ProviderRedisReference> providerRedisPersisterProvider;
        public RedisHandleProvider(
            IPersisterProvider<IRedisReference> redisPersisterProvider
            //IPersisterProvider<ProviderRedisReference> providerRedisPersisterProvider
            )
        {
            this.persister = redisPersisterProvider.GetPersister();
            //this.providerRedisPersisterProvider = providerRedisPersisterProvider;
        }

        #region IRedisReference

        public IReadHandle<T> GetReadHandle<T>(IRedisReference reference)
            => new PersisterReadHandle<RedisReference<T>, T, IPersister<IRedisReference>>(persister, (IReference<T>) reference);
        public IReadWriteHandle<T> GetReadWriteHandle<T>(IRedisReference reference, T? preresolvedValue = default)
            => new PersisterReadWriteHandle<IRedisReference, T, IPersister<IRedisReference>>(persister, (IReference<T>)reference, preresolvedValue);
        public IWriteHandle<T> GetWriteHandle<T>(IRedisReference reference, T prestagedValue = default) => GetReadWriteHandle<T>(reference, prestagedValue);

        #endregion

        #region IReference



        IReadHandle<T>? IReadHandleProvider.GetReadHandle<T>(IReference reference) => (reference is RedisReference<T> redisReference) ? GetReadHandle<T>(redisReference) : null;
        IReadWriteHandle<T>? IReadWriteHandleProvider.GetReadWriteHandle<T>(IReference reference, T preresolvedValue) => (reference is RedisReference<T> redisReference) ? GetReadWriteHandle<T>(redisReference, preresolvedValue) : null;
        IWriteHandle<T>? IWriteHandleProvider.GetWriteHandle<T>(IReference reference, T prestagedValue) => (reference is RedisReference<T> redisReference) ? GetWriteHandle<T>(redisReference, prestagedValue) : null;

        #endregion

        //public IReadWriteHandle<T> GetReadWriteHandle<T>(RedisReference<T> reference, T? preresolvedValue = default)
            //=> new PersisterReadWriteHandle<RedisReference<T>, T, IPersister<IRedisReference>>(persister, reference, preresolvedValue);

        

        //public IReadHandle<T> GetReadHandle<T>(ProviderRedisReference reference, T preresolvedValue = default)
            //=> new PersisterReadWriteHandle<ProviderRedisReference, T, IPersister<ProviderRedisReference>>(providerRedisPersisterProvider.GetPersister(reference.Persister), reference, preresolvedValue);
        //public IReadHandle<T> GetReadWriteHandle<T>(ProviderRedisReference reference)
        //        => new PersisterReadWriteHandle<ProviderRedisReference, T, IPersister<ProviderRedisReference>>(providerRedisPersisterProvider.GetPersister(reference.Persister), reference);

        //IReadHandle<T> IReadHandleProvider<RedisReference>.GetReadHandle<T>(RedisReference reference) => throw new System.NotImplementedException();

        
        
    }

    //public static class IReadHandleProviderExtensions
    //{
    //    public static IReadHandle<TValue> GetReadHandle<TValue, TReference>(this IReadHandleProvider<TReference> readHandleProvider, IReference reference)
    //        where TReference : IReference
    //        => (reference is TReference concreteReference) ? readHandleProvider.GetReadHandle<TValue>(concreteReference) : null;
    //}
}
