﻿using System.IO;
using LionFire.Structures;

namespace LionFire.Serialization.Hjson_
{
    // FUTURE - to revive?
    //public static class NewtonsoftJsonFileReadHandleFactoryExtensions
    //{
    //    public static IReadHandle<T> GetJsonFileReadHandle<T>(this string path)
    //        where T : class
    //    {
    //        return new CustomReadHandle<T>(path)
    //        {
    //            Resolver = ResolveFileToJsonDeserialize<T>
    //        };
    //    }

    //    public static T ResolveFileToJsonDeserialize<T>(string path)
    //    {
    //        return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
    //    }
    //}

    
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
