using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence
{
    public class ObjectNotFoundException : NotFoundException // RENAME to ValueNotFoundException to match RH.Value?
    {
        public IReadHandleBase<object> ReadHandle { get; private set; }
        public ObjectNotFoundException() { }
        public ObjectNotFoundException(IReadHandleBase<object> rh) { this.ReadHandle = rh; }
        public ObjectNotFoundException(string message) : base(message) { }
        public ObjectNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
