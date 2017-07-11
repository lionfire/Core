using LionFire.Serialization.Contexts;

namespace LionFire.Serialization
{
    /// <summary>
    /// Deserialize a file using file extensions as hints
    /// </summary>
    public interface IDeserializer
    {
        /// <summary>
        /// Deserialize based on information in the context.  The SerializationContext does not contain enough info to be useful, so a derived class should be supplied.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        T ToObject<T>(SerializationContext context);
    }

}
