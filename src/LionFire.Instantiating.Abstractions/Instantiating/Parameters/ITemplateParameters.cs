using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Structures;

namespace LionFire.Instantiating
{
    public interface ITemplateParameters : IKeyed<string>, IHasRTemplate
        //, ITemplateOverlayable // This breaks AssetInstantiation
    {
        //ParameterOverlayMode OverlayMode { get; }
        //object OverlayParent { get; set; }
        //IReadHandleBase<ITemplate> RTemplate { get;  }
        //IReadHandle<ITemplateAsset> TemplateAsset { get; set; }
    }

    public interface ITemplateParameters<TTemplate> : ITemplateParameters
       where TTemplate : ITemplate
    {
        new IReadHandleBase<TTemplate> RTemplate { get; set; }
    }
    public interface ITemplateHandleParameters<TTemplate, TTemplateHandle> : ITemplateParameters
      where TTemplate : ITemplate
    {
        new TTemplateHandle RTemplate { get; set; }
    }

    public interface ITemplateParameters<TemplateType, TInstance>
        : IInstantiation<TemplateType>
        , ITemplateParameters
        where TemplateType : ITemplate<TInstance>
        //where ParametersType : ITemplateParameters //, new()
    {
    }
}
