using System;
using LionFire.Persistence;
using LionFire.Structures;
//using System.IO;
//using LionFire.Serialization.Contexts;

namespace LionFire.Serialization
{
    public static class ISerializerExtensions
    {

        /// <summary>
        /// (EXPERIMENTAL API - is this helpful? TODO: if so, do the same for tostring/tostream)
        /// </summary>
        public static (byte[] Bytes, SerializationResult Result) ToBytes(this ISerializer serializer, PersistenceOperation operation, PersistenceContext context = null)
        => serializer.ToBytes(operation.Serialization.Object, ((Func<PersistenceOperation>)(() => operation)).ToLazy(), context);

        //public static (string String, SerializationResult Result) ToString(object obj, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null);
        //public static SerializationResult ToStream(object obj, Stream stream, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null);



        //        public static object ToObject(this ISerializer serializer, byte[] serializedData,  SerializationContext context = null)
        //        {
        //            if (context == null) context = new SerializationContext();
        //            context.BytesData = serializedData;
        //            return serializer.ToObject<object>(context);
        //        }
        //        public static object ToObject(this ISerializer serializer, string serializedData, SerializationContext context = null)
        //        {
        //            if (context == null) context = new SerializationContext();
        //            context.StringData = serializedData;
        //            return serializer.ToObject<object>(context);
        //        }
        //        public static object ToObject(this ISerializer serializer, Stream serializedData, SerializationContext context = null)
        //        {
        //            if (context == null) context = new SerializationContext();
        //            context.Stream = serializedData;
        //            return serializer.ToObject<object>(context);
        //        }

        //        public static byte[] ToBytes(this ISerializer serializer, object obj, SerializationContext context = null)
        //        {
        //            if (context == null) context = new SerializationContext();
        //            context.Object = obj;
        //            return serializer.ToBytes(context);
        //        }
        //        public static string ToString(this ISerializer serializer, object obj, SerializationContext context = null)
        //        {
        //            if (context == null) context = new SerializationContext();
        //            context.Object = obj;
        //            return serializer.ToString(context);
        //        }
    }
}
