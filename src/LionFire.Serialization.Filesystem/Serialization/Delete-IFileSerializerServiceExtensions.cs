//using LionFire.DependencyInjection;
//using LionFire.Serialization.Contexts;
//using System;
//using System.IO;

//namespace LionFire.Serialization
//{
//    public static class IFileSerializerServiceExtensions
//    {

//        public static T FileToObject<T>(this ISerializationService service, string path = null, FileSerializationContext context = null)
//            where T : class
//        {
//            if (context == null) context = new FileSerializationContext();

//            if (context.FileName == null) context.FileName = path;

//            if (context.FileName == null) throw new ArgumentNullException("path or context.FileName must be set");

//            return InjectionContext.Current.GetService<ISerializationService>().ToObject<T>(context);
//        }
//    }
//}
