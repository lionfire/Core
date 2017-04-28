using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    public class OBusException  : Exception
    {
        public OBusException() { }
        public OBusException(string message) : base(message) { }
        public OBusException(string message, Exception inner) : base(message, inner) { }
    }
}
