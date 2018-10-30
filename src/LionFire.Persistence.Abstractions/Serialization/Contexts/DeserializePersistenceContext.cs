using System;
using System.IO;
using LionFire.Serialization;

namespace LionFire.Persistence
{
    public class DeserializePersistenceContext 
    {
        public SerializerSelectionContext DeserializerSelectionContext { get; set; } = new SerializerSelectionContext();

        // TODO Make async
        public Func<string, Stream> PathToStream { get; set; }
        public Func<string, byte[]> PathToBytes { get; set; }
        public Func<string, string> PathToString { get; set; }
    }
}
