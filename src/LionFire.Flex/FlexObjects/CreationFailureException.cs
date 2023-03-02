using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire
{

    [Serializable]
    public class CreationFailureException : Exception
    {
        public CreationFailureException() { }
        public CreationFailureException(string message) : base(message) { }
        public CreationFailureException(string message, Exception inner) : base(message, inner) { }
        protected CreationFailureException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
