using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Assets;
using System.Threading.Tasks;
using LionFire.Instantiating.Templating;
using LionFire.Instantiating;

namespace LionFire.Assets
{
    public interface ITemplateAsset : ITemplate, IAsset
    {
        //IAssetInstantiation CreateAssetInstantiation();
    }
}
