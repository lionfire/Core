// REVIEW - UNUSED? TODEPRECATE ?
using System;

namespace LionFire.Referencing
{
    public struct TypedReference<TValue> : ITypedReference<TValue>
    {
        public TypedReference(IReference reference)
        {
            Reference = reference;
        }
        public Type Type => typeof(TValue);
        public IReference Reference { get; private set; }
    }
}
