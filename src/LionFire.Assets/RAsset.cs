using LionFire.Assets;
using LionFire.Persistence;
using LionFire.Persistence.Handles;
using LionFire.Referencing;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Assets
{
    public interface IAssetReadHandle : IReferencable<IAssetReference>, IReadHandle
    {
    }
    public interface IHRAsset : IAssetReadHandle // PORTINGGUIDE IHRAsset > RAsset
    {
        // PORTINGGUIDE - Type > TreatAsType
    } // TEMP TOPORT

    public class RAsset<TValue> : ReadHandlePassthrough<TValue, IAssetReference>, IAssetReadHandle
        where TValue : IAsset<TValue>
    {
        public static implicit operator RAsset<TValue>(string assetPath) => new RAsset<TValue> { Reference = new AssetReference<TValue>(assetPath) };
        public static implicit operator RAsset<TValue>(TValue asset) => new RAsset<TValue> { Reference = (AssetReference<TValue>)asset.Reference, Value = asset };
        public static implicit operator AssetReference<TValue>(RAsset<TValue> asset) => (AssetReference<TValue>)asset.Reference; // TODO add to RWAsset, make sure RWAsset has feature parity

        public RAsset() { }
        public RAsset(IReadHandle<TValue> handle) : base(handle) { }

        public static implicit operator TValue(RAsset<TValue> rAsset) => rAsset.Value;

        public string AssetPath => Reference.Path;
        public new AssetReference<TValue> Reference { get => (AssetReference<TValue>)base.Reference; set => base.Reference = value; }


    }
    public static class RAssetExtensions
    {
        public static RAsset<TValue> ToRAsset<TValue>(this IReadHandle<TValue> readHandle)
            where TValue : IAsset<TValue>
            => new RAsset<TValue>(readHandle);
    }
}
