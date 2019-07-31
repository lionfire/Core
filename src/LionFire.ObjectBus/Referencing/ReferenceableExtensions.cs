using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    public static class ReferenceableExtensions
    {
        public static IHandle CreateHandle(this IReferencable referenceable)
        {
            throw new NotImplementedException("How to do this?  Previous implementation: create a plain Handle class");
            //return new Handle(referenceable.Reference, referenceable);
        }

        public static Task<IEnumerable<string>> GetKeys(this IReferencable referencable) => referencable.Reference.GetKeys();

    }
}
