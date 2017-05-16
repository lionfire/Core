using LionFire.Applications.Hosting;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using LionFire.Structures;

namespace LionFire.Serialization.Json.Newtonsoft
{
    public static class NewtonsoftJsonFileReadHandleFactoryExtensions
    {
        public static IReadHandle<T> GetJsonFileReadHandle<T>(this string path)
            where T : class
        {
            return new ReadHandle<string, T>()
            {
                Key = path,
                Resolver = ResolveFileToJsonDeserialize<T>
            };
        }

        public static T ResolveFileToJsonDeserialize<T>(string path)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
    }

    public static class NewtonsoftJsonAppHostExtensions
    {
        //public static IAppHost AddNewtonsoftJson(this IAppHost app)
        //{
        //    app.ServiceCollection.Add(new ServiceDescriptor()
        //    {
                
        //    });
        //}
    }
}

//namespace LionFire.Applications.AutoInit
//{
//    public interface IAppInitializer
//    {
//        IAppHost Initializer(IAppHost appHost);
//    }
//    public static class Initializer
//    {
//        public static IAppHost Initialize(this IAppHost appHost)
//        {

//            return appHost;
//        }
//    }
//}
