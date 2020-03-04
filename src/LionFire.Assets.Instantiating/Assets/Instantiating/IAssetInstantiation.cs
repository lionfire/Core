using LionFire.Instantiating;
using System;
using System.Collections.Generic;
using System.Text;

namespace LionFire.Assets
{
    public interface IAssetInstantiation : IInstantiation, IHasTemplateAsset
    {
        IEnumerable<IAssetInstantiation> AllChildren { get; }
    }
}
