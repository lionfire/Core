using System;

namespace LionFire.Assets
{
    public interface IHRAsset : IReadHandle
    {
        string AssetTypePath { get; }
        Type Type {
            get;
        }
    }
}