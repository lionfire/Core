using System;
using System.Runtime.Serialization;

namespace LionFire.Persistence
{
    public class PersistenceException : Exception
    {
        public PersistenceException() { }
        public PersistenceException(string message) : base(message) { }
        public PersistenceException(string message, Exception inner) : base(message, inner) { }

        public PersistenceException(IPersistenceResult result, string message = null) : base(message ?? $"Persistence operation failed.  See Result for details.")
        {
            this.Result = result;
        }

        protected PersistenceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IPersistenceResult Result { get; private set; }

    }

}
