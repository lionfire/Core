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

    // OLD Dupe - REVIEW:

    //// TODO: Make it optional for instance types to implement ITemplateInstance!  The related features can then be opt-in

    ///// <summary>
    ///// Templates are a lightweight parameterized factory framework.  
    ///// No factories are needed.  Simply call 
    ///// </summary>
    ////[Asset("")]
    //public interface ITemplateAsset
    //    : IAsset, ITemplate
    ////: ITreeNode<ITemplate>
    ////: INode
    //{
    //    IAssetInstantiation CreateAssetInstantiation();
    //}
}
