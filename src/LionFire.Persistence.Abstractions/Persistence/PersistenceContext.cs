using System;
using LionFire.Serialization;

namespace LionFire.Persistence
{
    public class PersistenceContext // : IMultiTypable
    {
        //public MultiType MultiTyped => multiTyped.Value;
        //protected Lazy<MultiType> multiTyped = new Lazy<MultiType>();

        public Func<PersistenceOperation> GetPersistenceOperation { get; set; }

        public string RootPath { get; set; }
        public object RootObject { get; set; }

        /// <summary>
        /// Defaults to typeof(object) which will save the full type information.
        /// </summary>
        public Type SaveType { get; set; }

        public bool AllowInstantiator { get; set; }
        
        public SerializationContext SerializationContext { get; set; }
        public ISerializationProvider SerializationProvider { get; set; }

        public SerializePersistenceContext Serialization { get; set; }
        public DeserializePersistenceContext Deserialization { get; set; }
    }
}
