using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire
{
    /// <summary>
    /// Indicates that the requested operation has already been completed.
    /// </summary>
    public class AlreadyException : Exception
    {
        public AlreadyException() { }
        public AlreadyException(string message) : base(message) { }
        public AlreadyException(string message, Exception inner) : base(message, inner) { }

    }
}
