namespace LionFire.Serialization
{
    public static class ISerializerStrategyExtensions
    {
        /// <summary>
        /// Returns true if the flags are supported by this serializer
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static bool SupportsCapability(this ISerializationStrategy serializer, SerializationFlags flags)
        {
            return (serializer.SupportedCapabilities & flags) == flags;
        }
    }

}
