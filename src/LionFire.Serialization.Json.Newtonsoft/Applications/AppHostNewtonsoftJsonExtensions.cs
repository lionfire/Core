using LionFire.Serialization;
using LionFire.Serialization.Json.Newtonsoft;

namespace LionFire.Applications.Hosting
{
    public static class NewtonsoftJsonAppHostExtensions
    {
        public static IAppHost AddNewtonsoftJson(this IAppHost app) => app.TryAddEnumerableSingleton<ISerializationStrategy, NewtonsoftJsonSerializer>();
    }
}
