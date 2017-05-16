using System.Text;

namespace LionFire.Serialization
{
    public abstract class BinarySerializerBase : SerializerBase
    {
        public override T ToObject<T>(string serializedData, object settings = null)
        {
            return this.ToObject<T>(UTF8Encoding.UTF8.GetBytes(serializedData), settings);
        }

        public override string ToString(object obj, object settings = null)
        {
            return UTF8Encoding.UTF8.GetString(this.ToBytes(obj, settings));
        }
    }
}
