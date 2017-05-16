using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Serialization.Contexts
{
    public class MessageSerializationContext : SerializationContext
    {
        public override IEnumerable<DeserializationStrategy> DeserializationStrategies
        {
            get
            {
                yield return DeserializationStrategy.Headers;
                yield return DeserializationStrategy.Detect;
            }
        }

        /// <summary>
        /// E.g. For message buses, this contains out of band info describing a payload.
        /// </summary>
        public object Headers { get; set; }
    }

}
