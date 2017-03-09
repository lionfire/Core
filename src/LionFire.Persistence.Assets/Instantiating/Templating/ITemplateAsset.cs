using System;
using System.Collections.Generic;
using System.Linq;
using LionFire.Assets;
using System.Threading.Tasks;
using LionFire.Instantiating.Templating;

namespace LionFire.Instantiating
{
    public interface ITemplateAsset : ITemplate, IAsset
    {
    }

    //public static class ITemplateAssetExtensions
    //{
    //    public static IInstantiator ToInstantiator(this ITemplateAsset ta)
    //    {
    //        if (ta == null) return null;

    //        return new TemplateAssetInstantiation
    //        {
    //            AssetSubPath = ta.AssetSubPath,
    //            TypeName = ta.GetType().FullName, // TODO ENH: alias system for short names
    //        };
    //    }
    //}


}
