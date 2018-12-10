using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LionFire.Referencing;

namespace LionFire.ObjectBus
{
    public static class IOBusKeysExtensions
    {
        public static Task<IEnumerable<string>> Keys(this IReference reference) => reference.GetOBase().GetKeys(reference);
        public static IEnumerable<string> KeyNames(this IReference reference) => reference.GetOBase().GetChildrenNames(reference); // TODO Task<>

    }
}
