#if TODO
using System.Collections.Generic;
using System.Linq;
using LionFire.Referencing;

namespace LionFire.Vos.VosApp.Old
{
    public static class VobVosAppExtensions
    {
     
        public static string GetPackageStoreSubPath(this Vob vob) => vob.GetPackageStoreSubPathChunks().ToSubPath();

        public static IEnumerable<string> GetPackageStoreSubPathChunks(this Vob vob)
        {
            var subPathChunks = vob.GetSubPathChunksOfAncestor(V.Archives).ToList();
            if (subPathChunks == null || subPathChunks.Count <= 1)
            {
                return null;
            }

            return subPathChunks.Skip(2);
        }
    }
}
#endif
