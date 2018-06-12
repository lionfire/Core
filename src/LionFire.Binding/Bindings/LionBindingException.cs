using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Bindings
{
    [Serializable]
    public class LionBindingException
    {
        public LionBindingException() { }
        public LionBindingException(string message) : base(message) { }
        public LionBindingException(string message, Exception inner) : base(message, inner) { }
        protected LionBindingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
