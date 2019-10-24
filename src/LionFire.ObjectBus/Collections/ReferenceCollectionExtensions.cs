using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
    public static class ReferenceCollectionExtensions
    {
        public static Task<IEnumerable<string>> List<T>(this IReference reference)
            => throw new NotImplementedException();
            //reference.ToReadCollectionHandle<TValue>().List();
    }
}
