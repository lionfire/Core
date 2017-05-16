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

        public override T ToObject<T>(byte[] bytes, SerializationContext context = null)
        {
            return this.ToObject<T>(UTF8Encoding.UTF8.GetString(bytes), context);
        }

        public override byte[] ToBytes(object obj, SerializationContext context = null)
        {
            return UTF8Encoding.UTF8.GetBytes(this.ToString(obj, context));
        }
    }
}
