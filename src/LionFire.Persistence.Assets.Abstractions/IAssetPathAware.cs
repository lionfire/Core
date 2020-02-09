namespace LionFire.Assets
{
    public interface IAssetPathAware
    {
        /// <summary>
        /// Set by AssetPersister when retrieving asset
        /// </summary>
        [SetOnce]
        string AssetPath { get; set; }
    }
}
