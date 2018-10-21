using System;
using System.IO;
using LionFire.Serialization;

namespace LionFire.Persistence
{
    public class PersistenceContext
    {
        public string RootPath { get; set; }
        public object RootObject { get; set; }

        /// <summary>
        /// Defaults to typeof(object) which will save the full type information.
        /// </summary>
        public Type SaveType { get; set; }

        public bool AllowInstantiator { get; set; }
        public SerializationContext SerializationContext { get; set; }

        public Func<PersistenceOperation> GetPersistenceOperation { get; set; }
        public ISerializationProvider SerializationProvider { get; set; }

        public SerializePersistenceContext Serialization { get; set; }
        public DeserializePersistenceContext Deserialization { get; set; }

    }
    public class SerializePersistenceContext 
    {
        public SerializerSelectionContext SerializerSelectionContext { get; set; } = new SerializerSelectionContext();
    }

    public class DeserializePersistenceContext 
    {
        public SerializerSelectionContext DeserializerSelectionContext { get; set; } = new SerializerSelectionContext();

        // TODO Make async
        public Func<string, Stream> PathToStream { get; set; }
        public Func<string, byte[]> PathToBytes { get; set; }
        public Func<string, string> PathToString { get; set; }
    }
}
