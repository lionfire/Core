using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Assets
{
    public static class IAssetExtensions
    {
        public static void Save(this IAsset obj)
        {
            obj.Save(obj.AssetSubPath);
        }
    }
}
