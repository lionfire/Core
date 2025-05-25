using LionFire.Persistence;
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
        // TODO: Add other OBus methods?

        //public static H<T> CreateHandle(this IReferenceable referenceable)
        //{
        //    throw new NotImplementedException("How to do this?  Previous implementation: create a plain Handle class");
        //    //return new Handle(referenceable.Reference, referenceable);
        //}

        /// <summary>
        /// RENAME: List
        /// TODO: add generic?
        /// </summary>
        /// <param name="referencable"></param>
        /// <returns></returns>
        public static Task<IEnumerable<string>> List<T>(this IReferenceable referencable) => referencable.Reference.List<T>();

    }
}
