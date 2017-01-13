using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire
{

    public class TypeNotFoundException : NotFoundException
    {
        public TypeNotFoundException() { }
        public TypeNotFoundException(string message) : base(message) { }
        public TypeNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
