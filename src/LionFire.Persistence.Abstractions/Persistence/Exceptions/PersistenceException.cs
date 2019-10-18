using System;

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

        public IPersistenceResult Result { get; private set; }

    }

}
