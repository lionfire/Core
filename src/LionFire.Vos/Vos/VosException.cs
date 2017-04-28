using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    [Serializable]
    public class VosException : OBusException
    {
        public VosException() { }
        public VosException(string message) : base(message) { }
        public VosException(string message, Exception inner) : base(message, inner) { }
        protected VosException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
