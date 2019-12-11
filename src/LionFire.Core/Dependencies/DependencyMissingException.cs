using System;
using System.Runtime.Serialization;

namespace LionFire.Dependencies
{
    [Serializable]
    internal class DependencyMissingException : Exception
    {
        public DependencyMissingException()
        {
        }

        public DependencyMissingException(string message) : base(message)
        {
        }

        public DependencyMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DependencyMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}