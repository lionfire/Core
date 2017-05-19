using System;
using System.Text;

namespace LionFire.Serialization
{
    public abstract class StringSerializerBase : SerializerBase
    {
        public override SerializationFlags SupportedCapabilities => 
            SerializationFlags.Text
            | SerializationFlags.HumanReadable
            | SerializationFlags.Deserialize
            | SerializationFlags.Serialize;

        //public override T ToObject<T>(SerializationContext context)
        //{
        //    return this.ToObject<T>(UTF8Encoding.UTF8.GetString(context.BytesData), context);
        //}
        

        public override byte[] ToBytes(SerializationContext context)
        {
            return UTF8Encoding.UTF8.GetBytes(this.ToString( context));
        }
    }
}
