using LionFire.Persistence;
using LionFire.Referencing;
using System;

namespace LionFire.Assets
{
    public interface IHRAsset : IHRAsset<object>
    {        
    }

    public interface IHRAsset<out T> : IReadHandleBase<T>
    {
        string AssetTypePath { get; }
        Type Type
        {
            get;
        }
    }
}