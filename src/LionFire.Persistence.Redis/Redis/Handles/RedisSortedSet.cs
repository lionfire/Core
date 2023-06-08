using LionFire.Collections;
using LionFire.Persistence.Redis;
using LionFire.Data.Async.Gets;
using MorseCode.ITask;
using StackExchange.Redis;
using Swordfish.NET.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence.Handles.Redis
{
    public class CollectionEntry : ICollectionEntry
    {
        public string Name { get; set; }
    }

    public class RedisEntry : CollectionEntry
    {
    }

    public class ConcurrentObservableListing<TEntry, TValue> :
        ConcurrentObservableDictionary<string, TValue>
        , IEnumerable<TEntry>
    {
        IEnumerator<TEntry> IEnumerable<TEntry>.GetEnumerator() => throw new NotImplementedException();
    }

    public class RedisSortedSet<TValue>
            : RCollectionBase<RedisReference<ConcurrentObservableListing<RedisEntry, TValue>>, ConcurrentObservableListing<RedisEntry, TValue>, RedisEntry>
            where TValue : class
    {
        #region Dependencies

        public ConnectionMultiplexer Redis
        {
            get => redis;
            set => redis = value;
        }
        private ConnectionMultiplexer redis;

        #endregion

        public override int Count => throw new NotImplementedException();

        public override IEnumerator<RedisEntry> GetEnumerator() => throw new NotImplementedException();
        public override void OnCollectionChangedEvent(INotifyCollectionChangedEventArgs<RedisEntry> a) => throw new NotImplementedException();
        protected override ITask<IGetResult<ConcurrentObservableListing<RedisEntry, TValue>>> ResolveImpl() => throw new NotImplementedException();
    }

}
