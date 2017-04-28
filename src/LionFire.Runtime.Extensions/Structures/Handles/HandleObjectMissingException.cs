using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Structures
{
    public class HandleObjectMissingException : Exception
    {
        public IReadHandle<object> ReadHandle { get; private set; }
        public HandleObjectMissingException() { }
        public HandleObjectMissingException(IReadHandle<object> rh) { this.ReadHandle = rh; }
        public HandleObjectMissingException(string message) : base(message) { }
        public HandleObjectMissingException(string message, Exception inner) : base(message, inner) { }
    }
}
