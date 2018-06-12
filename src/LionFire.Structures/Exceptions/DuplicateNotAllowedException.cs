using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire
{
    public class DuplicateNotAllowedException : Exception
    {
        public DuplicateNotAllowedException() { }
        public DuplicateNotAllowedException(string message) : base(message) { }
        public DuplicateNotAllowedException(string message, Exception inner) : base(message, inner) { }
    }
}
