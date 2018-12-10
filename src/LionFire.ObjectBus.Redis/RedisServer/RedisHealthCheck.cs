#if UNUSED // TODO: use something like this as a health check?
using System;
using StackExchange.Redis;

namespace LionFire.ObjectBus.Redis
{
    public class RedisTest
    {
        /// <summary>
        /// Comma separated host:port
        /// E.g. "server1:6379,server2:6379"
        /// Order not important; master is automatically identified
        /// </summary>
        public string Servers => "localhost";
        //public string Servers => ;

        ConnectionMultiplexer redis;
        public void Start()
        {
            redis = ConnectionMultiplexer.Connect(Servers);

            IDatabase db = redis.GetDatabase();

            {
                string value = "abcdefg";
                db.StringSet("mykey", value);
            }
            {
                string value = db.StringGet("mykey");
                Console.WriteLine(value);
            }
        }

        private void PubSubExample()
        {
            ISubscriber sub = redis.GetSubscriber();

            sub.Subscribe("messages", (channel, message) => {
                Console.WriteLine((string)message);
            });

            sub.Publish("messages", "hello");
        }
    }
}

#endif