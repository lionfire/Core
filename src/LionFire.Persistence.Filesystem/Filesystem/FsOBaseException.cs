using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus.Filesystem
{
    [Serializable]
    public class FsOBaseException : OBaseException
    {
        public FsOBaseException() { }
        public FsOBaseException(string message) : base(message) { }
        public FsOBaseException(string message, Exception inner) : base(message, inner) { }
        
    }
}
