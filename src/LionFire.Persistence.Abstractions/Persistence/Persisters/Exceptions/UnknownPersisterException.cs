using System;
using System.Runtime.Serialization;

namespace LionFire.Persistence
{
    [Serializable]
    public class UnknownPersisterException : PersistenceException
    {
        public UnknownPersisterException()
        {
        }

        public UnknownPersisterException(string message) : base(message)
        {
        }

        public UnknownPersisterException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnknownPersisterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}