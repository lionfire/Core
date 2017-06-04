using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire
{

    public class ReadOnlyException : Exception
    {
        public ReadOnlyException() { }
        public ReadOnlyException(string message) : base(message) { }
        public ReadOnlyException(string message, Exception inner) : base(message, inner) { }
    }
}
