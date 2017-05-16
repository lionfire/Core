namespace LionFire.Serialization
{
    public static class ISerializerExtensions
    {
        /// <summary>
        /// Returns true if the flags are supported by this serializer
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool SupportsCapability(this ISerializer serializer, SerializationFlags flags)
        {
            return (serializer.SupportedCapabilities & flags) == flags;
        }

        public static object ToObject(this ISerializer serializer, byte[] serializedData)
        {
            return serializer.ToObject<object>(serializedData);
        }
        public static object ToObject(this ISerializer serializer, string serializedData)
        {
            return serializer.ToObject<object>(serializedData);
        }

    }

}
