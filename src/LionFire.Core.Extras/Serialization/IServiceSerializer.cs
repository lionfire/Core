using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LionFire.Serialization
{
    public interface IServiceSerializer
    {
        void Serialize(Stream stream, object obj, Type expectedType = null);
        
        /// <summary>
        /// Deserialize a service reference.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="expectedType"></param>
        /// <remarks>Value in stream is already checked for a null value.</remarks>
        /// <returns>A proxy to a remote service</returns>
        object Deserialize(Stream stream, Type expectedType = null);

        bool CanSerialize(object obj, Type expectedType = null);

        bool CanDeserialize(Type expectedType);
    }
}
