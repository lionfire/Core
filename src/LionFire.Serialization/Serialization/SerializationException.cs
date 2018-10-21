using System;
using System.Collections.Generic;
using LionFire.Persistence;

namespace LionFire.Serialization
{
    [System.Serializable]
    public class SerializationException : System.Exception
    {
        public SerializationOperationType Type { get; set; }
        public PersistenceOperation Operation { get; set; }
        public PersistenceContext Context { get; set; }
        public IEnumerable<KeyValuePair<ISerializationStrategy, SerializationResult>> FailReasons { get; set; }

        public SerializationException(SerializationOperationType type, PersistenceOperation operation = null, PersistenceContext context = null, IEnumerable<KeyValuePair<ISerializationStrategy, SerializationResult>> failReasons = null)
        {
            Type = type;
            Operation = operation;
            Context = context;
            FailReasons = failReasons;
        }
        //public SerializationException(SerializationOperationType type, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null, IEnumerable<KeyValuePair<ISerializationStrategy, SerializationResult>> failReasons = null)
        //{
        //    Type = type;
        //    PersistenceContext = context;
        //    FailReasons = failReasons;
        //}
    }
}
