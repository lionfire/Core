using System.IO;
using LionFire.Structures;

namespace LionFire.Serialization.Json.Newtonsoft
{
    // FUTURE - to revive?
    //public static class NewtonsoftJsonFileReadHandleFactoryExtensions
    //{
    //    public static IReadHandle<TValue> GetJsonFileReadHandle<TValue>(this string path)
    //        where TValue : class
    //    {
    //        return new CustomReadHandle<TValue>(path)
    //        {
    //            Resolver = ResolveFileToJsonDeserialize<TValue>
    //        };
    //    }

    //    public static TValue ResolveFileToJsonDeserialize<TValue>(string path)
    //    {
    //        return JsonConvert.DeserializeObject<TValue>(File.ReadAllText(path));
    //    }
    //}

    
}

//namespace LionFire.Applications.AutoInit
//{
//    public interface IAppInitializer
//    {
//        IAppHost Builder(IAppHost appHost);
//    }
//    public static class Builder
//    {
//        public static IAppHost Initialize(this IAppHost appHost)
//        {

//            return appHost;
//        }
//    }
//}
