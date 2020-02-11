#define ASSETCACHEFLAT
#define HASSETG

using LionFire.Persistence;

namespace LionFire.Assets
{
    public interface IHAsset : IHAsset<object>
    {
    }
    public interface IHAsset<T> : IHRAsset<T>, ISaveable
    {
    }

}