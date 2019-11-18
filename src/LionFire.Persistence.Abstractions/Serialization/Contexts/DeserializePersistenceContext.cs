using System;
using System.IO;
using System.Threading.Tasks;
using LionFire.Serialization;

namespace LionFire.Persistence
{
    public class DeserializePersistenceContext 
    {
        public SerializerSelectionContext DeserializerSelectionContext { get; set; } = new SerializerSelectionContext();

        public Func<string, Task<Stream>> PathToStream { get; set; }
        public Func<string, Task<byte[]>> PathToBytes { get; set; }
        public Func<string, Task<string>> PathToString { get; set; }
    }
}
