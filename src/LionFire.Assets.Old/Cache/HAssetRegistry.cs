using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Assets
{
    public static class HAssetRegistry<AssetType>
        where AssetType : class
    {
        internal static ConcurrentDictionary<AssetIdentifier<AssetType>, HAsset<AssetType>> Registry = new ConcurrentDictionary<AssetIdentifier<AssetType>, HAsset<AssetType>>();
    }
}
