using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    public class OBusReferenceException : OBusException
    {
        public OBusReferenceException() { }
        public OBusReferenceException(string message) : base(message) { }
        public OBusReferenceException(string message, Exception inner) : base(message, inner) { }
    }


}
