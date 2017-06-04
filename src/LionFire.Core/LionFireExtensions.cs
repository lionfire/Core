using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire
{
    // REVIEW - not sure on the point of LionFireException

        //[Obsolete("No point in using this?")]
    public class LionFireException : Exception
    {
        public LionFireException() { }
        public LionFireException(string message) : base(message) { }
        public LionFireException(string message, Exception inner) : base(message, inner) { }
        
    }
}
