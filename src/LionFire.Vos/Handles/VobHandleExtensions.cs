#define ConcurrentHandles
#define WARN_VOB
//#define INFO_VOB
#define TRACE_VOB

using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Referencing;

namespace LionFire.Extensions.ObjectBus
{
    public static class VobHandleExtensions
    {
        #region NonVirtual

        public static IEnumerable<IReference> ToNonVirtualReferences(this RH<object> parent)
        {
            foreach (var handle in ToNonVirtualHandle(parent).Select(h => h.Reference))
            {
                yield return handle;
            }
        }

        public static IEnumerable<IHandle> ToNonVirtualHandle(this RH<object> parent)
        {
            throw new NotImplementedException("TOPORT");
            // TOPORT
            //var vhParent = parent as IVobHandle;
            //if (vhParent == null) { yield return parent; yield break; }

            //foreach (var vhChild in vhParent.Vob.ReadHandles)
            //{
            //    foreach (var result in ToNonVirtualHandle(vhChild))
            //    {
            //        yield return result;
            //    }
            //}
        }

        #endregion
    }
}
