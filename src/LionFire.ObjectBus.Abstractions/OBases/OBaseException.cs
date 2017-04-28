using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    public class OBaseException : OBusException
    {
        public OBaseException() { }
        public OBaseException(string message) : base(message) { }
        public OBaseException(string message, Exception inner) : base(message, inner) { }
    }

}
