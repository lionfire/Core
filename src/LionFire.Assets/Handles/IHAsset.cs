#define ASSETCACHEFLAT
#define HASSETG

namespace LionFire.Assets
{
    public interface IHAsset : IHRAsset
    {
        bool HasPathOrObject { get; }
        void Save();
    }

}