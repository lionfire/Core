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
    public interface IHRAsset : IAssetReadHandle
    {
        // PORTINGGUIDE - Type > TreatAsType
    } // TEMP TOPORT

    public class RAsset<TValue> : ReadHandlePassthrough<TValue, IAssetReference>
        where TValue : IAsset<TValue>
    {
        public static implicit operator RAsset<TValue>(string assetPath) => new RAsset<TValue> { Reference = new AssetReference<TValue>(assetPath) };
        public static implicit operator RAsset<TValue>(TValue asset) => new RAsset<TValue> { Reference = asset.Reference, Value = asset };
    }
}
