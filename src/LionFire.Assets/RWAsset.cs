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


    public class RWAsset<TValue> : ReadWriteHandlePassthrough<TValue, IAssetReference>
    {
        public static implicit operator RWAsset<TValue>(string assetPath) => new RWAsset<TValue> { Reference = new AssetReference<TValue>(assetPath) };
    }
}
