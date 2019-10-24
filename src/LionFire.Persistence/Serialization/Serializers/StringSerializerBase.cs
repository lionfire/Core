//using System;
//using System.IO;
//using System.Text;

//namespace LionFire.Serialization
//{
//    public abstract class StringSerializerBase : SerializerBase
//    {
//        public override SerializationFlags SupportedCapabilities => 
//            SerializationFlags.Text
//            | SerializationFlags.HumanReadable
//            | SerializationFlags.Deserialize
//            | SerializationFlags.Serialize;

//        //public override TValue ToObject<TValue>(SerializationContext context)
//        //{
//        //    return this.ToObject<TValue>(UTF8Encoding.UTF8.GetString(context.BytesData), context);
//        //}

//        //public override byte[] ToBytes(object obj, SerializationContext context)
//        //{
//        //    return UTF8Encoding.UTF8.GetBytes(this.ToString( context));
//        //}

//        //public override void ToStream(object obj, Stream stream, SerializationContext context)
//        //{
//        //    var bytes = this.ToBytes;
//        //    stream.Write;
//        //}
//    }
//}
