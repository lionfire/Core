using System;
using System.Collections;
using System.Collections.Generic;
using LionFire.Persistence;

namespace LionFire.Serialization
{
    [System.Serializable]
    public class SerializationException : System.Exception
    {
        public SerializationOperationType Type { get; set; }
        public PersistenceOperation Operation { get; set; }

        #region Context

        public PersistenceContext Context
        {
            get => context ?? Operation?.Context;
            set => context = value;
        }
        private PersistenceContext context;

        #endregion


        public IEnumerable<KeyValuePair<ISerializationStrategy, SerializationResult>> FailReasons { get; set; }

        public bool NoSerializerAvailable { get; set; }

        public SerializationException(SerializationOperationType type, PersistenceOperation operation = null, PersistenceContext context = null, IEnumerable<KeyValuePair<ISerializationStrategy, SerializationResult>> failReasons = null, bool noSerializerAvailable = false) : base(noSerializerAvailable?"No serializer available":null)
        {
            Type = type;
            Operation = operation;
            Context = context;
            FailReasons = failReasons;
            NoSerializerAvailable =noSerializerAvailable;
        }
        //public SerializationException(SerializationOperationType type, Lazy<PersistenceOperation> operation = null, PersistenceContext context = null, IEnumerable<KeyValuePair<ISerializationStrategy, SerializationResult>> failReasons = null)
        //{
        //    Type = type;
        //    PersistenceContext = context;
        //    FailReasons = failReasons;
        //}

        
    }
}
