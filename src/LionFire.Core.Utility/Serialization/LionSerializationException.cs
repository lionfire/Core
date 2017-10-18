using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Serialization
{
    public class LionSerializationException : Exception
    {
        public LionSerializationException() { }
        public LionSerializationException(string message) : base(message) { }
        public LionSerializationException(string message, Exception inner) : base(message, inner) { }
   
    }
}
