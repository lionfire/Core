using System;

namespace LionFire.Referencing
{
    public class ReferencedValueNotFoundException : NotFoundException
    {
        public IReferenceable Referenceable { get; private set; }
        public ReferencedValueNotFoundException() { }
        public ReferencedValueNotFoundException(IReferenceable referencable) { this.Referenceable = referencable; }
        public ReferencedValueNotFoundException(string message) : base(message) { }
        public ReferencedValueNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
