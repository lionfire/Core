using LionFire.Referencing;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Assets
{
    public interface IAsset : IAssetPathAware, IReferencable { }
    public interface IAsset<TValue> : IAsset
    {
        Type AssetType { get; }
    }
}
