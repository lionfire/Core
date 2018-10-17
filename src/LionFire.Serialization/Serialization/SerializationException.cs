using System.Collections.Generic;
using LionFire.Persistence;

namespace LionFire.Serialization
{
    [System.Serializable]
    public class SerializationException : System.Exception
    {
        public SerializationOperationType Type { get; set; }
        public SerializationOperation Operation { get; set; }
        public SerializationContext Context { get; set; }
        public PersistenceContext PersistenceContext { get; set; }
        public IEnumerable<KeyValuePair<ISerializationStrategy, SerializationResult>> FailReasons { get; set; }
        public SerializationException(SerializationOperationType type, SerializationOperation operation = null, SerializationContext context = null, IEnumerable<KeyValuePair<ISerializationStrategy, SerializationResult>> failReasons = null)
        {
            Type = type;
            Operation = operation;
            Context = context;
            FailReasons = failReasons;
        }
        public SerializationException(SerializationOperationType type, PersistenceContext context = null, IEnumerable<KeyValuePair<ISerializationStrategy, SerializationResult>> failReasons = null)
        {
            Type = type;
            PersistenceContext = context;
            FailReasons = failReasons;
        }
    }
}
