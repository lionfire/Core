using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;

namespace LionFire.Assets
{
    public interface IAssetReadWriteHandle : IReferencable<IAssetReference>, IReadWriteHandle
    {
    }

    public interface IHAsset : IAssetReadWriteHandle
    {
        // PORTINGGUIDE - Type > TreatAsType
    } // TEMP TOPORT


    public class RWAsset<TValue> : ReadWriteHandlePassthrough<TValue, IAssetReference>, IAssetReadWriteHandle
        where TValue : IAsset<TValue>
    {

        #region Construction and Implicit Operators

        public RWAsset() { }
        public RWAsset(IReadWriteHandle<TValue> handle) : base(handle) { }
        public RWAsset(TValue asset) { Reference = (AssetReference<TValue>)asset.Reference; Value = asset;   }

        public static implicit operator RWAsset<TValue>(string assetPath) => new RWAsset<TValue> { Reference = new AssetReference<TValue>(assetPath) };
        public static implicit operator RWAsset<TValue>(TValue asset) => new RWAsset<TValue>(asset);
        public static implicit operator AssetReference<TValue>(RWAsset<TValue> asset) => asset.Reference;
        public static implicit operator TValue(RWAsset<TValue> rAsset) => rAsset.Value;

        #endregion

        public string AssetPath => Reference.Path;
        public new AssetReference<TValue> Reference { get => (AssetReference<TValue>)base.Reference; set => base.Reference = value; }


    }
}
