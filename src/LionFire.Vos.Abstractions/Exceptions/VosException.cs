using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LionFire.ObjectBus;

namespace LionFire.Vos
{
    [Serializable]
    public class VosException : OBusException
    {
        public VosException() { }
        public VosException(string message) : base(message) { }
        public VosException(string message, Exception inner) : base(message, inner) { }
    }
}
