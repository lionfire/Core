using System;
using System.Collections.Generic;

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

        IEnumerable<Type> SerializationOptionsTypes { get; }
        //object DefaultDeserializationSettings { get; set; }
        //object DefaultSerializationSettings { get; set; }

    }
}
