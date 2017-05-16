using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LionFire.Serialization.Contexts
{

    public class SerializationContext
    {
        public object SerializationOptions { get; set; }

        public SerializationFlags Flags { get; set; }


        public virtual IEnumerable<DeserializationStrategy> DeserializationStrategies
        {
            get
            {
                yield return DeserializationStrategy.Detect;
            }
        }

        public object Context { get; set; }

    }

    //public class MimeSerializationContext
    //{
    //    public string MimeType { get; set; }
    //}
}
