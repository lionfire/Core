using LionFire.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LionFire.Assets
{
    public interface IAsset //: ISaveable
    {
        Type Type { get; }
        string AssetSubPath { get; }

        AssetID ID { get; }
    }
}
