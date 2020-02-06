using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Persistence.Assets
{

    public static class IReferencableAssetExtensions
    {
        //// FUTURE: Make more generalized by having a SubPath method, and an interface implementing IReference that provides SubPath
        //[Obsolete("Use SubPath() instead")]
        //public static string AssetSubPath(this IReferencable<ISubPathReference> subPathReferencable)
        //{
        //    return subPathReferencable.Reference.SubPath;
        //}
        public static string AssetSubPath(this IReferencable<AssetReference> subPathReferencable) // RENAME to SubPath once Valor is compiling again
        {
            return subPathReferencable.Reference.SubPath;
        }
    }
}
