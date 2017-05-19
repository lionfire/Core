using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LionFire.Serialization
{
    //public enum StrategyFlags
    //{
    //    None = 0,
    //}

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

        public object MetaData { get; set; }
        public string StringData { get; set; }
        public byte[] BytesData { get; set; }

        public object Object { get; set; }

        public virtual void OnSerialized(ISerializerStrategy s) { }

        // TODO: Throw if can't resolve StringData / BytesData
        public virtual void LoadStringDataIfNeeded() { }
        public virtual void LoadBytesDataIfNeeded() { }
    }

    //public class MimeSerializationContext
    //{
    //    public string MimeType { get; set; }
    //}
}
