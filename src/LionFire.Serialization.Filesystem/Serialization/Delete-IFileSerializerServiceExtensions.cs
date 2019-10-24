//using LionFire.DependencyInjection;
//using LionFire.Serialization.Contexts;
//using System;
//using System.IO;

//namespace LionFire.Serialization
//{
//    public static class IFileSerializerServiceExtensions
//    {

//        public static TValue FileToObject<TValue>(this ISerializationService service, string path = null, FileSerializationContext context = null)
//            where TValue : class
//        {
//            if (context == null) context = new FileSerializationContext();

//            if (context.FileName == null) context.FileName = path;

//            if (context.FileName == null) throw new ArgumentNullException("path or context.FileName must be set");

//            return DependencyContext.Current.GetService<ISerializationService>().ToObject<TValue>(context);
//        }
//    }
//}
