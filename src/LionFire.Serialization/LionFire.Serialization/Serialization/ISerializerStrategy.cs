using System.Collections.Generic;

namespace LionFire.Serialization
{
    public interface ISerializerStrategy : ISerializer
    {
        /// <summary>
        /// Return NaN for not supported
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        float GetPriorityForContext(SerializationContext context);

        SerializationFlags SupportedCapabilities { get; }

        IEnumerable<string> FileExtensions { get; }
        string DefaultFileExtension { get; }

        IEnumerable<string> MimeTypes { get; }

        object DefaultDeserializationSettings { get; set; }
        object DefaultSerializationSettings { get; set; }

    }
}
