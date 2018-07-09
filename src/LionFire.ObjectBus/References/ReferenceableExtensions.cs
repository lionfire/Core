using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.ObjectBus
{
    public static class ReferenceableExtensions
    {
        public static IHandle CreateHandle(this IReferencable referenceable)
        {
            throw new NotImplementedException("How to do this?  Previous implementation: create a plain Handle class");
            //return new Handle(referenceable.Reference, referenceable);
        }
    }
}
