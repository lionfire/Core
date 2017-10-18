#if MSGPACK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LionFire.Serialization
{
    public class LionPackSerializer : LionSerializer
    {
        LionTypeSerializer lionTypeSerializer = new LionTypeSerializer();

        public override string Name
        {
            get { return "LionPack"; }
        }

        public override byte[][] IdentifyingHeaders
        {
            get { return new byte[][] { UTF8Encoding.UTF8.GetBytes("LIONPACK") }; }
        }

        public override string DefaultFileExtension { get { return "lpk"; } }

        public override T Deserialize<T>(Stream stream)
        {
            ConsumeIdentifyingHeader(stream);

            return (T) lionTypeSerializer.Deserialize(stream, typeof(T));
        }

        public override object Deserialize(System.IO.Stream stream, Type type = null)
        {
            ConsumeIdentifyingHeader(stream);
            return lionTypeSerializer.Deserialize(stream, type);
        }

        private void ConsumeIdentifyingHeader(Stream stream)
        {
            byte[] bytes = new byte[IdentifyingHeaders[0].Length];
            int bytesRead = stream.Read(bytes, 0, IdentifyingHeaders[0].Length);

            if (bytesRead != IdentifyingHeaders[0].Length) throw new SerializationException("Failed to read identifying header.");

            for (int i = 0; i < IdentifyingHeaders[0].Length; i++)
            {
                if (bytes[i] != IdentifyingHeaders[0][i]) throw new SerializationException("Identifying header does not match for this deserializer.");
            }
        }

        public override void Serialize(System.IO.Stream stream, object graph)
        {
            stream.Write(IdentifyingHeaders[0], 0, IdentifyingHeaders[0].Length);

            lionTypeSerializer.Serialize(stream, graph);
        }
    }
}
#endif