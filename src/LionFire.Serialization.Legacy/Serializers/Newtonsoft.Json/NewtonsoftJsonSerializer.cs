using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace LionFire.Serialization
{
    public class NewtonsoftJsonSerializer : LionSerializer
    {
        public override string Name => "Newtonsoft Json";

        public override byte[][] IdentifyingHeaders
        {
            get { return new byte[][] { UTF8Encoding.UTF8.GetBytes("("), UTF8Encoding.UTF8.GetBytes("{") }; }
        }

        public override string DefaultFileExtension { get { return "json"; } }

        public override T Deserialize<T>(Stream stream)
        {
            throw new NotImplementedException();
        }

        public override object Deserialize(Stream stream, Type type)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(Stream stream, object graph)
        {
            throw new NotImplementedException();
        }
    }
}
