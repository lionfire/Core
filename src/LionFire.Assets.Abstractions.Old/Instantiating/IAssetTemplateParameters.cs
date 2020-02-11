using LionFire.Instantiating;
using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.Assets
{
    //public abstract class TemplateParameters : ITemplateParameters
    //{
    //    //public abstract AssetReference<ITemplate> Template { get; set; }
    //}

    public interface IAssetTemplateParameters : IKeyed<string>
        , ITemplateOverlayable
        , IHasTemplateAsset
    {
        //ParameterOverlayMode OverlayMode { get; }
        //object OverlayParent { get; set; }
        //ITemplate Template { get; set; }
        //IReadHandle<ITemplateAsset> TemplateAsset { get; set; }
    }
    public interface IAssetTemplateParameters<TemplateType> : IAssetTemplateParameters
        where TemplateType : class, ITemplateAsset
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

    public interface IAssetTemplateParameters<TemplateType, ParametersType, InstanceType>
            : IAssetInstantiation<InstanceType>
            where TemplateType : class, ITemplateAsset
            where ParametersType : class, ITemplateParameters, ITemplateAssetInstance, new()
            where InstanceType : ITemplateAssetInstance, new()
    {
    }

    //public class ParentedTemplateParameters : IParentedTemplateParameters // Base?
    //{
    //    public string ParentKey { get; set; }
    //    public string Key { get; set; }
    //    public ParameterOverlayMode OverlayMode { get; set; }

    //    [Ignore]
    //    public ITemplateParameters OverlayParent { get; set; }
    //}

    public class EmptyTemplateParameters : ITemplateParameters
    {

        public string Key { get; set; }
        //#if AOT
        //object IKeyed<string>.Key { get { return Key; } }
        //#endif

        public IReadHandleBase<ITemplateAsset> TemplateAsset { get; set; }
        public IReadHandleBase<ITemplate> Template { get => TemplateAsset; set => TemplateAsset = (IReadHandleBase<ITemplateAsset>)value; }
        public ParameterOverlayMode OverlayMode { get => ParameterOverlayMode.None; set => throw new NotSupportedException(); }

        [Ignore]
        public object OverlayParent { get => null; set => throw new NotSupportedException(); }

        public IEnumerable<IEnumerable<IInstantiation>> OverlayTargets => Enumerable.Empty<IEnumerable<IInstantiation>>();
    }

    //public abstract class TemplateParameters : ITemplateParameters
    //{
    //    //public abstract AssetReference<ITemplate> Template { get; set; }
    //}
}
