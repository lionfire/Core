using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.ObjectBus
{

    [Serializable]
    public class ObjectBusException : Exception
    {
        public ObjectBusException() { }
        public ObjectBusException(string message) : base(message) { }
        public ObjectBusException(string message, Exception inner) : base(message, inner) { }
    }
    
}
