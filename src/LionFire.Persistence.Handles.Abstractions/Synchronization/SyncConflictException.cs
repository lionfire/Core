using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Synchronization
{

    public class SyncConflictException : Exception
    {
        public SyncConflictException() { }
        public SyncConflictException(string message) : base(message) { }
        public SyncConflictException(string message, Exception inner) : base(message, inner) { }
    }

}
