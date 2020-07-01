using LionFire.Referencing;
using LionFire.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Assets
{
    /// <summary>
    /// Used to point to IAsset&lt;TValue&gt; instances when TValue is unknown.
    /// </summary>
    public interface IAsset : IAssetPathAware, IReferencable<IAssetReference>, ITreatAsType { }

    /// <summary>
    /// An IAsset&lt;TValue&gt; is a type of object that is aware of its own AssetPath and TValue type, and therefore has all the info it needs
    /// to provide its own IAssetReference.
    /// </summary>
    public interface IAsset<out TValue> : IAsset
    {
        //Type AssetType { get; } // PORTINGGGUIDE - change to TreatAsType
    }
}
