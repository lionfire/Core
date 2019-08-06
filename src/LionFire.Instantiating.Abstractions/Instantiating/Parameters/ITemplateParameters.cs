using LionFire.Persistence;
using LionFire.Referencing;
using LionFire.Structures;

namespace LionFire.Instantiating
{
    public interface ITemplateParameters : IKeyed<string>
        , ITemplateOverlayable
    {
        //ParameterOverlayMode OverlayMode { get; }
        //object OverlayParent { get; set; }
        RH<ITemplate> Template { get; set; }
        //IReadHandle<ITemplateAsset> TemplateAsset { get; set; }
    }

    public interface ITemplateParameters<TemplateType> : ITemplateParameters
       where TemplateType : class
    {
    }

    public interface ITemplateParameters<TemplateType, ParametersType>
        : IInstantiation<TemplateType>
        where TemplateType : ITemplate
        where ParametersType : class, ITemplateParameters, ITemplateInstance, new()
    {
    }
}
