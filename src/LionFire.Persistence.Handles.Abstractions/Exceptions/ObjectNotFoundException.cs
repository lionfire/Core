using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Referencing
{
    public class ObjectNotFoundException : NotFoundException
    {
        public IReadHandle<object> ReadHandle { get; private set; }
        public ObjectNotFoundException() { }
        public ObjectNotFoundException(IReadHandle<object> rh) { this.ReadHandle = rh; }
        public ObjectNotFoundException(string message) : base(message) { }
        public ObjectNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
