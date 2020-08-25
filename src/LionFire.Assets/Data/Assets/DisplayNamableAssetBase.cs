using LionFire.Structures;

namespace LionFire.Assets
{
    public class DisplayNamableAssetBase<TValue> : AssetBase<TValue>, IDisplayNamable
    {
        public string DisplayName
        {
            get => displayName ?? DefaultDisplayName;
            set => displayName = value;
        }
        private string displayName; 
        public virtual string DefaultDisplayName => null;

        public DisplayNamableAssetBase() { }
        public DisplayNamableAssetBase(AssetReference<TValue> reference) : base(reference) { }
    }
}
