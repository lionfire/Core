using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{

    public class CannotOverwriteException : OBusException // REVIEW: Should this be OBaseException?
    {
        public CannotOverwriteException() { }
        public CannotOverwriteException(string message) : base(message) { }
        public CannotOverwriteException(string message, Exception inner) : base(message, inner) { }
        
    }
}
