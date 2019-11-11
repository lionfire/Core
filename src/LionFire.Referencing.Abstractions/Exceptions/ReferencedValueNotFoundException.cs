using System;

namespace LionFire.Referencing
{
    public class ReferencedValueNotFoundException : NotFoundException
    {
        public IReferencable Referencable { get; private set; }
        public ReferencedValueNotFoundException() { }
        public ReferencedValueNotFoundException(IReferencable referencable) { this.Referencable = referencable; }
        public ReferencedValueNotFoundException(string message) : base(message) { }
        public ReferencedValueNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
