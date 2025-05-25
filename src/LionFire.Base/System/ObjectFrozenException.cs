using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Structures
{

    [Serializable]
    public class ObjectFrozenException : Exception
    {
        public ObjectFrozenException() { }
        public ObjectFrozenException(string message) : base(message) { }
        public ObjectFrozenException(string message, Exception inner) : base(message, inner) { }
        
        // Obsolete
        //protected ObjectFrozenException(
        //  System.Runtime.Serialization.SerializationInfo info,
        //  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
