using LionFire.Serialization.Contexts;

namespace LionFire.Serialization
{
    public static class ISerializerExtensions
    {

        public static object ToObject(this ISerializer serializer, byte[] serializedData, SerializationContext context = null)
        {
            if (context == null) context = new SerializationContext();
            context.BytesData = serializedData;
            return serializer.ToObject<object>(context);
        }
        public static object ToObject(this ISerializer serializer, string serializedData, SerializationContext context = null)
        {
            if (context == null) context = new SerializationContext();
            context.StringData = serializedData;
            return serializer.ToObject<object>(context);
        }

        public static byte[] ToBytes(this ISerializer serializer, object obj, SerializationContext context = null)
        {
            if (context == null) context = new SerializationContext();
            context.Object = obj;
            return serializer.ToBytes(context);
        }
        public static string ToString(this ISerializer serializer, object obj, SerializationContext context = null)
        {
            if (context == null) context = new SerializationContext();
            context.Object = obj;
            return serializer.ToString(context);
        }
    }
}
