using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Referencing;

namespace LionFire.ObjectBus
{
    public static class IOBusKeysExtensions
    {
        //public static Task<IEnumerable<string>> Keys(this IReference reference) => reference.GetCollectionHandle().GetKeys(reference);
        public static Task<IEnumerable<string>> Keys(this IReference reference) => throw new NotImplementedException();
        //public static Task<IEnumerable<string>> KeyNames(this IReference reference) => reference.GetCollectionHandle().GetChildrenNames(reference);
        public static Task<IEnumerable<string>> KeyNames(this IReference reference) => throw new NotImplementedException();
    }
}
