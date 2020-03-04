using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Structures;

namespace LionFire.Instantiating
{
    public interface ITemplateParameters : IKeyed<string>
        //, ITemplateOverlayable // This breaks AssetInstantiation
    {
        //ParameterOverlayMode OverlayMode { get; }
        //object OverlayParent { get; set; }
        IReadHandleBase<ITemplate> Template { get;  }
        //IReadHandle<ITemplateAsset> TemplateAsset { get; set; }
    }

    public interface ITemplateParameters<TemplateType> : ITemplateParameters
       where TemplateType : ITemplate
    {
        new IReadHandleBase<TemplateType> Template { get; set; }
    }

    public interface ITemplateParameters<TemplateType, TInstance>
        : IInstantiation<TemplateType>
        , ITemplateParameters
        where TemplateType : ITemplate<TInstance>
        //where ParametersType : ITemplateParameters //, new()
    {
    }
}
