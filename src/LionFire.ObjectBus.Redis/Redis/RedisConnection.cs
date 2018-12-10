//using System;
//using StackExchange.Redis;

//namespace LionFire.ObjectBus.Redis
//{
//    public class RedisConnection : OBaseConnection<RedisOBase>
//    {
//        public RedisConnection(IConnectingOBase obase) : base(obase) {
//        }

//        ConnectionMultiplexer redis;
//        public void Start()
//        {
//            redis = ConnectionMultiplexer.Connect(ConnectionString);

//            IDatabase db = redis.GetDatabase();

//            {
//                string value = "abcdefg";
//                db.StringSet("mykey", value);
//            }
//            {
//                string value = db.StringGet("mykey");
//                Console.WriteLine(value);
//            }
//        }

//        private void PubSubExample()
//        {
//            ISubscriber sub = redis.GetSubscriber();

//            sub.Subscribe("messages", (channel, message) => {
//                Console.WriteLine((string)message);
//            });

//            sub.Publish("messages", "hello");
//        }
//    }
    
//}
