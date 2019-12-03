using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{

    public class ReferenceException : Exception
    {
        public ReferenceException() { }
        public ReferenceException(string message) : base(message) { }
        public ReferenceException(string message, Exception inner) : base(message, inner) { }
    }
}
