using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire
{

    public class DataCorruptException : Exception
    {
        public DataCorruptException() { }
        public DataCorruptException(string message) : base(message) { }
        public DataCorruptException(string message, Exception inner) : base(message, inner) { }
        
    }
}
