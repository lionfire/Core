using LionFire.Referencing;

namespace LionFire.ObjectBus.Redis
{
    public class RedisPath
    {
        public static char SeparatorChar = ':';
        public static string Separator = ":";

        public static string PathToRedisPath(string path) => path.Replace(LionPath.SeparatorChar, RedisPath.SeparatorChar);
    }
}
