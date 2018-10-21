using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Bindings
{
    [Serializable]
    public class LionBindingException : Exception
    {
        public LionBindingException() { }
        public LionBindingException(string message) : base(message) { }
        public LionBindingException(string message, Exception inner) : base(message, inner) { }
    }
}
