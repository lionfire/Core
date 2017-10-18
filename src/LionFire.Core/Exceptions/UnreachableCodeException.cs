using System;

namespace LionFire
{
    public class UnreachableCodeException : Exception
    {
        public const string DefaultMessage = "Sanity check failure: reached a code block that should be unreachable.";
        public UnreachableCodeException() : base(DefaultMessage) { }
        public UnreachableCodeException(string message) : base(message) { }
        public UnreachableCodeException(string message, Exception inner) : base(message, inner) { }
    }
}
