namespace LionFire.Instantiating
{
    public interface ITemplateInstance<TemplateType, ParametersType> : ITemplateInstance<TemplateType>
        where TemplateType : class, ITemplate
        where ParametersType : ITemplateParameters
    {
        ITemplateParameters Parameters { get; set; }
    }
    
}
