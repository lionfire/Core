//using LionFire.DependencyInjection;
//using LionFire.Serialization.Contexts;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

//namespace LionFire.Serialization
//{
//    public class FileDeserializer : IFileDeserializer
//    {
//        public TValue ToObject<TValue>(SerializationContext context)
//        {
//            return Deserialize<TValue>(((FileSerializationContext)context).FileName, context);
//        }

//        public TValue Deserialize<TValue>(string path, SerializationContext context = null)
//        {
//            var fileContext = context as FileSerializationContext;
//            if(path==null) path = fileContext.FileName;

//            if (path == null) throw new ArgumentNullException(nameof(path));

//            var serializers = new SortedList<double, ISerializerStrategy>();

//            // TODO: Rank decompressers higher

//            double counter = 1;

//            foreach (var s in DependencyContext.Current.GetService<IEnumerable<ISerializerStrategy>>())
//            {
//                foreach (var extension in s.FileExtensions.Where(ext=>path.EndsWith(ext)))
//                {
//                    serializers.Add(counter++, s);
//                    break;
//                }
//            }

//            var selectedSerializer = serializers.Values.FirstOrDefault();

//            if (selectedSerializer == null)
//            {
//                throw new NotSupportedException("No deserializer registered for file with name: " + path);
//            }

//            // TODO: Autoretry
//            var bytes = File.ReadAllBytes(path);

//            return selectedSerializer.ToObject<TValue>(bytes, context);
//        }
//    }

//}
