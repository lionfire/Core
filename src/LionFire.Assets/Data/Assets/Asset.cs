using System;
using System.Collections.Generic;
using System.Text;

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

        //public Asset(AssetType assetObject)
        //{
        //    this.assetObject = assetObject;
        //}
        public Asset(AssetType assetObject, AssetReference<AssetType> reference = null) : base(reference) { 
            this.assetObject = assetObject;
        }

        #endregion

        internal AssetType AssetObject => assetObject;
        private readonly AssetType assetObject;

    }
}
