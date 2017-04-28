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
            return new Handle(referenceable.Reference, referenceable);
        }
    }
}
