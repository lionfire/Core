// REVIEW - probably move out of ObjectBus TOREFACTOR
//using LionFire.Assets;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.ObjectBus
{
#if UNUSED
    public static class IHasHandlePersistenceExtensions
    {
    #region Misc

        private static ILogger l = Log.Get();

    #endregion


    #region Save 

// UNUSED?  Use IHasHAsset instead?

        public static Task<ICommitResult> Commit(this IHasHandle hasHandle) 
        {
            if (hasHandle.Handle.HasValue && !object.ReferenceEquals(hasHandle.Handle.Value, hasHandle))
            {
                l.Warn("Saving: !object.ReferenceEquals(hasHandle.Handle.Object, hasHandle)");
                //hasHandle.Handle.Object = hasHandle;  --- this doesn't make sense for Ihashandle objects that do not save themselves!
            }
            return hasHandle.Handle.Commit();
        }


#if !AOT

        public static Task<ICommitResult> Save<T>(this IHasHandle<T> hasHandle)
            where T : class
            //, new()
        {
#if ASSETCACHE
            IHasHAsset hha = hasHandle as IHasHAsset;
            if (hha != null)
            {
                hha.Save();
                return;
            }
#endif
            
            if (hasHandle.Handle.HasValue && !object.ReferenceEquals(hasHandle.Handle.Value, hasHandle))
            {
                l.Warn("Saving: !object.ReferenceEquals(hasHandle.Handle.Object, hasHandle).  hasHandle.Handle.Object type: " + hasHandle.Handle.Value?.GetType().Name + ", hasHandle type: " + hasHandle?.GetType().Name 
                    //+ ".  Using hasHandle."
                    );
                //hasHandle.Handle.Object = (T)hasHandle; --- this doesn't make sense for Ihashandle objects that do not save themselves!
            }

            return hasHandle.Handle.Commit();
        }
#endif
    #endregion
}
#endif
}
