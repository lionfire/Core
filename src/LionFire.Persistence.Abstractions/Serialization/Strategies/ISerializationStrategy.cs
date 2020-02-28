using LionFire.IO;
using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.Serialization
{
    //public interface ISerializationStrategyHints
    //{
    //    float 
    //}

    public interface ISerializationStrategy : ISerializer
    {
        ///// <summary>
        ///// Return NaN for not supported
        ///// </summary>
        ///// <param name="context"></param>
        ///// <returns></returns>
        //float GetPriorityForContext(SerializationContext context);

        SerializationFlags SupportedCapabilities { get; }

        //IEnumerable<string> FileExtensions { get; }
        //string DefaultFileExtension { get; }

        IEnumerable<SerializationFormat> Formats { get; }
        SerializationFormat DefaultFormat { get; }

        IEnumerable<ISerializeScorer> SerializeScorers { get; }
        IEnumerable<IDeserializeScorer> DeserializeScorers { get; }

        IEnumerable<Type> SerializationOptionsTypes { get; }
        //object DefaultDeserializationSettings { get; set; }
        //object DefaultSerializationSettings { get; set; }

        bool ImplementsToString { get; }
        bool ImplementsToStream { get; }
        bool ImplementsToBytes { get; }

        bool ImplementsFromString { get; }
        bool ImplementsFromStream { get; }
        bool ImplementsFromBytes { get; }
    }

    public static class ISerializationStrategyExtensions
    {
        public static IEnumerable<string> SupportedExtensions(this ISerializationStrategy strategy, IODirection direction)
        {
            if (!strategy.SupportedCapabilities.SupportsDirection(direction)) return Enumerable.Empty<string>();
            return strategy.Formats.SelectMany(f => f.FileExtensions).Distinct();
        }
    }
}
