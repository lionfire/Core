using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionFire.Assets
{
    /// <summary>
    /// Provides AssetBase class functionality by composition rather than by inheritance
    /// </summary>
    /// <typeparam name="AssetType"></typeparam>
    public class Asset<AssetType> : AssetBase<AssetType>
        where AssetType : class
    {

        #region Construction

        public Asset(AssetType assetObject)
        {
            this.assetObject = assetObject;
        }

#if true //!AOT
        public Asset(AssetType assetObject, HAsset<AssetType> hAsset)
            : base(hAsset)
        {
            this.assetObject = assetObject;
            hAsset.Object = assetObject;
            
        }
#endif

        #endregion

        internal override AssetType AssetObject {
            get {
                return assetObject;
            }
        }
        private readonly AssetType assetObject;

    }
}
