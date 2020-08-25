using LionFire.Persistence.Handles;

namespace LionFire.Assets
{
    public class WAsset<TValue> : WriteHandlePassthrough<TValue, IAssetReference>
    {
        public static implicit operator WAsset<TValue>(string assetPath) => new WAsset<TValue> { Reference = new AssetReference<TValue>(assetPath) };
    }
}
