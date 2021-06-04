using LionFire.Data;
using LionFire.Data.Connections;

namespace LionFire.Redis
{


    public class RedisConnectionOptions : ConnectingConnectionOptions<RedisConnectionOptions>, IHasConnectionStringRW
    {
        //protected new static string ConfigurationKey => typeof(RedisConnectionOptions).Name.Replace("ConnectionOptions", "");

        public string ConnectionString { get; set; }
    }
}
