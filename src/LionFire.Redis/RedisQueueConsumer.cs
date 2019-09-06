using LionFire.Redis;
using LionFire.Threading;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace LionFire.Redis
{
    public class RedisQueueConsumer
    {
        public Guid Guid;

        public string IsConsumerForKey;
        public string Key;

        public RedisQueueConsumer(string isConsumerForKey)
        {
            Guid = Guid.NewGuid();
            IsConsumerForKey = isConsumerForKey;
            Key = isConsumerForKey + ":consumer:" + Guid.ToString();
        }

        public const string TransactionsKey = "trans";

        public async Task<bool> PutBack(RedisValue result, RedisValue? replacementValue = null)
        {
            var tr = Db.CreateTransaction();

            tr.ListRemoveAsync(Key, result, 1).FireAndForget(); // PERF?  Use LPop instead?
            tr.ListRightPushAsync(IsConsumerForKey, replacementValue ?? result).FireAndForget();

            bool committed = await tr.ExecuteAsync();
            return committed;
        }

        public IDatabase Db => Redis.Db;
        public RedisConnection Redis { get; set; }

        public async Task Done(RedisValue result) => await Db.ListRemoveAsync(Key, result);
    }
}
